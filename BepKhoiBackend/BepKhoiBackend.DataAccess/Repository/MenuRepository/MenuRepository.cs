using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Helpers;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

public class MenuRepository : IMenuRepository
{
    private readonly bepkhoiContext _context;

    public MenuRepository(bepkhoiContext context)
    {
        _context = context;
    }

    /*=========== FUNCTION TO CALL IN API============*/
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

    // Method to update menu
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

    // Method to search product in menulist by name or ID
    public async Task<PagedResult<Menu>> SearchProductByNameOrId(
        string productNameOrId,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        int? categoryId
    )
    {
        try
        {
            // call method to check parameter input valid or not
            ProductValidator.VallidateStringProductNameOrIdInput(productNameOrId);

            var query = _context.Menus.AsQueryable();

            // Filter and undisplay product have soft deleted
            query = query.Where(m => m.IsDelete == false);

            // Check paramter get in is integer or name
            if (ProductValidator.IsPositiveInteger(productNameOrId))
            {
                // Convert and check ID is valid or not
                int id = int.Parse(productNameOrId);
                ProductValidator.ValidatePositiveProductId(id);

                // Check Id exist in database
                
                // Check Id exist in database
                if (!await CheckMenuExistById(id))
                {
                    return new PagedResult<Menu> { IsSuccess = false, Message = $"Can't find product with ID: {id}." };
                }    
                    
                // Check flag isDelete of product
                if (await CheckMenuIsDelete(id))
                {
                    return new PagedResult<Menu> { IsSuccess = false, Message = $"Product have ID: {id} had been deleted." };
                }

                query = query.Where(m => m.ProductId == id);
            }
            else // incase parameter get in are string name
            {
                var stringInput = productNameOrId.Trim().ToLower();
                query = query.Where(m => m.ProductName.ToLower().Contains(stringInput));
            }

            // Filtered by category if have
            if (categoryId.HasValue)
            {
                query = query.Where(m => m.ProductCategoryId == categoryId.Value);
            }

            query = MenuHelper.ApplySorting(query, sortBy, sortDirection);

            var totalRecords = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Menu>
            {
                IsSuccess = data.Any(),
                Message = data.Any() ? "Find successfully." : "Can't find product matched!.",
                Data = data,
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (FormatException ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Format of data is not valid: {ex.Message}" };
        }
        catch (ArgumentException ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Error parameter get in: {ex.Message}" };
        }
        catch (InvalidOperationException ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Action invalid: {ex.Message}" };
        }
        catch (KeyNotFoundException ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Can't find: {ex.Message}" };
        }
        catch (Exception ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Occur error when find product: {ex.Message}" };
        }
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


}
