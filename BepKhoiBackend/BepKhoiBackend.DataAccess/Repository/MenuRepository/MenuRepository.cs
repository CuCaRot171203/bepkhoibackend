using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

public class MenuRepository : IMenuRepository
{
    private readonly bepkhoiContext _context;

    public MenuRepository(bepkhoiContext context)
    {
        _context = context;
    }

    // Method to get all products have exist
    public async Task<IEnumerable<Menu>> GetAllMenus()
    {
        // Incase have found all product of menu
        try
        {
            // screen for product haven't been removed
            return await _context.Menus
                .Where(m => m.IsDelete == false).ToListAsync(); 
        }
        catch (Exception ex)
        {
            throw new Exception("Have error when take list Menu", ex);
        }
    }

    // Method to get product by product id
    public async Task<Menu> GetMenuById(int pId)
    {
        // Incase found product by Id
        try
        {
            // Check if menu have exist in database
            if (!await CheckMenuExistById(pId))
            {
                throw new KeyNotFoundException($"Couldn't found menu with ID : {pId}.");
            }

            var productById = await _context.Menus.FindAsync(pId);

            // Check then product have been deleted or not
            if (productById == null || productById.IsDelete == true)
            {
                throw new KeyNotFoundException("Couldn't found menu or menu have been deleted");
            }

            return productById;
        }
        catch (Exception ex) // Incase couldn't found any results
        {
            throw new Exception($"Error when take menu by Id: {pId}", ex);
        }
    }
    // Method to add a new project to database
    public async Task<Menu> AddMenu(Menu menu)
    {
        // Incase add success
        try
        {
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return menu;
        }
        catch (Exception ex) // Incase add failed
        {
            throw new Exception("Have error when add menu", ex);
        }
    }

    public async Task UpdateMenu(Menu menu)
    {
        try
        {
            var existingMenu = await _context.Menus.FindAsync(menu.ProductId);
            if (existingMenu == null)
            {
                throw new KeyNotFoundException($"Couldn't find product with Id: {menu.ProductId}");
            }

            // Update all values of product from new object import
            _context.Entry(existingMenu).CurrentValues.SetValues(menu);

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error when update product", ex);
        }
    }

    // Method to change flag isDelete of product
    public async Task DeleteMenu(int id)
    {
        try
        {
            var menu = await _context.Menus.FindAsync(id);

            // check menu have exist or not
            if (menu == null)
                throw new KeyNotFoundException($"Couldn't found any product have Id: {id}");
            
            menu.IsDelete = true;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error when delete product", ex);
        }
    }

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
}
