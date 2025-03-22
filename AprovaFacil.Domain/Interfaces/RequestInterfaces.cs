using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Interfaces;

public static class RequestInterfaces
{
    public interface IRequestService
    {
        Task<RequestDTO> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation);
    }

    public interface IRequestRepository
    {
        Task<Request?> RegisterRequestAsync(Request request, CancellationToken cancellation);
    }
}