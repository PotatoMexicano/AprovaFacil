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
            UUID = request.UUID,
            RequesterId = request.RequesterId,
            InvoiceName = request.InvoiceName,
            BudgetName = request.BudgetName,
            PaymentDate = request.PaymentDate,
            CreateAt = request.CreateAt,
            ApprovedFirstLevelAt = request.ApprovedFirstLevelAt,
            ApprovedSecondLevelAt = request.ApprovedSecondLevelAt,
            ReceivedAt = request.ReceivedAt,
            Amount = request.Amount,
            Note = request.Note,
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
            })],
        };
    }

    public static IQueryable<Request> Filter(ref IQueryable<Request> query, FilterRequest filter, Int32? applicationUserId)
    {
        if (filter.ApplicationUserId.HasValue)
        {
            query = query.Where(x => x.RequesterId == applicationUserId);
        }

        return query.AsQueryable();
    }
}
