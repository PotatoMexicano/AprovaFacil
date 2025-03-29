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
    ServerDirectory serverDirectory
) : RequestInterfaces.IRequestService
{
    public async Task<RequestDTO[]> ListRequests(FilterRequest request, String strApplicationUserId, CancellationToken cancellation = default)
    {

        if (Int32.TryParse(strApplicationUserId, out Int32 applicationUserId))
        {
            Request[] requests = await repository.ListRequestsAsync(request, applicationUserId, cancellation);
            RequestDTO[] response = [.. requests.Select(RequestExtensions.ToDTO)];
            return response;
        }
        else
        {
            return Array.Empty<RequestDTO>();
        }
    }

    public async Task<RequestDTO> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation)
    {
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
            String folderName = newRequestDTO.UUID.ToString("N");
            String pathForFile = Path.Combine(serverDirectory.InvoicePath, folderName);

            if (!Directory.Exists(pathForFile))
            {
                Directory.CreateDirectory(pathForFile);
            }

            String filePath = Path.Combine(pathForFile, newRequestDTO.InvoiceName.ToString("N")) + ".pdf";

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

            String filePath = Path.Combine(pathForFile, newRequestDTO.BudgetName.ToString("N")) + ".pdf";

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
}
