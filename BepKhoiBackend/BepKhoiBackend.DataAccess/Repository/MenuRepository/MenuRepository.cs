using BepKhoiBackend.DataAccess.Abstract.MenuAbstract;
using BepKhoiBackend.DataAccess.Helpers;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;
using Microsoft.VisualBasic;
using System.Data.Common;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;

public class MenuRepository : IMenuRepository
{
    private readonly bepkhoiContext _context;

    public MenuRepository(bepkhoiContext context)
    {
        _context = context;
    }

    /*=========== FUNCTION TO CALL IN API============*/
    // Method to get all products have exist with filter
    public async Task<PagedResult<Menu>> GetAllMenus(int page,int pageSize,string sortBy,string sortDirection,int? categoryId = null,bool? isActive = null,string? productNameOrId = null)
    {
        try
        {
            var query = _context.Menus.Where(m => m.IsDelete == false).AsQueryable();

            // Filter theo productId hoặc productName nếu có truyền vào
            if (!string.IsNullOrEmpty(productNameOrId))
            {
                var searchValue = productNameOrId.Trim().ToLower();

                if (ProductValidator.IsPositiveInteger(searchValue))
                {
                    int id = int.Parse(searchValue);
                    ProductValidator.ValidatePositiveProductId(id);
                    query = query.Where(m => m.ProductId == id);
                }
                else
                {
                    query = query.Where(m => m.ProductName.ToLower().Contains(searchValue));
                }
            }

            // Filter by categoryId
            if (categoryId.HasValue)
            {
                query = query.Where(m => m.ProductCategoryId == categoryId.Value);
            }

            // Filter by isActive
            if (isActive.HasValue)
            {
                query = query.Where(m => m.Status == isActive.Value);
            }

            // Apply sorting
            query = MenuHelper.ApplySorting(query, sortBy, sortDirection);

            // Count all records
            var totalRecords = await query.CountAsync();

            // Pagination
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Menu>
            {
                IsSuccess = data.Any(),
                Message = data.Any() ? "Get all menus successfully." : "No product matched the condition.",
                Data = data,
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize
            };
        }

        catch (FormatException ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Invalid format: {ex.Message}" };
        }
        catch (ArgumentException ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Invalid argument: {ex.Message}" };
        }
        catch (InvalidOperationException ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Invalid operation: {ex.Message}" };
        }
        catch (Exception ex)
        {
            return new PagedResult<Menu> { IsSuccess = false, Message = $"Error occurred while getting menus: {ex.Message}" };
        }
    }

    // Method to get product by product id
    public async Task<PagedResult<Menu>> GetMenuById(int pId)
    {
        try
        {
            // Validate input ID
            if (pId <= 0)
            {
                throw new ArgumentException("Product ID must be greater than 0.", nameof(pId));
            }

            var menu = await _context.Menus.Where(m => m.ProductId == pId && m.IsDelete == false)
                .FirstOrDefaultAsync();

            // Check flag if have null 
            if (menu == null)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Menu with ID {pId} not found or has been deleted.",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }

            return new PagedResult<Menu>
            {
                IsSuccess = true,
                Message = $"Found menu with ID {pId}.",
                Data = new List<Menu> { menu },
                TotalRecords = 1,
                Page = 1,
                PageSize = 1
            };
        }
        catch (ArgumentException ex) // Catch parameter get in is not valid
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Invalid argument: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
        catch (Exception ex) // Catch another error
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Error occurred while retrieving menu by ID: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
    }

    // Method to add a new menu to database
    public async Task<PagedResult<Menu>> AddMenu(Menu menu)
    {
        try
        {
            // Flag check object menu get in parameter is null
            if (menu == null)
            {
                throw new ArgumentException("Menu data must not be null.", nameof(menu));
            }

            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();

            return new PagedResult<Menu>
            {
                IsSuccess = true,
                Message = "Menu added successfully.",
                Data = new List<Menu> { menu },
                TotalRecords = 1,
                Page = 1,
                PageSize = 1
            };
        }
        catch (ArgumentException ex) // In case data parameter invalid
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Invalid argument: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
        catch (Exception ex) // Another errors
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Error occurred while adding menu: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
    }

    // Method to update menu and return result as PagedResult
    public async Task<PagedResult<Menu>> UpdateMenu(Menu menu)
    {
        try
        {
            if (menu == null)
            {
                throw new ArgumentException("Menu data must not be null.", nameof(menu));
            }

            var existingMenu = await _context.Menus.FirstOrDefaultAsync(m => m.ProductId == menu.ProductId);

            if (existingMenu == null)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Menu with ID {menu.ProductId} not found.",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }

            if (existingMenu.IsDelete == true)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Menu with ID {menu.ProductId} has been deleted.",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }

            // Manual update
            existingMenu.ProductName = menu.ProductName;
            existingMenu.ProductCategoryId = menu.ProductCategoryId;
            existingMenu.CostPrice = menu.CostPrice;
            existingMenu.SellPrice = menu.SellPrice;
            existingMenu.SalePrice = menu.SalePrice;
            existingMenu.ProductVat = menu.ProductVat;
            existingMenu.Description = menu.Description;
            existingMenu.UnitId = menu.UnitId;
            existingMenu.IsAvailable = menu.IsAvailable;
            existingMenu.Status = menu.Status;

            await _context.SaveChangesAsync();

            return new PagedResult<Menu>
            {
                IsSuccess = true,
                Message = $"Menu with ID {menu.ProductId} updated successfully.",
                Data = new List<Menu> { existingMenu },
                TotalRecords = 1,
                Page = 1,
                PageSize = 1
            };
        }
        catch (ArgumentException ex)
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Invalid argument: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
        catch (Exception ex)
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Error occurred while updating menu: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
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
    public async Task<PagedResult<Menu>> SearchProductByNameOrId(string productNameOrId, int page,int pageSize,string sortBy,string sortDirection,int? categoryId)
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
    
    // Method to get product IsActive with paging
    public async Task<PagedResult<Menu>> GetActiveProductsList(int page, int pageSize,
        string sortBy, string sortDirection, int? categoryId)
    {
        try
        {
            var query = _context.Menus.Where(m => m.IsDelete == false).AsQueryable();

            // Filter by category if provided
            if (categoryId.HasValue)
            {
                query = query.Where(m => m.ProductCategoryId == categoryId.Value);
            }

            // Apply sorting 
            query = MenuHelper.ApplySorting(query, sortBy, sortDirection);

            // Fetch all data
            var allProducts = await query.ToListAsync();

            // Filter products that are active
            var activeProducts = allProducts.Where(m => FilterProductByTypeOfIsActive(m)).ToList();

            var totalRecords = activeProducts.Count;
            // Apply pagination
            var pagedProducts = activeProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Menu>
            {
                IsSuccess = pagedProducts.Any(),
                Message = pagedProducts.Any() ? "Successfully retrieved active products." : "No active products found.",
                Data = pagedProducts,
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"An error occurred: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = page,
                PageSize = pageSize
            };
        }
    }

    // Flag to check product have flag isAvtive
    public static bool FilterProductByTypeOfIsActive(Menu menu)
    {
        try
        {
            // Check if object is null
            if (menu == null)
            {
                throw new ArgumentNullException(nameof(menu), "Product (menu) is null.");
            }

            // Check if product is soft-deleted
            if (menu.IsDelete.HasValue && menu.IsDelete.Value)
            {
                throw new InvalidOperationException($"Product with ID {menu.ProductId} has been deleted.");
            }

            // Check if product is available for sale
            if (menu.IsAvailable.HasValue && !menu.IsAvailable.Value)
            {
                throw new InvalidOperationException($"Product with ID {menu.ProductId} is marked as unavailable.");
            }

            // Check if product status is active
            if (menu.Status.HasValue && !menu.Status.Value)
            {
                throw new InvalidOperationException($"Product with ID {menu.ProductId} is inactive.");
            }

            // If all conditions are satisfied, product is active
            return true;
        }
        catch (ArgumentNullException ex)
        {
            // Handle null object
            Console.WriteLine($"[Filter Error]: {ex.Message}");
            return false;
        }
        catch (InvalidOperationException ex)
        {
            // Handle logic-related issues
            Console.WriteLine($"[Filter Error]: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            // Handle unexpected error
            Console.WriteLine($"[Unexpected Error]: {ex.Message}");
            return false;
        }
    }

    // Method to export to excel file
    public async Task<(byte[] fileContent, string FileName, bool HasData, string ErrorMessage)> ExportActiveProductsToExcelAsync(
    string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null)
    {
        try
        {
            var query = _context.Menus.Where(m => m.IsDelete == false).AsQueryable();

            // Check category
            if (categoryId.HasValue)
            {
                bool isCategoryExist = await _context.ProductCategories.AnyAsync(c => c.ProductCategoryId == categoryId.Value);
                if (!isCategoryExist)
                {
                    return (null, null, false, $"Category ID {categoryId.Value} does not exist.");
                }
                query = query.Where(m => m.ProductCategoryId == categoryId.Value);
            }

            // Apply sorting
            query = MenuHelper.ApplySorting(query, sortBy, sortDirection);

            var products = await query.ToListAsync();

            // Check IsActive
            if (isActive.HasValue)
            {
                products = products.Where(m => FilterProductByTypeOfIsActive(m) == isActive.Value).ToList();
            }

            if (!products.Any())
            {
                return (null, null, false, "No product data found to export.");
            }

            var now = DateTime.Now;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products List");

                // ===== Add Title =====
                var title = $"List of products by day {now:dd/MM/yyyy}";
                worksheet.Range("A1:K1").Merge().Value = title;
                var titleCell = worksheet.Cell("A1");
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Font.FontSize = 16;
                titleCell.Style.Font.FontColor = XLColor.White;
                titleCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#535bed");
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Header
                worksheet.Cell(2, 1).Value = "Product ID";
                worksheet.Cell(2, 2).Value = "Product Name";
                worksheet.Cell(2, 3).Value = "Category ID";
                worksheet.Cell(2, 4).Value = "Cost Price";
                worksheet.Cell(2, 5).Value = "Sell Price";
                worksheet.Cell(2, 6).Value = "Sale Price";
                worksheet.Cell(2, 7).Value = "VAT";
                worksheet.Cell(2, 8).Value = "Description";
                worksheet.Cell(2, 9).Value = "Unit ID";
                worksheet.Cell(2, 10).Value = "Available";
                worksheet.Cell(2, 11).Value = "Status";

                // Header style
                var headerRange = worksheet.Range("A2:K2");
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#535bed");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Data
                int row = 3;
                foreach (var product in products)
                {
                    worksheet.Cell(row, 1).Value = product.ProductId;
                    worksheet.Cell(row, 2).Value = product.ProductName;
                    worksheet.Cell(row, 3).Value = product.ProductCategoryId;
                    worksheet.Cell(row, 4).Value = product.CostPrice;
                    worksheet.Cell(row, 5).Value = product.SellPrice;
                    worksheet.Cell(row, 6).Value = product.SalePrice;
                    worksheet.Cell(row, 7).Value = product.ProductVat;
                    worksheet.Cell(row, 8).Value = product.Description;
                    worksheet.Cell(row, 9).Value = product.UnitId;
                    worksheet.Cell(row, 10).Value = product.IsAvailable == true ? "Yes" : "No";
                    worksheet.Cell(row, 11).Value = product.Status == true ? "Active" : "Inactive";
                    row++;
                }

                // Auto fit columns
                worksheet.Columns().AdjustToContents();

                // Save to memory stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileContent = stream.ToArray();

                    // File name dynamic
                    string fileName = $"{(isActive == true ? "Active" : (isActive == false ? "Inactive" : "All"))}_ProductList_{now:dd_MM_yyyy}" +
                        $"{(categoryId.HasValue ? $"_ByCategory_{categoryId.Value}" : "_No_Filter")}.xlsx";

                    return (fileContent, fileName, true, null); // No error
                }
            }
        }
        catch (TimeoutException)
        {
            return (null, null, false, "Database timeout occurred. Please try again later.");
        }
        catch (DbUpdateException)
        {
            return (null, null, false, "Database update error. Please contact admin.");
        }
        catch (SqlException)
        {
            return (null, null, false, "Database connection error. Please try again later.");
        }
        catch (IOException)
        {
            return (null, null, false, "An IO error occurred while creating Excel file.");
        }
        catch (UnauthorizedAccessException)
        {
            return (null, null, false, "You don't have permission to export this file.");
        }
        catch (Exception)
        {
            return (null, null, false, "An unexpected error occurred. Please contact support.");
        }
    }


    // Method to update price of product
    public async Task<PagedResult<Menu>> UpdatePriceOfProduct(int productId, decimal costPrice, decimal sellPrice, decimal? salePrice, decimal? vat)
    {
        try
        {
            // Condition check input get in
            if (costPrice < 0 || sellPrice < 0 || (salePrice.HasValue && salePrice < 0) || (vat.HasValue && vat < 0))
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = "Cost price, sell price, sale price, and VAT must be greater than or equal to 0.",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }

            // Check exist
            bool isExist = await CheckMenuExistById(productId);
            if (!isExist)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Product with ID {productId} does not exist.",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }

            // Check isDeleted
            bool isDeleted = await CheckMenuIsDelete(productId);
            if (isDeleted)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Product with ID {productId} has been deleted and cannot be updated.",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }

            // Check null of parameter get in
            var existingMenu = await _context.Menus.FindAsync(productId);
            if (existingMenu == null)
            {
                return new PagedResult<Menu>
                {
                    IsSuccess = false,
                    Message = $"Unexpected error: Product with ID {productId} could not be found in database.",
                    Data = new List<Menu>(),
                    TotalRecords = 0,
                    Page = 1,
                    PageSize = 1
                };
            }

            existingMenu.CostPrice = costPrice;
            existingMenu.SellPrice = sellPrice;
            existingMenu.SalePrice = salePrice;
            existingMenu.ProductVat = vat;

            await _context.SaveChangesAsync();

            return new PagedResult<Menu>
            {
                IsSuccess = true,
                Message = $"Product price with ID {productId} updated successfully.",
                Data = new List<Menu> { existingMenu },
                TotalRecords = 1,
                Page = 1,
                PageSize = 1
            };
        }
        catch (ArgumentException ex) // Error of parameter get in
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Invalid argument: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
        catch (DbUpdateException ex) // Error when couldn't update to database
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Database update error: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
        catch (DbException ex) // Error when couldn't connect to database
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"Database connection error: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
        catch (Exception ex) // Another errors
        {
            return new PagedResult<Menu>
            {
                IsSuccess = false,
                Message = $"An unexpected error occurred: {ex.Message}",
                Data = new List<Menu>(),
                TotalRecords = 0,
                Page = 1,
                PageSize = 1
            };
        }
    }

    // Method export to excel file of price
    public async Task<(byte[] fileContent, string FileName, bool HasData, string ErrorMessage)> ExportPriceExcelAsync(
    string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null)
    {
        try
        {
            var query = _context.Menus.Where(m => m.IsDelete == false).AsQueryable();

            if (categoryId.HasValue)
            {
                bool categoryExists = await _context.ProductCategories.AnyAsync(c => c.ProductCategoryId == categoryId.Value);
                if (!categoryExists)
                {
                    return (null, null, false, $"Category ID {categoryId.Value} does not exist.");
                }

                query = query.Where(m => m.ProductCategoryId == categoryId.Value);
            }

            // Apply sorting
            query = MenuHelper.ApplySorting(query, sortBy, sortDirection);
            var products = await query.ToListAsync();
            var now = DateTime.Now;

            if (isActive.HasValue)
            {
                products = products.Where(m => FilterProductByTypeOfIsActive(m) == isActive.Value).ToList();
            }

            if (!products.Any())
            {
                return (null, null, false, "No product price data found to export.");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Product Prices");

                // Add title
                var title = $"List of product by day {now:dd/MM/yyyy}";
                worksheet.Range("A1:F1").Merge().Value = title;
                var titleCell = worksheet.Cell("A1");
                titleCell.Style.Font.Bold = true;
                titleCell.Style.Font.FontSize = 16;
                titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Header row
                var headerRow = 2;
                worksheet.Cell(headerRow, 1).Value = "Product ID";
                worksheet.Cell(headerRow, 2).Value = "Product Name";
                worksheet.Cell(headerRow, 3).Value = "Cost Price";
                worksheet.Cell(headerRow, 4).Value = "Sell Price";
                worksheet.Cell(headerRow, 5).Value = "Sale Price";
                worksheet.Cell(headerRow, 6).Value = "VAT";

                // Format header style
                var headerRange = worksheet.Range("A2:F2");
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#535bed");
                headerRange.Style.Font.FontColor = XLColor.White;
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Data rows
                int row = 3;
                foreach (var product in products)
                {
                    worksheet.Cell(row, 1).Value = product.ProductId;
                    worksheet.Cell(row, 2).Value = product.ProductName;
                    worksheet.Cell(row, 3).Value = product.CostPrice;
                    worksheet.Cell(row, 4).Value = product.SellPrice;
                    worksheet.Cell(row, 5).Value = product.SalePrice;
                    worksheet.Cell(row, 6).Value = product.ProductVat;
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                // Stream to byte array
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var fileContent = stream.ToArray();
                    string fileName = $"ProductPriceList_{now:dd_MM_yyyy}.xlsx";

                    return (fileContent, fileName, true, null); // No error
                }
            }
        }
        catch (TimeoutException)
        {
            return (null, null, false, "Database timeout occurred. Please try again later.");
        }
        catch (DbUpdateException)
        {
            return (null, null, false, "Database error occurred. Please contact support.");
        }
        catch (SqlException)
        {
            return (null, null, false, "Database connection error. Please try again later.");
        }
        catch (IOException)
        {
            return (null, null, false, "An IO error occurred while creating the Excel file.");
        }
        catch (UnauthorizedAccessException)
        {
            return (null, null, false, "You do not have permission to export this data.");
        }
        catch (Exception)
        {
            return (null, null, false, "An unexpected error occurred. Please contact admin.");
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
