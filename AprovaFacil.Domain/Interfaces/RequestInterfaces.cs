using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class RequestInterfaces
{
    public interface IRequestService
    {
        Task<RequestDTO[]> ListRequests(FilterRequest filter, String ApplicationUserId, CancellationToken cancellation);
        Task<RequestDTO> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation);
    }

    public interface IRequestRepository
    {
        Task<Request[]> ListRequestsAsync(FilterRequest filter, Int32 ApplicationUserId, CancellationToken cancellation);
        Task<Request?> RegisterRequestAsync(Request request, CancellationToken cancellation);
    }
}