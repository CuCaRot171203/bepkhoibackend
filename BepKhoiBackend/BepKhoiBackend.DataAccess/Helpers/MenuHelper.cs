using BepKhoiBackend.DataAccess.Models;
using System.Linq;

namespace BepKhoiBackend.DataAccess.Helpers
{
    public static class MenuHelper
    {
        public static IQueryable<Menu> ApplySorting(IQueryable<Menu> query, string sortBy, string sortDirection)
        {
            // Default is sort by productId
            sortBy = sortBy?.ToLower() ?? "productid";
            sortDirection = sortDirection?.ToLower() ?? "desc";

            return (sortBy, sortDirection) switch
            {
                ("productname", "asc") => query.OrderBy(m => m.ProductName),
                ("productname", "desc") => query.OrderByDescending(m => m.ProductName),
                ("sellprice", "asc") => query.OrderBy(m => m.SellPrice),
                ("sellprice", "desc") => query.OrderByDescending(m => m.SellPrice),
                ("productid", "asc") => query.OrderBy(m => m.ProductId),
                ("productid", "desc") => query.OrderByDescending(m => m.ProductId),
                _ => query.OrderBy(m => m.ProductId)
            };
        }
    }
}
