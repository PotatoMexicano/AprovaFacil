using AprovaFacil.Domain.Constants;
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
    public async Task<Boolean> ApproveRequestAsync(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        Request? request = await context.Requests
            .Include(x => x.Requester)
            .Include(x => x.Managers)
            .Include(x => x.Directors)
            .FirstOrDefaultAsync(x => x.UUID == requestGuid, cancellation);

        if (request is null) return false;

        Boolean updated = false;

        RequestManager? manager = request.Managers.FirstOrDefault(m => m.ManagerId == applicationUserId);
        if (manager is not null && manager.Approved == StatusRequest.Pending)
        {
            manager.Approved = StatusRequest.Approved;
            request.FirstLevelAt = DateTime.UtcNow;
            request.ApprovedFirstLevel = true;
            request.Level = LevelRequest.FirstLevel;

            if (request.Directors.Count == 0)
            {
                request.Status = StatusRequest.Approved;
                request.SecondLevelAt = DateTime.UtcNow;
                request.ApprovedSecondLevel = true;
                request.Level = LevelRequest.SecondLevel;
            }
            updated = true;
        }

        RequestDirector? director = request.Directors.FirstOrDefault(d => d.DirectorId == applicationUserId);
        if (director is not null && director.Approved == StatusRequest.Pending)
        {
            director.Approved = StatusRequest.Approved;
            request.SecondLevelAt = DateTime.UtcNow;
            request.ApprovedSecondLevel = true;
            request.Level = LevelRequest.SecondLevel;
            request.Status = StatusRequest.Approved;
            updated = true;
        }

        if (!updated) return false;

        await context.SaveChangesAsync(cancellation);
        return true;
    }

    public async Task<Boolean> RejectRequestAsync(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        Request? request = await context.Requests
            .Include(x => x.Requester)
            .Include(x => x.Managers)
            .Include(x => x.Directors)
            .FirstOrDefaultAsync(x => x.UUID == requestGuid, cancellation);

        if (request is null) return false;

        Boolean updated = false;

        // Se já está recusada, retorna
        if (request.Status == StatusRequest.Reject) return false;

        RequestManager? manager = request.Managers.FirstOrDefault(m => m.ManagerId == applicationUserId);
        if (manager is not null && manager.Approved == StatusRequest.Pending)
        {
            manager.Approved = StatusRequest.Reject;
            request.Status = StatusRequest.Reject;
            request.FirstLevelAt = DateTime.UtcNow;
            updated = true;
        }

        // Verifica se é director
        RequestDirector? director = request.Directors.FirstOrDefault(d => d.DirectorId == applicationUserId);
        if (director is not null && director.Approved == StatusRequest.Pending)
        {
            director.Approved = StatusRequest.Reject;
            updated = true;

            // Se TODOS os diretores recusaram, então recusa o request
            if (request.Directors.All(d => d.Approved == StatusRequest.Reject))
            {
                request.Status = StatusRequest.Reject;
                request.SecondLevelAt = DateTime.UtcNow;
            }
        }

        if (!updated) return false;

        await context.SaveChangesAsync(cancellation);
        return true;
    }

    public async Task<Boolean> FinishRequestAsync(Guid requestGuid, Int32 applicationUserId, CancellationToken cancellation)
    {
        Request? request = await context.Requests.FirstOrDefaultAsync(cancellation);

        if (request is null) return false;

        request.FinishedAt = DateTime.UtcNow;
        request.Level = LevelRequest.Finished;
        request.FinisherId = applicationUserId;

        await context.SaveChangesAsync(cancellation);

        return true;
    }

    public async Task<Request?> ListRequestAsync(Guid request, CancellationToken cancellation)
    {
        Request? result = await context.Requests
            .Where(x => x.UUID == request)
            .Select(x => new Request
            {
                UUID = x.UUID,
                RequesterId = x.RequesterId,
                CreateAt = x.CreateAt,
                Amount = x.Amount,
                FirstLevelAt = x.FirstLevelAt,
                SecondLevelAt = x.SecondLevelAt,
                InvoiceName = x.InvoiceName,
                HasBudget = x.HasBudget,
                HasInvoice = x.HasInvoice,
                Level = x.Level,
                Status = x.Status,
                BudgetName = x.BudgetName,
                Note = x.Note,
                PaymentDate = x.PaymentDate,
                FinishedAt = x.FinishedAt,
                ApprovedFirstLevel = x.ApprovedFirstLevel,
                ApprovedSecondLevel = x.ApprovedSecondLevel,
                CompanyId = x.CompanyId,
                Company = new Company
                {
                    Id = x.Company.Id,
                    LegalName = x.Company.LegalName,
                    TradeName = x.Company.TradeName,
                    Address = x.Company.Address,
                    CNPJ = x.Company.CNPJ,
                    Email = x.Company.Email,
                    Phone = x.Company.Phone,
                    Enabled = x.Company.Enabled
                },
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
                Finisher = ( x.Finisher != null ) ? new ApplicationUser
                {
                    Email = x.Finisher.Email,
                    Department = x.Finisher.Department,
                    Role = x.Finisher.Role,
                    FullName = x.Finisher.FullName,
                    Id = x.Finisher.Id,
                    Enabled = x.Finisher.Enabled,
                    UserName = x.Finisher.UserName,
                    PictureUrl = x.Finisher.PictureUrl,
                } : null,
                Managers = x.Managers.Select(m => new RequestManager
                {
                    ManagerId = m.ManagerId,
                    RequestUUID = m.RequestUUID,
                    Approved = m.Approved,
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
                    Approved = d.Approved,
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
        .FirstOrDefaultAsync(cancellation);

        return result;

    }

    public async Task<Request[]> ListRequestsAsync(FilterRequest filter, CancellationToken cancellation)
    {
        IQueryable<Request> query = context.Requests.AsQueryable();

        RequestExtensions.Filter(ref query, filter);
        RequestExtensions.FilterPersonal(ref query, filter);

        Request[] results = await query.Select(x => new Request
        {
            UUID = x.UUID,
            RequesterId = x.RequesterId,
            CreateAt = x.CreateAt,
            Amount = x.Amount,
            FirstLevelAt = x.FirstLevelAt,
            SecondLevelAt = x.SecondLevelAt,
            InvoiceName = x.InvoiceName,
            HasBudget = x.HasBudget,
            HasInvoice = x.HasInvoice,
            Level = x.Level,
            Status = x.Status,
            BudgetName = x.BudgetName,
            Note = x.Note,
            PaymentDate = x.PaymentDate,
            FinishedAt = x.FinishedAt,
            ApprovedFirstLevel = x.ApprovedFirstLevel,
            ApprovedSecondLevel = x.ApprovedSecondLevel,
            CompanyId = x.CompanyId,
            Company = new Company
            {
                Id = x.Company.Id,
                LegalName = x.Company.LegalName,
                TradeName = x.Company.TradeName,
                Address = x.Company.Address,
                CNPJ = x.Company.CNPJ,
                Email = x.Company.Email,
                Phone = x.Company.Phone,
                Enabled = x.Company.Enabled,
            },
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
                Approved = m.Approved,
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
                Approved = d.Approved,
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
        })
            .AsNoTracking()
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
                Company = new Company
                {
                    Address = x.Company.Address,
                    CNPJ = x.Company.CNPJ,
                    Email = x.Company.Email,
                    LegalName = x.Company.LegalName,
                    Phone = x.Company.Phone,
                    TradeName = x.Company.TradeName,
                    Enabled = x.Company.Enabled,
                    Id = x.Company.Id,
                },
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
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.UUID == request.UUID, cancellation);
    }

    public async Task<Request[]> ListAllAsync(CancellationToken cancellation)
    {
        Request[] result = await context.Requests
            .Select(x => new Request
            {
                UUID = x.UUID,
                RequesterId = x.RequesterId,
                CreateAt = x.CreateAt,
                Amount = x.Amount,
                FirstLevelAt = x.FirstLevelAt,
                SecondLevelAt = x.SecondLevelAt,
                InvoiceName = x.InvoiceName,
                HasBudget = x.HasBudget,
                HasInvoice = x.HasInvoice,
                Level = x.Level,
                Status = x.Status,
                BudgetName = x.BudgetName,
                Note = x.Note,
                PaymentDate = x.PaymentDate,
                FinishedAt = x.FinishedAt,
                ApprovedFirstLevel = x.ApprovedFirstLevel,
                ApprovedSecondLevel = x.ApprovedSecondLevel,
                CompanyId = x.CompanyId,
                Company = new Company
                {
                    Id = x.Company.Id,
                    LegalName = x.Company.LegalName,
                    TradeName = x.Company.TradeName,
                    Address = x.Company.Address,
                    CNPJ = x.Company.CNPJ,
                    Email = x.Company.Email,
                    Phone = x.Company.Phone,
                    Enabled = x.Company.Enabled
                },
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
                    Approved = m.Approved,
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
                    Approved = d.Approved,
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

        return result;
    }

    public async Task<Object> MyStatsAsync(Int32 applicationUserId, CancellationToken cancellation)
    {
        Int64 amount = await context.Requests.Where(x => x.RequesterId == applicationUserId && x.ApprovedFirstLevel == true && x.ApprovedSecondLevel == true).SumAsync(x => x.Amount, cancellation);
        Int32 totalRequest = await context.Requests.Where(x => x.RequesterId == applicationUserId).CountAsync(cancellation);
        Int32 totalRequestPending = await context.Requests.Where(x => x.RequesterId == applicationUserId && x.Status == StatusRequest.Pending).CountAsync(cancellation);
        Int32 totalRequestRejected = await context.Requests.Where(x => x.RequesterId == applicationUserId && x.Status == StatusRequest.Reject).CountAsync(cancellation);
        Int32 totalRequestApproved = await context.Requests.Where(x => x.RequesterId == applicationUserId && x.Status == StatusRequest.Approved).CountAsync(cancellation);

        DateTime now = DateTime.UtcNow;
        DateTime firstMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-11);

        // Agrupando por Mês e Status
        var groupedData = await context.Requests
            .Where(r => r.RequesterId == applicationUserId && r.CreateAt >= firstMonth)
            .GroupBy(r => new { r.CreateAt.Year, r.CreateAt.Month, r.Status })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Status = g.Key.Status, // -1, 0, 1
                Count = g.Count()
            })
            .ToListAsync(cancellation);

        // Preparar estrutura dos 12 meses
        System.Globalization.DateTimeFormatInfo labels = System.Globalization.CultureInfo.InvariantCulture.DateTimeFormat;
        var last12Months = Enumerable.Range(0, 12)
            .Select(i => firstMonth.AddMonths(i))
            .Select(d => new
            {
                Year = d.Year,
                Month = d.Month,
                Label = labels.GetAbbreviatedMonthName(d.Month)
            })
            .ToList();

        // Juntar os dados
        var result = last12Months.Select(m =>
        {
            var monthData = groupedData.Where(x => x.Year == m.Year && x.Month == m.Month);

            return new
            {
                Month = m.Label,
                Pending = monthData.FirstOrDefault(x => x.Status == StatusRequest.Pending)?.Count ?? 0,
                Approved = monthData.FirstOrDefault(x => x.Status == StatusRequest.Approved)?.Count ?? 0,
                Rejected = monthData.FirstOrDefault(x => x.Status == StatusRequest.Reject)?.Count ?? 0
            };
        }).ToList();

        var stats = new
        {
            TotalRequests = totalRequest,
            TotalRequestPending = totalRequestPending,
            TotalRequestApproved = totalRequestApproved,
            TotalRequestRejected = totalRequestRejected,
            TotalRequestsByMonth = result,
            TotalAmountRequestsApproved = amount
        };

        return stats;
    }

    public async Task<Request[]> ListPendingRequestsAsync(FilterRequest filter, CancellationToken cancellation)
    {
        IQueryable<Request> query = context.Requests.AsQueryable();

        RequestExtensions.Filter(ref query, filter);

        Request[] results = await query
            .Where(r => r.Status == StatusRequest.Pending && (
            r.Managers.Any(m => m.ManagerId == filter.ApplicationUserId) ||
            r.Directors.Any(d => d.DirectorId == filter.ApplicationUserId) ))
            .OrderBy(x => x.Level)
            .Select(x => new Request
            {
                UUID = x.UUID,
                RequesterId = x.RequesterId,
                CreateAt = x.CreateAt,
                Amount = x.Amount,
                FirstLevelAt = x.FirstLevelAt,
                SecondLevelAt = x.SecondLevelAt,
                InvoiceName = x.InvoiceName,
                HasBudget = x.HasBudget,
                HasInvoice = x.HasInvoice,
                Level = x.Level,
                Status = x.Status,
                BudgetName = x.BudgetName,
                Note = x.Note,
                PaymentDate = x.PaymentDate,
                FinishedAt = x.FinishedAt,
                ApprovedFirstLevel = x.ApprovedFirstLevel,
                ApprovedSecondLevel = x.ApprovedSecondLevel,
                CompanyId = x.CompanyId,
                Company = new Company
                {
                    Id = x.Company.Id,
                    LegalName = x.Company.LegalName,
                    TradeName = x.Company.TradeName,
                    Address = x.Company.Address,
                    CNPJ = x.Company.CNPJ,
                    Email = x.Company.Email,
                    Phone = x.Company.Phone,
                    Enabled = x.Company.Enabled,
                },
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
                    Approved = m.Approved,
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
                    Approved = d.Approved,
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
            })
            .AsNoTracking()
        .ToArrayAsync(cancellation);

        return results;
    }

    public async Task<Request[]> ListApprovedRequestsAsync(FilterRequest filter, CancellationToken cancellation)
    {
        IQueryable<Request> query = context.Requests.AsQueryable();

        RequestExtensions.Filter(ref query, filter);

        Request[] results = await query
            .Where(r => r.Status == StatusRequest.Approved && r.Level == LevelRequest.SecondLevel && r.ApprovedFirstLevel && r.ApprovedSecondLevel)
            .OrderBy(r => r.CreateAt)
            .Select(x => new Request
            {
                UUID = x.UUID,
                RequesterId = x.RequesterId,
                CreateAt = x.CreateAt,
                Amount = x.Amount,
                FirstLevelAt = x.FirstLevelAt,
                SecondLevelAt = x.SecondLevelAt,
                InvoiceName = x.InvoiceName,
                HasBudget = x.HasBudget,
                HasInvoice = x.HasInvoice,
                Level = x.Level,
                Status = x.Status,
                BudgetName = x.BudgetName,
                Note = x.Note,
                PaymentDate = x.PaymentDate,
                FinishedAt = x.FinishedAt,
                ApprovedFirstLevel = x.ApprovedFirstLevel,
                ApprovedSecondLevel = x.ApprovedSecondLevel,
                CompanyId = x.CompanyId,
                Company = new Company
                {
                    Id = x.Company.Id,
                    LegalName = x.Company.LegalName,
                    TradeName = x.Company.TradeName,
                    Address = x.Company.Address,
                    CNPJ = x.Company.CNPJ,
                    Email = x.Company.Email,
                    Phone = x.Company.Phone,
                    Enabled = x.Company.Enabled,
                },
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
                    Approved = m.Approved,
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
                    Approved = d.Approved,
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
            })
            .AsNoTracking()
            .ToArrayAsync(cancellation);

        return results;

    }

    public async Task<Request[]> ListFinishedRequestsAsync(FilterRequest filter, CancellationToken cancellation)
    {
        IQueryable<Request> query = context.Requests.AsQueryable();

        RequestExtensions.Filter(ref query, filter);

        Request[] results = await query
            .Where(r => r.Status == StatusRequest.Approved && r.Level == LevelRequest.Finished)
            .OrderByDescending(r => r.FinishedAt)
            .Select(x => new Request
            {
                UUID = x.UUID,
                RequesterId = x.RequesterId,
                FinisherId = x.FinisherId,
                CreateAt = x.CreateAt,
                Amount = x.Amount,
                FirstLevelAt = x.FirstLevelAt,
                SecondLevelAt = x.SecondLevelAt,
                InvoiceName = x.InvoiceName,
                HasBudget = x.HasBudget,
                HasInvoice = x.HasInvoice,
                Level = x.Level,
                Status = x.Status,
                BudgetName = x.BudgetName,
                Note = x.Note,
                PaymentDate = x.PaymentDate,
                FinishedAt = x.FinishedAt,
                ApprovedFirstLevel = x.ApprovedFirstLevel,
                ApprovedSecondLevel = x.ApprovedSecondLevel,
                CompanyId = x.CompanyId,
                Company = new Company
                {
                    Id = x.Company.Id,
                    LegalName = x.Company.LegalName,
                    TradeName = x.Company.TradeName,
                    Address = x.Company.Address,
                    CNPJ = x.Company.CNPJ,
                    Email = x.Company.Email,
                    Phone = x.Company.Phone,
                    Enabled = x.Company.Enabled,
                },
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
                Finisher = ( x.Finisher != null ) ? new ApplicationUser
                {
                    Email = x.Finisher.Email,
                    Department = x.Finisher.Department,
                    Role = x.Finisher.Role,
                    FullName = x.Finisher.FullName,
                    Id = x.Finisher.Id,
                    Enabled = x.Finisher.Enabled,
                    UserName = x.Finisher.UserName,
                    PictureUrl = x.Finisher.PictureUrl,
                } : null,
                Managers = x.Managers.Select(m => new RequestManager
                {
                    ManagerId = m.ManagerId,
                    RequestUUID = m.RequestUUID,
                    Approved = m.Approved,
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
                    Approved = d.Approved,
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
            })
            .AsNoTracking()
            .ToArrayAsync(cancellation);

        return results;
    }

}
