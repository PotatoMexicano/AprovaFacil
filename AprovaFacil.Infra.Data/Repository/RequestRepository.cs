using AprovaFacil.Domain.Extensions;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Interfaces;
using AprovaFacil.Domain.Models;
using AprovaFacil.Infra.Data.Context;
using AprovaFacil.Infra.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace AprovaFacil.Infra.Data.Repository;

public class RequestRepository(ApplicationDbContext context) : RequestInterfaces.IRequestRepository
{
    public async Task<Request[]> ListRequestsAsync(FilterRequest filter, Int32 applicationUserId, CancellationToken cancellation)
    {
        IQueryable<Request> query = context.Requests.AsQueryable();

        RequestExtensions.Filter(ref query, filter, applicationUserId);

        Request[] results = await query.Select(x => new Request
        {
            UUID = x.UUID,
            RequesterId = x.RequesterId,
            CreateAt = x.CreateAt,
            Amount = x.Amount,
            ApprovedFirstLevelAt = x.ApprovedFirstLevelAt,
            ApprovedSecondLevelAt = x.ApprovedSecondLevelAt,
            InvoiceName = x.InvoiceName,
            BudgetName = x.BudgetName,
            Note = x.Note,
            PaymentDate = x.PaymentDate,
            ReceivedAt = x.ReceivedAt,
            Requester = new ApplicationUser
            {
                Email = x.Requester.Email,
                Department = x.Requester.Department,
                Role = x.Requester.Role,
                FullName = x.Requester.FullName,
                Id = x.Requester.Id,
                Enabled = x.Requester.Enabled,
                UserName = x.Requester.UserName,
                PictureUrl = x.Requester.PictureUrl,
            },
            Managers = x.Managers.Select(m => new RequestManager
            {
                ManagerId = m.ManagerId,
                RequestUUID = m.RequestUUID,
                User = new ApplicationUser
                {
                    Id = m.User.Id,
                    FullName = m.User.FullName,
                    UserName = m.User.UserName,
                    Email = m.User.Email,
                    Department = m.User.Department,
                    Role = m.User.Role,
                    PictureUrl = m.User.PictureUrl,
                    Enabled = m.User.Enabled,
                }
            }).ToList(),
            Directors = x.Directors.Select(d => new RequestDirector
            {
                DirectorId = d.DirectorId,
                RequestUUID = d.RequestUUID,
                User = new ApplicationUser
                {
                    Id = d.User.Id,
                    FullName = d.User.FullName,
                    UserName = d.User.UserName,
                    Email = d.User.Email,
                    Department = d.User.Department,
                    Role = d.User.Role,
                    PictureUrl = d.User.PictureUrl,
                    Enabled = d.User.Enabled,
                }
            }).ToList(),
        }).AsNoTracking()
        .ToArrayAsync(cancellation);

        return results;
    }

    public async Task<Request?> RegisterRequestAsync(Request request, CancellationToken cancellation)
    {
        if (request.UUID == Guid.Empty)
        {
            request.UUID = Guid.NewGuid();
        }

        if (request.BudgetName == Guid.Empty)
        {
            request.BudgetName = Guid.NewGuid();
        }

        if (request.InvoiceName == Guid.Empty)
        {
            request.InvoiceName = Guid.NewGuid();
        }

        if (request.CreateAt == default)
        {
            request.CreateAt = DateTime.UtcNow;
        }

        context.Requests.Add(request);
        await context.SaveChangesAsync(cancellation);

        // Recarregar a entidade com as relações
        return await context.Requests
            .Select(x => new Request
            {
                UUID = x.UUID,
                CreateAt = x.CreateAt,
                Amount = x.Amount,
                Note = x.Note,
                InvoiceName = x.InvoiceName,
                BudgetName = x.BudgetName,
                PaymentDate = x.PaymentDate,
                Requester = new ApplicationUser
                {
                    FullName = x.Requester.FullName,
                    Email = x.Requester.Email,
                    Id = x.RequesterId,
                    Enabled = x.Requester.Enabled,
                    Department = x.Requester.Department,
                    Role = x.Requester.Role,
                    PictureUrl = x.Requester.PictureUrl
                },
                Directors = x.Directors.Select(d => new RequestDirector
                {
                    DirectorId = d.DirectorId,
                    RequestUUID = d.RequestUUID,
                    User = new ApplicationUser
                    {
                        FullName = d.User.FullName,
                        Email = d.User.Email,
                        Id = d.User.Id,
                        Enabled = d.User.Enabled,
                        Department = d.User.Department,
                        Role = d.User.Role,
                        PictureUrl = d.User.PictureUrl
                    },
                }).ToList(),
                Managers = x.Managers.Select(m => new RequestManager
                {
                    ManagerId = m.ManagerId,
                    RequestUUID = m.RequestUUID,
                    User = new ApplicationUser
                    {
                        FullName = m.User.FullName,
                        Email = m.User.Email,
                        Id = m.User.Id,
                        Enabled = m.User.Enabled,
                        Department = m.User.Department,
                        Role = m.User.Role,
                        PictureUrl = m.User.PictureUrl
                    },
                }).ToList(),
            })
            .FirstOrDefaultAsync(r => r.UUID == request.UUID, cancellation);
    }
}
