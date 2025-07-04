using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;
using Serilog;
using System.Net.Http.Headers;

namespace AprovaFacil.Application.Services;

public class RequestService(RequestInterfaces.IRequestRepository repository, UserInterfaces.IUserRepository userRepository, ServerDirectory serverDirectory, NotificationInterfaces.INotificationService notificationService, NotificationInterfaces.INotificationRepository notificationRepository, ITenantProvider tenantProvider, ITenantRepository tenantRepository, IUnitOfWorkInterface unitOfWork, IHttpClientFactory httpClientFactory) : RequestInterfaces.IRequestService
{
    public async Task<Result> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();
            if (!tenantId.HasValue)
            {
                return Result.Failure(ErrorType.Unathorized, "TenantId não encontrado.");
            }

            Tenant? currentTenant = await tenantRepository.GetByIdAsync(tenantId.Value, cancellation);
            if (currentTenant == null)
            {
                return Result.Failure(ErrorType.NotFound, "Informações do tenant não encontradas.");
            }

            currentTenant.SetLimitsBasedOnPlan();

            if (ShouldResetRequests(currentTenant.LastRequestResetDate))
            {
                currentTenant.CurrentRequestsThisMonth = 0;
                currentTenant.LastRequestResetDate = new DateTime(
                    DateTime.UtcNow.Year,
                    DateTime.UtcNow.Month,
                    1,
                    0, 0, 0,
                    DateTimeKind.Utc);
            }

            if (currentTenant.CurrentRequestsThisMonth >= currentTenant.MaxRequestsPerMonth)
            {
                return Result.Failure(ErrorType.Forbidden, $"Você atingiu o limite de {currentTenant.MaxRequestsPerMonth} requisições para o seu plano este mês.");
            }

            Int32[] usersId = [.. request.ManagersId, .. request.DirectorsIds, request.RequesterId];
            Dictionary<Int32, IApplicationUser> users = await userRepository.GetUsersDictionary(usersId, cancellation);

            Request? newRequest = new Request
            {
                InvoiceName = Guid.NewGuid(),
                BudgetName = Guid.NewGuid(),
                CompanyId = request.CompanyId,
                Amount = request.Amount,
                UUID = request.UUID,
                PaymentDate = request.PaymentDate,
                CreateAt = request.CreateAt,
                Note = request.Note,
                RequesterId = request.RequesterId,
                TenantId = tenantId.Value,
                Level = LevelRequest.Pending,
            };

            foreach (Int32 manager in request.ManagersId)
            {
                newRequest.Managers.Add(new RequestManager { ManagerId = manager, RequestUUID = newRequest.UUID });
            }

            foreach (Int32 director in request.DirectorsIds)
            {
                newRequest.Directors.Add(new RequestDirector { DirectorId = director, RequestUUID = newRequest.UUID });
            }

            newRequest.HasInvoice = request.Invoice.Length > 0;
            newRequest.HasBudget = request.Budget.Length > 0;

            await repository.RegisterRequestAsync(newRequest, cancellation);

            currentTenant.CurrentRequestsThisMonth++;
            await tenantRepository.UpdateAsync(currentTenant, cancellation);

            //await UploadFile(newRequestDTO, request);

            await unitOfWork.SaveChangesAsync(cancellation);

            Request? registeredRequest = await repository.ListRequestAsync(newRequest.UUID, tenantId.Value, cancellation);

            if (registeredRequest is null)
            {
                return Result.Failure(ErrorType.Validation, "Falha ao registrar solicitação.");
            }

            await notificationService.NotifyUsers(new NotificationRequest(request.UUID, [.. request.ManagersId, request.RequesterId]), cancellation);
            await notificationRepository.SaveNotifyAsync(new NotificationRequest(request.UUID, [.. request.ManagersId], "Você foi mencionado em uma solicitação !"));

            return Result.Success();
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure(ErrorType.InternalError, "Ocorreu um erro ao registrar a solicitação.");
        }
    }

    public async Task<Result> ApproveRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result.Failure(ErrorType.NotFound, "TenantId não encontrado.");

            Boolean result = await repository.ApproveRequestAsync(requestGuid, applicationUserId, cancellation);

            await unitOfWork.SaveChangesAsync(cancellation);

            Request? request = await repository.ListRequestAsync(requestGuid, tenantId.Value, cancellation);

            if (request is null) return Result.Failure(ErrorType.NotFound, "Não foi possível buscar a solicitação.");

            // Has directors?
            if (request.Directors.Count > 0)
            {
                // Notify everyone
                await notificationService.NotifyUsers(new NotificationRequest(request.UUID, [.. request.Managers.Select(x => x.ManagerId), .. request.Directors.Select(x => x.DirectorId), request.RequesterId]), cancellation);

                if (request.Status == StatusRequest.Pending)
                {
                    await notificationRepository.SaveNotifyAsync(new NotificationRequest(request.UUID, [.. request.Directors.Select(x => x.DirectorId)], "Você foi mencionado em uma solicitação !"));
                }
            }
            else
            {
                // Notify requester & manager
                await notificationService.NotifyUsers(new NotificationRequest(request.UUID, [.. request.Managers.Select(x => x.ManagerId), request.RequesterId]), cancellation);
            }

            if (request.Status == StatusRequest.Approved && request.ApprovedFirstLevel && request.ApprovedSecondLevel)
            {
                await notificationService.NotifyGroup(new NotificationGroupRequest(request.UUID, "role-finance"), cancellation);
                await notificationRepository.SaveNotifyAsync(new NotificationRequest(request.UUID, request.RequesterId, "Sua solicitação foi aprovada !"));
            }

            return result ? Result.Success() : Result.Failure(ErrorType.InternalError, "Não foi possível aprovar a solicitação.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure(ErrorType.InternalError, "Ocorreu um erro ao aprovar a solicitação.");
        }
    }

    public async Task<Result> RejectRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result.Failure(ErrorType.NotFound, "TenantId não encontrado.");

            Boolean result = await repository.RejectRequestAsync(requestGuid, applicationUserId, cancellation);

            await unitOfWork.SaveChangesAsync(cancellation);

            Request? request = await repository.ListRequestAsync(requestGuid, tenantId.Value, cancellation);

            if (request is null) return Result.Failure(ErrorType.NotFound, "Não foi possível buscar a solicitação.");

            if (request.Status == StatusRequest.Reject)
            {
                // Was rejected.
                await notificationService.NotifyUsers(new NotificationRequest(request.UUID, [.. request.Managers.Select(x => x.ManagerId), .. request.Directors.Select(x => x.DirectorId), request.RequesterId]), cancellation);
                await notificationRepository.SaveNotifyAsync(new NotificationRequest(request.UUID, request.RequesterId, "Sua solicitação foi rejeitada."));
            }

            return result ? Result.Success() : Result.Failure(ErrorType.InternalError, "Não foi possível rejeitar a solicitação.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure(ErrorType.InternalError, "Não foi possível rejeitar a solicitação.");
        }
    }

    public async Task<Result> FinishRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result.Failure(ErrorType.NotFound, "TenantId não encontrado.");

            Boolean result = await repository.FinishRequestAsync(requestGuid, applicationUserId, cancellation);

            await unitOfWork.SaveChangesAsync(cancellation);

            Request? request = await repository.ListRequestAsync(requestGuid, tenantId.Value, cancellation);

            if (request is null) return Result.Failure(ErrorType.NotFound, "Não foi possível buscar a solicitação.");

            await notificationService.NotifyUsers(new NotificationRequest(request.UUID, [request.RequesterId, request.FinisherId!.Value, applicationUserId]), cancellation);
            await notificationRepository.SaveNotifyAsync(new NotificationRequest(request.UUID, request.RequesterId, "Sua solicitação foi faturada !"));

            return result ? Result.Success() : Result.Failure(ErrorType.NotFound, "Não foi possível finalizar a solicitação.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure(ErrorType.InternalError, "Não foi possível finalizar a solicitação.");
        }
    }

    public async Task<Result<RequestDTO[]>> ListUserRequests(FilterRequest filter, Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            filter.ApplicationUserId = applicationUserId;

            Request[] requests = await repository.ListRequestsAsync(filter, tenantId.Value, cancellation);

            if (requests is null)
            {
                return Result<RequestDTO[]>.Failure(ErrorType.NotFound, "Nenhuma solicitação encontrada.");
            }

            RequestDTO[] response = requests.Select<Request, RequestDTO>(x => x).ToArray();

            return Result<RequestDTO[]>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar solicitações.");
        }
    }

    public async Task<Result<Byte[]>> LoadFileRequest(String type, Guid requestGuid, Guid fileGuid, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<Byte[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Request? request = await repository.ListRequestAsync(requestGuid, tenantId.Value, cancellation);

            if (request is null)
                return Result<Byte[]>.Failure(ErrorType.NotFound, "Nenhum arquivo encontrado.");

            if (type == "invoice" && fileGuid == request.InvoiceName)
            {
                String requestFilePath = Path.Combine(serverDirectory.InvoicePath, request.UUID.ToString("N"), request.InvoiceName.ToString("N") + ".pdf");

                Byte[] fileStream = await DownloadFile(requestFilePath);
                if (fileStream.Length > 0)
                {
                    return Result<Byte[]>.Success(fileStream);
                }
                else
                {
                    return Result<Byte[]>.Failure(ErrorType.NotFound, "Nenhum arquivo encontrado.");
                }
            }

            if (type == "budget" && fileGuid == request.BudgetName)
            {
                String requestFilePath = Path.Combine(serverDirectory.BudgetPath, request.UUID.ToString("N"), request.BudgetName.ToString("N") + ".pdf");

                Byte[] fileStream = await DownloadFile(requestFilePath);

                if (fileStream.Length > 0)
                {
                    return Result<Byte[]>.Success(fileStream);
                }
                else
                {
                    return Result<Byte[]>.Failure(ErrorType.NotFound, "Nenhum arquivo encontrado.");
                }
            }

            return Result<Byte[]>.Failure(ErrorType.NotFound, "Nenhum arquivo encontrado.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<Byte[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar arquivo.");
        }
    }

    public async Task<Result<RequestDTO[]>> ListAll(CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Request[] requests = await repository.ListAllAsync(tenantId.Value, cancellation);

            if (requests is null)
            {
                return Result<RequestDTO[]>.Failure(ErrorType.NotFound, "Nenhuma solicitação encontrada.");
            }

            return Result<RequestDTO[]>.Success(requests.Select<Request, RequestDTO>(req => req).ToArray());

        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao listar solicitações.");
        }
    }

    public async Task<Result<Object>> UserStats(Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<Object>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Object response = await repository.UserStatsAsync(applicationUserId, cancellation);

            if (response is null)
            {
                return Result<Object>.Failure(ErrorType.NotFound, "Nenhuma informação do usuário foi encontrada.");
            }

            return Result<Object>.Success(response);

        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<Object>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar informações do usuário.");
        }
    }

    public async Task<Result<Object>> TenantStats(CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<Object>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Object response = await repository.TenantStatsAsync(tenantId.Value, cancellation);

            if (response is null)
            {
                return Result<Object>.Failure(ErrorType.NotFound, "Nenhuma informação da empresa foi encontrada.");
            }

            return Result<Object>.Success(response);

        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<Object>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar informações do usuário.");
        }
    }

    public async Task<Result<RequestDTO>> ListRequest(Guid requestGuid, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<RequestDTO>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Request? request = await repository.ListRequestAsync(requestGuid, tenantId.Value, cancellation: cancellation);

            if (request is null)
            {
                return Result<RequestDTO>.Failure(ErrorType.NotFound, "Solicitação não encontrada.");
            }

            return Result<RequestDTO>.Success(request);

        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar a solicitação.");
        }
    }

    public async Task<Result<RequestDTO[]>> ListPendingRequests(FilterRequest filter, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            if (filter.UserRole == Roles.Manager)
            {
                filter.Levels = [LevelRequest.Pending];
            }
            else if (filter.UserRole == Roles.Director)
            {
                filter.Levels = [LevelRequest.FirstLevel];
            }
            else
            {
                return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "Usuário não possui permissão para essa consulta.");
            }

            Request[]? requests = await repository.ListPendingRequestsAsync(filter, tenantId.Value, cancellation);

            if (requests is null)
            {
                return Result<RequestDTO[]>.Failure(ErrorType.NotFound, "Nenhuma solicitação pendente encontrada.");
            }

            RequestDTO[] response = requests.Select<Request, RequestDTO>(x => x).ToArray();

            return Result<RequestDTO[]>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar solicitações pendentes.");
        }
    }

    public async Task<Result<RequestDTO[]>> ListApprovedRequests(FilterRequest filter, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            if (filter.UserRole != Roles.Finance) return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "Usuário não possui permissão para essa consulta.");

            Request[] requests = await repository.ListApprovedRequestsAsync(filter, tenantId.Value, cancellation);

            if (requests is null) return Result<RequestDTO[]>.Failure(ErrorType.NotFound, "Nenhuma solicitação aprovada encontrada.");

            RequestDTO[] response = requests.Select<Request, RequestDTO>(x => x).ToArray();

            return Result<RequestDTO[]>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar solicitações aprovadas.");
        }
    }

    public async Task<Result<RequestDTO[]>> ListFinishedRequests(FilterRequest filter, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Request[] requests = await repository.ListFinishedRequestsAsync(filter, tenantId.Value, cancellation);

            if (requests is null) return Result<RequestDTO[]>.Failure(ErrorType.NotFound, "Nenhuma solicitação finalizada encontrada.");

            RequestDTO[] response = requests.Select<Request, RequestDTO>(x => x).ToArray();

            return Result<RequestDTO[]>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar solicitações finalizadas.");
        }
    }

    public async Task<Result<RequestDTO[]>> ListTenantRequests(FilterRequest filter, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();

            if (!tenantId.HasValue) return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");

            Request[] requests = await repository.ListRequestsAsync(filter, tenantId.Value, cancellation);

            if (requests is null)
            {
                return Result<RequestDTO[]>.Failure(ErrorType.NotFound, "Nenhuma solicitação encontrada.");
            }

            RequestDTO[] response = requests.Select<Request, RequestDTO>(x => x).ToArray();

            return Result<RequestDTO[]>.Success(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO[]>.Failure(ErrorType.InternalError, "Ocorreu um erro ao buscar solicitações.");
        }
    }

    private async Task<Byte[]> DownloadFile(String objectPath)
    {
        HttpClient _httpClient = httpClientFactory.CreateClient("Supabase");

        String fullpath = objectPath.Replace("//", "/");
        HttpResponseMessage response = await _httpClient.GetAsync($"object/{fullpath}", CancellationToken.None);

        if (!response.IsSuccessStatusCode)
        {
            String error = await response.Content.ReadAsStringAsync(CancellationToken.None);
            throw new Exception($"Erro ao baixar arquivo: {response.StatusCode} - {error}");
        }

        return await response.Content.ReadAsByteArrayAsync(CancellationToken.None);
    }

    private async Task<Result> UploadFile(RequestDTO request, RequestRegisterDTO register)
    {
        async Task<Result> UploadToSupabaseAsync(String bucketName, String objectPath, Byte[] fileBytes)
        {
            try
            {
                HttpClient _httpClient = httpClientFactory.CreateClient("Supabase");

                using ByteArrayContent content = new ByteArrayContent(fileBytes);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

                String fullPath = objectPath.Replace("//", "/");
                HttpResponseMessage response = await _httpClient.PostAsync($"object/{fullPath}", content, CancellationToken.None);

                if (!response.IsSuccessStatusCode)
                {
                    String body = await response.Content.ReadAsStringAsync(CancellationToken.None);
                    return Result.Failure(ErrorType.InternalError, $"Falha ao salvar {bucketName}.");
                }

                return Result.Success();

            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return Result.Failure(ErrorType.InternalError, ex.Message);
            }
        }

        Result resultInvoice = Result.Success();
        Result resultBudget = Result.Success();

        if (register.Invoice?.Length > 0)
        {
            try
            {
                String folderName = request.UUID;
                String pathForFile = Path.Combine(serverDirectory.InvoicePath, folderName);
                String filePath = Path.Combine(pathForFile, request.InvoiceName) + ".pdf";

                resultInvoice = await UploadToSupabaseAsync(
                    bucketName: serverDirectory.InvoicePath,
                    objectPath: filePath,
                    fileBytes: register.Invoice);
            }
            catch (Exception ex)
            {
                resultInvoice = Result.Failure(ErrorType.InternalError, ex.Message);
            }
        }

        if (register.Budget?.Length > 0)
        {
            try
            {
                String folderName = request.UUID;
                String pathForFile = Path.Combine(serverDirectory.BudgetPath, folderName);
                String filePath = Path.Combine(pathForFile, request.BudgetName) + ".pdf";

                resultBudget = await UploadToSupabaseAsync(
                    bucketName: serverDirectory.BudgetPath,
                    objectPath: filePath,
                    fileBytes: register.Budget);
            }
            catch (Exception ex)
            {
                resultInvoice = Result.Failure(ErrorType.InternalError, ex.Message);
            }
        }

        if (resultInvoice.IsFailure || resultBudget.IsFailure)
        {
            return Result.Failure(ErrorType.InternalError, "Falha ao salvar arquivos.");
        }

        return Result.Success();
    }

    private static Boolean ShouldResetRequests(DateTime lastResetDate)
    {
        DateTime currentDate = DateTime.UtcNow;

        return lastResetDate.Year != currentDate.Year || lastResetDate.Month != currentDate.Month;
    }
}

