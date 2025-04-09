using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class RequestInterfaces
{
    public interface IRequestService
    {
        Task<RequestDTO[]> ListAll(CancellationToken cancellation);
        Task<RequestDTO[]> ListPendingRequests(FilterRequest filter, CancellationToken cancellation);
        Task<RequestDTO[]> ListRequests(FilterRequest filter, String strApplicationUserId, CancellationToken cancellation);
        Task<RequestDTO?> ListRequest(Guid requestGuid, CancellationToken cancellation);
        Task<Object> MyStats(String strApplicationUserId, CancellationToken cancellation);

        Task<RequestDTO> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation);
        Task<Byte[]> LoadFileRequest(String type, Guid requestGuid, Guid fileGuid, CancellationToken cancellation);

        Task ApproveRequest(Guid requestGuid, String strApplicationUserId, CancellationToken cancellation);
        Task RejectRequest(Guid requestGuid, String strApplicationUserId, CancellationToken cancellation);
    }

    public interface IRequestRepository
    {
        Task<Request[]> ListAllAsync(CancellationToken cancellation);
        Task<Request?> ListRequestAsync(Guid request, CancellationToken cancellation);
        Task<Request[]> ListRequestsAsync(FilterRequest filter, CancellationToken cancellation, Int32? ApplicationUserId = default);
        Task<Request[]> ListPendingRequestsAsync(FilterRequest filter, CancellationToken cancellation, Int32? ApplicationUserId = default);
        Task<Object> MyStatsAsync(Int32 applicationUserId, CancellationToken cancellation);

        Task<Request?> RegisterRequestAsync(Request request, CancellationToken cancellation);

        Task<Boolean> ApproveRequestAsync(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation);
        Task<Boolean> RejectRequestAsync(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation);
    }
}