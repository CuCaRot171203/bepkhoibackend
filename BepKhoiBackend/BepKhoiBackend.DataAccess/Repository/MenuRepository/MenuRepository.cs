using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Helpers;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using BepKhoiBackend.DataAccess.Repository.Base;
using Microsoft.Extensions.Logging;

public class MenuRepository : RepositoryBase, IMenuRepository
{
    private readonly bepkhoiContext _context;

    public MenuRepository(bepkhoiContext context, ILogger<MenuRepository> logger) : base(logger)
    {
        _context = context;
    }

    /*=========== SHARED FUNCTION ================*/

    // Method to check product have exist or not by Id
    public async Task<bool> CheckMenuExistById(int pId)
    {
        return await _context.Menus.AnyAsync(
            m => m.ProductId == pId);
    }

    // Method to check if product have to soft delete or not by id
    public async Task<bool> CheckMenuIsDelete(int pId)
    {
        var menu = await _context.Menus.FindAsync(pId);
        return (menu != null
            && menu.IsDelete == true);
    }

    // Method to check if product have exist name
    public async Task<bool> CheckMenuExistByName(string name)
    {
       return await _context.Menus.AllAsync(m => m.ProductName == name);
    }


    /*========= CALL METHOD ==========*/

    // Get all menu
    public async Task<IQueryable<Menu>> GetMenusQueryableAsync()
    {
        return _context.Menus.Where(m => m.IsDelete == false).AsQueryable();
    }

    // Get menu by id
    public async Task<Menu?> GetMenuByIdAsync(int pId)
    {
        if (pId <= 0)
        {
            throw new ArgumentException("Product ID must be greater than 0.", nameof(pId));
        }

        return await _context.Menus
            .Where(m => m.ProductId == pId && m.IsDelete == false)
            .FirstOrDefaultAsync();
    }

    // Add menu
    public async Task<Menu> AddMenuAsync(Menu menu)
    {
        return await ExecuteDbActionAsync(async () =>
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return menu;
        });
    }

    // Update Menu
    public async Task<Menu> UpdateMenuAsync(Menu menu)
    {
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
        return menu;
    }

    // Soft delete (IsDelete = true)
    public async Task DeleteMenuAsync(Menu menu)
    {
        menu.IsDelete = true;
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
    }

    // Get menu list by Id (Filter category, sort)
    public async Task<List<Menu>> GetMenusByConditionAsync(int? categoryId, string sortBy, string sortDirection)
    {
        var query = _context.Menus.Where(m => m.IsDelete == false).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(m => m.ProductCategoryId == categoryId.Value);
        }

        query = MenuHelper.ApplySorting(query, sortBy, sortDirection);

        return await query.ToListAsync();
    }

    // Get filtered
    public async Task<IQueryable<Menu>> GetFilteredMenusAsync(int? categoryId)
    {
        var query = _context.Menus.Where(m => m.IsDelete == false).AsQueryable();

        if (categoryId.HasValue)
        {
            bool categoryExists = await _context.ProductCategories
                .AnyAsync(c => c.ProductCategoryId == categoryId.Value);
            if (!categoryExists) throw new ArgumentException($"Category ID {categoryId.Value} does not exist.");

            query = query.Where(m => m.ProductCategoryId == categoryId.Value);
        }

        return query;
    }

    // Update price of product
    public async Task UpdateMenuPriceAsync(Menu menu)
    {
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync();
    }
}
