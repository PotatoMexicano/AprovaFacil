using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class RequestInterfaces
{
    public interface IRequestService
    {
        Task<RequestDTO[]> ListPendingRequests(FilterRequest filter, CancellationToken cancellation);
        Task<RequestDTO[]> ListRequests(FilterRequest filter, String strApplicationUserId, CancellationToken cancellation);
        Task<RequestDTO> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation);
        Task<Byte[]> LoadFileRequest(String type, Guid requestGuid, Guid fileGuid, CancellationToken cancellation);
    }

    public interface IRequestRepository
    {
        Task<Request?> ListRequestAsync(Guid request, CancellationToken cancellation);
        Task<Request[]> ListRequestsAsync(FilterRequest filter, CancellationToken cancellation, Int32? ApplicationUserId = default);
        Task<Request[]> ListPendingRequestsAsync(FilterRequest filter, CancellationToken cancellation, Int32? ApplicationUserId = default);
        Task<Request?> RegisterRequestAsync(Request request, CancellationToken cancellation);
    }
}