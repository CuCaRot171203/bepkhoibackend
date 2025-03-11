using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;


namespace BepKhoiBackend.DataAccess.Abstract.MenuAbstract
{
    // Create interface of all tasks
    public interface IMenuRepository
    {
        Task<PagedResult<Menu>> GetAllMenus(int page, int pageSize, string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null, string? productNameOrId = null);
        Task<PagedResult<Menu>> GetMenuById(int pId);
        Task<PagedResult<Menu>> AddMenu(Menu menu);
        Task<PagedResult<Menu>> UpdateMenu(Menu menu);
        Task DeleteMenu(int id);
        Task<bool> CheckMenuExistById(int id);
        Task<bool> CheckMenuIsDelete(int id);
        Task<PagedResult<Menu>> SearchProductByNameOrId(string productNameOrId,int page, int pageSize, string sortBy, string sortDirection, int? categoryId);
        Task<PagedResult<Menu>> GetActiveProductsList(int page, int pageSize, string sortBy, string sortDirection, int? categoryId);
        Task<(byte[] fileContent, string FileName, bool HasData, string ErrorMessage)> ExportActiveProductsToExcelAsync(string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null);
        Task<PagedResult<Menu>> UpdatePriceOfProduct(int productId, decimal costPrice, decimal sellPrice, decimal? salePrice, decimal? vat);
        Task<(byte[] fileContent, string FileName, bool HasData, string ErrorMessage)> ExportPriceExcelAsync(string sortBy, string sortDirection, int? categoryId = null, bool? isActive = null);
    }
}
