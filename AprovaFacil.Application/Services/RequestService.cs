using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Extensions;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using Serilog;

namespace AprovaFacil.Application.Services;

public class RequestService(
    RequestInterfaces.IRequestRepository repository,
    UserInterfaces.IUserRepository userRepository,
    ServerDirectory serverDirectory) : RequestInterfaces.IRequestService
{
    public async Task ApproveRequest(Guid requestGuid, String strApplicationUserId, CancellationToken cancellation)
    {
        Boolean result = await repository.ApproveRequestAsync(requestGuid, Int32.Parse(strApplicationUserId), cancellation);
    }

    public async Task RejectRequest(Guid requestGuid, String strApplicationUserId, CancellationToken cancellation)
    {
        Boolean result = await repository.RejectRequestAsync(requestGuid, Int32.Parse(strApplicationUserId), cancellation);
    }

    public async Task<RequestDTO[]> ListPendingRequests(FilterRequest filter, CancellationToken cancellation)
    {
        if (filter.UserRole == Roles.Manager)
        {
            filter.Levels = [LevelRequest.Pending];
        }
        else
        {
            filter.Levels = [LevelRequest.FirstLevel];
        }

        Request[] requests = await repository.ListPendingRequestsAsync(filter, cancellation: cancellation);
        RequestDTO[] response = [.. requests.Select(RequestExtensions.ToDTO)];
        return response;
    }

    public async Task<RequestDTO?> ListRequest(Guid requestGuid, CancellationToken cancellation)
    {
        Request? request = await repository.ListRequestAsync(requestGuid, cancellation: cancellation);
        return request?.ToDTO();
    }

    public async Task<RequestDTO[]> ListRequests(FilterRequest filter, String strApplicationUserId, CancellationToken cancellation)
    {
        if (Int32.TryParse(strApplicationUserId, out Int32 applicationUserId))
        {
            Request[] requests = await repository.ListRequestsAsync(filter, cancellation, applicationUserId);
            RequestDTO[] response = [.. requests.Select(RequestExtensions.ToDTO)];
            return response;
        }
        else
        {
            return Array.Empty<RequestDTO>();
        }
    }

    public async Task<Byte[]> LoadFileRequest(String type, Guid requestGuid, Guid fileGuid, CancellationToken cancellation)
    {
        Request? request = await repository.ListRequestAsync(requestGuid, cancellation);

        if (request is null)
            return Array.Empty<Byte>();

        if (type == "invoice" && fileGuid == request.InvoiceName)
        {
            String requestFilePath = Path.Combine(serverDirectory.InvoicePath, request.UUID.ToString("N"), request.InvoiceName.ToString("N") + ".pdf");

            if (File.Exists(requestFilePath))
            {
                Byte[] fileStream = await LoadFile(requestFilePath, cancellation);
                return fileStream;
            }
            else
            {
                return Array.Empty<Byte>();
            }
        }

        if (type == "budget" && fileGuid == request.BudgetName)
        {
            String requestFilePath = Path.Combine(serverDirectory.BudgetPath, request.UUID.ToString("N"), request.BudgetName.ToString("N") + ".pdf");

            if (File.Exists(requestFilePath))
            {
                Byte[] fileStream = await LoadFile(requestFilePath, cancellation);
                return fileStream;
            }
            else
            {
                return Array.Empty<Byte>();
            }
        }

        return Array.Empty<Byte>();
    }

    public async Task<RequestDTO> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation)
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

        if (newRequest is null)
        {
            return new RequestDTO
            {

            };
        }

        RequestDTO newRequestDTO = newRequest.ToDTO();

        try
        {
            String folderName = newRequestDTO.UUID;
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
            String folderName = newRequestDTO.UUID;
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

        return newRequestDTO;
    }

    private async Task<Byte[]> LoadFile(String path, CancellationToken cancellation)
    {
        if (File.Exists(path))
        {
            return await File.ReadAllBytesAsync(path, cancellation);
        }
        return Array.Empty<Byte>();
    }

    public async Task<RequestDTO[]> ListAll(CancellationToken cancellation)
    {
        Request[] requests = await repository.ListAllAsync(cancellation);
        return [.. requests.Select(x => x.ToDTO())];
    }

    public async Task<Object> MyStats(String strApplicationUserId, CancellationToken cancellation)
    {
        if (Int32.TryParse(strApplicationUserId, out Int32 applicationUserId))
        {
            Object response = await repository.MyStatsAsync(applicationUserId, cancellation);
            return response;
        }
        else
        {
            return new { };
        }
    }
}
