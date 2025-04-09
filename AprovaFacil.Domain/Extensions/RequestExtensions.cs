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
                City = request.Company.Address.City,
                Complement = request.Company.Address.Complement,
                Neighborhood = request.Company.Address.Neighborhood,
                Number = request.Company.Address.Number,
                PostalCode = request.Company.Address.PostalCode,
                State = request.Company.Address.State,
                Street = request.Company.Address.Street
            },
            Requester = request.Requester.ToDTO(),
            Managers = [.. request.Managers.Select(x => x.ToDTO())],
            Directors = [.. request.Directors.Select(x => x.ToDTO())],
        };
    }

    public static NotificationRequestDTO ToNotificationDTO(this RequestDTO request)
    {
        return new NotificationRequestDTO
        {
            RequestUUID = request.UUID,
            RequesterID = request.RequesterId,
            ManagerIds = [.. request.Managers.Select(x => x.Id)],
            DirectorIds = [.. request.Directors.Select(x => x.Id)],
        };

    }

    public static IQueryable<Request> Filter(ref IQueryable<Request> query, FilterRequest filter, Int32? applicationUserId)
    {
        if (filter.Quantity.HasValue)
        {
            query = query.OrderByDescending(x => x.CreateAt).Take(filter.Quantity.Value);
        }

        if (applicationUserId.HasValue && applicationUserId.Value != 0)
        {
            query = query.Where(x => x.RequesterId == applicationUserId).OrderBy(x => x.Level);
        }

        if (filter.Levels.Length > 0)
        {
            query = query.Where(x => filter.Levels.ToList().Contains(x.Level)).OrderBy(x => x.Level);
        }

        return query.AsQueryable();
    }
}
