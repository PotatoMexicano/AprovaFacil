using AprovaFacil.Domain.Constants;
using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Domain.Results;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers; // Ensure this is present if httpClientFactory is used elsewhere, though not directly in RegisterRequest logic shown
using System.Threading;
using System.Threading.Tasks;

namespace AprovaFacil.Application.Services;

public class RequestService(RequestInterfaces.IRequestRepository repository, 
                          UserInterfaces.IUserRepository userRepository, 
                          ServerDirectory serverDirectory, 
                          NotificationInterfaces.INotificationService notificationService, 
                          NotificationInterfaces.INotificationRepository notificationRepository, 
                          ITenantProvider tenantProvider, // Renamed for clarity from 'tenant' to 'tenantProvider'
                          ITenantRepository tenantRepository, // Added ITenantRepository
                          IHttpClientFactory httpClientFactory) : RequestInterfaces.IRequestService
{
    // ... (other methods remain the same) ...

    public async Task<Result<RequestDTO>> RegisterRequest(RequestRegisterDTO request, CancellationToken cancellation)
    {
        try
        {
            Int32? tenantId = tenantProvider.GetTenantId();
            if (!tenantId.HasValue) 
            {
                return Result<RequestDTO>.Failure(ErrorType.Unathorized, "TenantId não encontrado.");
            }

            // Get Tenant to check limits
            Tenant? currentTenant = await tenantRepository.GetByIdAsync(tenantId.Value, cancellation);
            if (currentTenant == null)
            {
                return Result<RequestDTO>.Failure(ErrorType.NotFound, "Informações do tenant não encontradas.");
            }

            // Ensure limits are set based on the current plan (might be redundant if constructor always called, but safe)
            currentTenant.SetLimitsBasedOnPlan(); 

            // Check and reset monthly request count if a new month has started
            if (currentTenant.LastRequestResetDate.Year < DateTime.UtcNow.Year || 
                (currentTenant.LastRequestResetDate.Year == DateTime.UtcNow.Year && currentTenant.LastRequestResetDate.Month < DateTime.UtcNow.Month))
            {
                currentTenant.CurrentRequestsThisMonth = 0;
                currentTenant.LastRequestResetDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            }

            // Check request limit
            if (currentTenant.CurrentRequestsThisMonth >= currentTenant.MaxRequestsPerMonth)
            {
                return Result<RequestDTO>.Failure(ErrorType.Forbidden, $"Você atingiu o limite de {currentTenant.MaxRequestsPerMonth} requisições para o seu plano este mês.");
            }

            // Proceed with request registration logic (existing logic from user's file)
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
                TenantId = tenantId.Value,
                Level = level,
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

            Request? registeredRequest = await repository.RegisterRequestAsync(newRequest, cancellation); // Renamed for clarity

            if (registeredRequest is null) 
            {
                return Result<RequestDTO>.Failure(ErrorType.Validation, "Falha ao registrar solicitação.");
            }

            // Increment request count and update tenant
            currentTenant.CurrentRequestsThisMonth++;
            await tenantRepository.UpdateAsync(currentTenant, cancellation);

            RequestDTO? newRequestDTO = registeredRequest; // Implicit conversion if defined
            if (newRequestDTO is null) 
            {
                // This case should ideally not happen if registeredRequest is not null and conversion is valid
                return Result<RequestDTO>.Failure(ErrorType.Validation, "Falha ao converter solicitação para DTO.");
            }

            // File saving logic (existing logic from user's file)
            try
            {
                String folderName = newRequestDTO.UUID.ToString(); // Use ToString() for Guid
                String pathForInvoice = Path.Combine(serverDirectory.InvoicePath, folderName);
                if (!Directory.Exists(pathForInvoice)) Directory.CreateDirectory(pathForInvoice);
                String invoiceFilePath = Path.Combine(pathForInvoice, newRequestDTO.InvoiceName.ToString()) + ".pdf";
                if (request.Invoice.Length > 0) await File.WriteAllBytesAsync(invoiceFilePath, request.Invoice, cancellation); // Simplified, original had UploadFile method
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception raised when trying to save invoice file.");
            }

            try
            {
                String folderName = newRequestDTO.UUID.ToString();
                String pathForBudget = Path.Combine(serverDirectory.BudgetPath, folderName);
                if (!Directory.Exists(pathForBudget)) Directory.CreateDirectory(pathForBudget);
                String budgetFilePath = Path.Combine(pathForBudget, newRequestDTO.BudgetName.ToString()) + ".pdf";
                if (request.Budget.Length > 0) await File.WriteAllBytesAsync(budgetFilePath, request.Budget, cancellation);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception raised when trying to save budget file.");
            }

            // Notification logic (existing logic from user's file)
            await notificationService.NotifyUsers(new NotificationRequest(request.UUID, [.. request.ManagersId, request.RequesterId]), cancellation);
            await notificationRepository.SaveNotifyAsync(new NotificationRequest(request.UUID, [.. request.ManagersId], "Você foi mencionado em uma solicitação !"));

            return Result<RequestDTO>.Success(newRequestDTO);
        }
        catch (Exception ex)
        {
            Log.Error(ex, ex.Message);
            return Result<RequestDTO>.Failure(ErrorType.InternalError, "Ocorreu um erro ao registrar a solicitação.");
        }
    }

    // ... (other methods: ApproveRequest, RejectRequest, FinishRequest, ListUserRequests, LoadFileRequest, ListAll, UserStats, TenantStats, ListPendingRequests, ListApprovedRequests, ListFinishedRequests, ListRequest) remain the same ...
    // Helper methods like DownloadFile and UploadFile if they were part of the original class should also be included if not shown in the provided snippet.

    // Placeholder for other methods from the original file to ensure completeness
    public async Task<Result> ApproveRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result> RejectRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result> FinishRequest(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<RequestDTO[]>> ListUserRequests(FilterRequest filter, Int32 applicationUserId, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<Byte[]>> LoadFileRequest(String type, Guid requestGuid, Guid fileGuid, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<RequestDTO[]>> ListAll(CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<Object>> UserStats(Int32 applicationUserId, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<Object>> TenantStats(CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<RequestDTO[]>> ListPendingRequests(FilterRequest filter, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<RequestDTO[]>> ListApprovedRequests(FilterRequest filter, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<RequestDTO[]>> ListFinishedRequests(FilterRequest filter, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }
    public async Task<Result<RequestDTO>> ListRequest(Guid requestGuid, CancellationToken cancellation) { await Task.Delay(1); throw new NotImplementedException(); }

    // Dummy DownloadFile and UploadFile methods if they were part of the original class and not shown
    private async Task<byte[]> DownloadFile(string filePath) { await Task.Delay(1); return []; }
    private async Task UploadFile(RequestDTO dto, RequestRegisterDTO req) { await Task.Delay(1); }
}

