using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;
using Serilog;

namespace AprovaFacil.Application.Services;

public class RequestService(RequestInterfaces.IRequestRepository repository, UserInterfaces.IUserRepository userRepository, ServerDirectory serverDirectory, NotificationInterfaces.INotificationService notification) : RequestInterfaces.IRequestService
{
    public async Task<Result> ApproveRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Boolean result = await repository.ApproveRequestAsync(requestGuid, applicationUserId, cancellation);

            Request? request = await repository.ListRequestAsync(requestGuid, cancellation);

            if (request is null) return Result.Failure("Não foi possível buscar a solicitação.");

            // Has directors?
            if (request.Directors.Count > 0)
            {
                // Notify everyone
                await notification.NotifyUsers(new NotificationRequest(request.UUID, [.. request.Managers.Select(x => x.ManagerId), .. request.Directors.Select(x => x.DirectorId), request.RequesterId]), cancellation);
            }
            else
            {
                // Notify requester & manager
                await notification.NotifyUsers(new NotificationRequest(request.UUID, [.. request.Managers.Select(x => x.ManagerId), request.RequesterId]), cancellation);
            }

            if (request.Status == 1 && request.ApprovedFirstLevel && request.ApprovedSecondLevel)
            {
                await notification.NotifyGroup(new NotificationGroupRequest(request.UUID, "role-finance"), cancellation);
            }

            return result ? Result.Success() : Result.Failure("Não foi possível aprovar a solicitação.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure("Ocorreu um erro ao aprovar a solicitação.");
        }
    }

    public async Task<Result> RejectRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Boolean result = await repository.RejectRequestAsync(requestGuid, applicationUserId, cancellation);

            Request? request = await repository.ListRequestAsync(requestGuid, cancellation);

            if (request is null) return Result.Failure("Não foi possível buscar a solicitação.");

            // Was rejected.
            await notification.NotifyUsers(new NotificationRequest(request.UUID, [.. request.Managers.Select(x => x.ManagerId), .. request.Directors.Select(x => x.DirectorId), request.RequesterId]), cancellation);

            return result ? Result.Success() : Result.Failure("Não foi possível rejeitar a solicitação.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure("Não foi possível rejeitar a solicitação.");
        }
    }

    public async Task<Result> FinishRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Boolean result = await repository.FinishRequestAsync(requestGuid, applicationUserId, cancellation);

            Request? request = await repository.ListRequestAsync(requestGuid, cancellation);

            if (request is null) return Result.Failure("Não foi possível buscar a solicitação.");

            await notification.NotifyUsers(new NotificationRequest(request.UUID, request.RequesterId), cancellation);

            return result ? Result.Success() : Result.Failure("Não foi possível finalizar a solicitação.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result.Failure("Não foi possível finalizar a solicitação.");
        }
    }

    public async Task<Result<RequestDTO[]>> ListRequests(FilterRequest filter, Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            filter.ApplicationUserId = applicationUserId;

            Request[] requests = await repository.ListRequestsAsync(filter, cancellation);

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
            Request? request = await repository.ListRequestAsync(requestGuid, cancellation);

            if (request is null)
                return Result<Byte[]>.Failure(ErrorType.NotFound, "Nenhum arquivo encontrado.");

            if (type == "invoice" && fileGuid == request.InvoiceName)
            {
                String requestFilePath = Path.Combine(serverDirectory.InvoicePath, request.UUID.ToString("N"), request.InvoiceName.ToString("N") + ".pdf");

                if (File.Exists(requestFilePath))
                {
                    Byte[] fileStream = await LoadFile(requestFilePath, cancellation);
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

                if (File.Exists(requestFilePath))
                {
                    Byte[] fileStream = await LoadFile(requestFilePath, cancellation);
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

    public async Task<Result<RequestDTO>> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation)
    {
        try
        {
            Int32[] usersId = [.. request.ManagersId, .. request.DirectorsIds, request.RequesterId];

            Dictionary<Int32, IApplicationUser> users = await userRepository.GetUsersDictionary(usersId, cancellation);

            Int32 level = request.DirectorsIds.Length > 0 ? LevelRequest.Pending : LevelRequest.Pending;

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
                Level = level,
            };

            foreach (Int32 manager in request.ManagersId)
            {
                RequestManager requestManager = new RequestManager
                {
                    ManagerId = manager,
                    RequestUUID = newRequest.UUID,
                };

                newRequest.Managers.Add(requestManager);
            }

            foreach (Int32 director in request.DirectorsIds)
            {
                RequestDirector requestDirector = new RequestDirector
                {
                    DirectorId = director,
                    RequestUUID = newRequest.UUID,
                };

                newRequest.Directors.Add(requestDirector);
            }

            newRequest.HasInvoice = request.Invoice.Length > 0;
            newRequest.HasBudget = request.Budget.Length > 0;

            newRequest = await repository.RegisterRequestAsync(newRequest, cancellation);

            if (newRequest is null) return Result<RequestDTO>.Failure(ErrorType.Validation, "Falha ao registrar solicitação.");

            RequestDTO? newRequestDTO = newRequest;

            if (newRequestDTO is null) return Result<RequestDTO>.Failure(ErrorType.Validation, "Falha ao validar solicitação.");

            try
            {
                String folderName = newRequestDTO.UUID.ToString("N");
                String pathForFile = Path.Combine(serverDirectory.InvoicePath, folderName);

                if (!Directory.Exists(pathForFile))
                {
                    Directory.CreateDirectory(pathForFile);
                }

                String filePath = Path.Combine(pathForFile, newRequestDTO.InvoiceName) + ".pdf";

                if (request.Invoice.Length > 0)
                {
                    await File.WriteAllBytesAsync(filePath, request.Invoice, cancellation);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception raise when try save invoice file.");
            }

            try
            {
                String folderName = newRequestDTO.UUID.ToString("N");
                String pathForFile = Path.Combine(serverDirectory.BudgetPath, folderName);

                if (!Directory.Exists(pathForFile))
                {
                    Directory.CreateDirectory(pathForFile);
                }

                String filePath = Path.Combine(pathForFile, newRequestDTO.BudgetName) + ".pdf";

                if (request.Budget.Length > 0)
                {
                    await File.WriteAllBytesAsync(filePath, request.Budget, cancellation);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception raise when try save budget file.");
            }

            await notification.NotifyUsers(new NotificationRequest(request.UUID, [.. request.ManagersId, request.RequesterId]), cancellation);

            return Result<RequestDTO>.Success(newRequestDTO);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao registrar a solicitação.");
        }
    }

    public async Task<Result<RequestDTO[]>> ListAll(CancellationToken cancellation)
    {
        try
        {
            Request[] requests = await repository.ListAllAsync(cancellation);

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

    public async Task<Result<Object>> MyStats(Int32 applicationUserId, CancellationToken cancellation)
    {
        try
        {
            Object response = await repository.MyStatsAsync(applicationUserId, cancellation);

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

    public async Task<Result<RequestDTO>> ListRequest(Guid requestGuid, CancellationToken cancellation)
    {
        try
        {
            Request? request = await repository.ListRequestAsync(requestGuid, cancellation: cancellation);

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

    private async Task<Byte[]> LoadFile(String path, CancellationToken cancellation)
    {
        if (File.Exists(path))
        {
            return await File.ReadAllBytesAsync(path, cancellation);
        }
        return Array.Empty<Byte>();
    }

    public async Task<Result<RequestDTO[]>> ListPendingRequests(FilterRequest filter, CancellationToken cancellation)
    {
        try
        {
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

            Request[]? requests = await repository.ListPendingRequestsAsync(filter, cancellation);

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
            if (filter.UserRole != Roles.Finance) return Result<RequestDTO[]>.Failure(ErrorType.Unathorized, "Usuário não possui permissão para essa consulta.");

            Request[] requests = await repository.ListApprovedRequestsAsync(filter, cancellation);

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
            Request[] requests = await repository.ListFinishedRequestsAsync(filter, cancellation);

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
}

