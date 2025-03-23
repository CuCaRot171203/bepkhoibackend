using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace BepKhoiBackend.DataAccess.Repository.MenuPOSRepository
{
    public class MenuPOSRepository : IMenuPOSRepository
    {
        private readonly bepkhoiContext _context;

        public MenuPOSRepository(bepkhoiContext context)
        {
            _context = context;
        }

        public List<Menu> GetAllMenuPos(int limit, int offset)
        {
            return _context.Menus
                .Include(m => m.ProductCategory)
                .Include(m =>m.Unit)
                .AsNoTracking()
                .OrderBy(m => m.ProductId) // Đảm bảo kết quả có thứ tự
                .Skip(offset)
                .Take(limit)
                .ToList();
        }

        public List<ProductCategory> GetAllProductCategories()
        {
            return _context.ProductCategories
                .AsNoTracking()
                .OrderBy(c => c.ProductCategoryId)
                .ToList();
        }
        public List<Menu> FilterProductPos(int? productCategoryId, int limit, int offset)
        {
            var query = _context.Menus
                .Include(m => m.ProductCategory)
                .Include(m => m.Unit)
                .AsNoTracking();

            if (productCategoryId.HasValue)
            {
                query = query.Where(m => m.ProductCategoryId == productCategoryId);
            }

            return query
                .OrderBy(m => m.ProductId)
                .Skip(offset)
                .Take(limit)
                .ToList();
        }
    }
}
