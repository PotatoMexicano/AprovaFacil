using AprovaFacil.Domain.DTOs;
using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Extensions;

public static class RequestExtensions
{
    public static RequestDTO ToDTO(this Request request)
    {
        return new RequestDTO
        {
            UUID = request.UUID.ToString("N"),
            RequesterId = request.RequesterId,
            InvoiceName = request.InvoiceName.ToString("N"),
            BudgetName = request.BudgetName.ToString("N"),
            PaymentDate = request.PaymentDate,
            CreateAt = request.CreateAt,
            HasInvoice = request.HasInvoice,
            HasBudget = request.HasBudget,
            FirstLevelAt = request.FirstLevelAt,
            SecondLevelAt = request.SecondLevelAt,
            ReceivedAt = request.FinishedAt,
            Amount = request.Amount,
            Note = request.Note,
            Company = new CompanyDTO
            {
                Id = request.Company.Id,
                LegalName = request.Company.LegalName,
                TradeName = request.Company.TradeName,
                CNPJ = request.Company.CNPJ,
                Phone = request.Company.Phone,
                Email = request.Company.Email,
            },
            Requester = new UserDTO
            {
                Department = request.Requester.Department,
                Email = request.Requester.Email,
                FullName = request.Requester.FullName,
                Id = request.Requester.Id,
                Role = request.Requester.Role,
                PictureUrl = request.Requester.PictureUrl,
                Enabled = request.Requester.Enabled,
            },
            Managers = [.. request.Managers.Select(x => new UserDTO
            {
                Department = x.User.Department,
                Email = x.User.Email,
                FullName = x.User.FullName,
                Id = x.User.Id,
                Role = x.User.Role,
                PictureUrl = x.User.PictureUrl,
                Enabled = x.User.Enabled,
                RequestApproved = x.Approved,
            })],
            Directors = [.. request.Directors.Select(x => new UserDTO
            {
                Department = x.User.Department,
                Email = x.User.Email,
                FullName = x.User.FullName,
                Id = x.User.Id,
                Role = x.User.Role,
                PictureUrl = x.User.PictureUrl,
                Enabled = x.User.Enabled,
                RequestApproved = x.Approved,
            })],
        };
    }

    public static IQueryable<Request> Filter(ref IQueryable<Request> query, FilterRequest filter, Int32? applicationUserId)
    {
        if (applicationUserId.HasValue && applicationUserId.Value != 0)
        {
            query = query.Where(x => x.RequesterId == applicationUserId);
        }

        if (filter.Levels.Length > 0)
        {
            query = query.Where(x => filter.Levels.ToList().Contains(x.Level));
        }

        return query.AsQueryable();
    }
}
