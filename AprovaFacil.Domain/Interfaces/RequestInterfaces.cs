using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;

namespace AprovaFacil.Domain.Interfaces;

public static class RequestInterfaces
{
    public interface IRequestService
    {
        Task<Result<RequestDTO[]>> ListAll(CancellationToken cancellation);
        Task<Result<RequestDTO[]>> ListUserRequests(FilterRequest filter, Int32 applicationUserId, CancellationToken cancellation);
        Task<Result<RequestDTO[]>> ListTenantRequests(FilterRequest filter, CancellationToken cancellation);
        Task<Result<RequestDTO>> ListRequest(Guid requestGuid, CancellationToken cancellation);

        Task<Result<Object>> UserStats(Int32 applicationUserId, CancellationToken cancellation);
        Task<Result<Object>> TenantStats(CancellationToken cancellation);

        Task<Result<RequestDTO[]>> ListApprovedRequests(FilterRequest filter, CancellationToken cancellation);
        Task<Result<RequestDTO[]>> ListPendingRequests(FilterRequest filter, CancellationToken cancellation);
        Task<Result<RequestDTO[]>> ListFinishedRequests(FilterRequest filter, CancellationToken cancellation);

        Task<Result<RequestDTO>> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation);
        Task<Result<Byte[]>> LoadFileRequest(String type, Guid requestGuid, Guid fileGuid, CancellationToken cancellation);

        Task<Result> ApproveRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation);
        Task<Result> RejectRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation);
        Task<Result> FinishRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation);
    }

    public interface IRequestRepository
    {
        Task<Request[]> ListAllAsync(Int32 tenantId, CancellationToken cancellation);
        Task<Request?> ListRequestAsync(Guid request, Int32 tenantId, CancellationToken cancellation);
        Task<Request[]> ListRequestsAsync(FilterRequest filter, Int32 tenantId, CancellationToken cancellation);

        Task<Object> UserStatsAsync(Int32 applicationUserId, CancellationToken cancellation);
        Task<Object> TenantStatsAsync(Int32 tenantId, CancellationToken cancellation);

        Task<Request[]> ListPendingRequestsAsync(FilterRequest filter, Int32 tenantId, CancellationToken cancellation);
        Task<Request[]> ListApprovedRequestsAsync(FilterRequest filter, Int32 tenantId, CancellationToken cancellation);
        Task<Request[]> ListFinishedRequestsAsync(FilterRequest filter, Int32 tenantId, CancellationToken cancellation);

        Task<Request?> RegisterRequestAsync(Request request, CancellationToken cancellation);

        Task<Boolean> ApproveRequestAsync(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation);
        Task<Boolean> RejectRequestAsync(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation);
        Task<Boolean> FinishRequestAsync(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation);
    }
}