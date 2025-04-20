using AprovaFacil.Domain.Filters;
using AprovaFacil.Domain.Models;

namespace AprovaFacil.Domain.Extensions;

public static class RequestExtensions
{
    public static IQueryable<Request> Filter(ref IQueryable<Request> query, FilterRequest filter)
    {
        if (filter.Quantity.HasValue)
        {
            query = query.OrderByDescending(x => x.CreateAt).Take(filter.Quantity.Value);
        }

        if (filter.Levels.Length > 0)
        {
            query = query.Where(x => filter.Levels.ToList().Contains(x.Level)).OrderByDescending(x => x.CreateAt);
        }

        return query.AsQueryable();
    }

    public static IQueryable<Request> FilterPersonal(ref IQueryable<Request> query, FilterRequest filter)
    {

        if (filter.ApplicationUserId.HasValue && filter.ApplicationUserId.Value != 0)
        {
            query = query.Where(x => x.RequesterId == filter.ApplicationUserId).OrderByDescending(x => x.CreateAt);
        }

        return query.AsQueryable();
    }
}
