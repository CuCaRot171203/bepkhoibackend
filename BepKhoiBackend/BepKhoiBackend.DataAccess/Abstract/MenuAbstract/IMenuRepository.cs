using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;


namespace BepKhoiBackend.DataAccess.Abstract.MenuAbstract
{
    // Create interface of all tasks
    public interface IMenuRepository
    {
        Task<IEnumerable<Menu>> GetAllMenus(); 
        Task<Menu> GetMenuById(int id);
        Task<Menu> AddMenu(Menu menu);
        Task UpdateMenu(Menu menu);
        Task DeleteMenu(int id);
        Task<bool> CheckMenuExistById(int id);
        Task<bool> CheckMenuIsDelete(int id);
        Task<PagedResult<Menu>> SearchProductByNameOrId(
        string productNameOrId,
        int page,
        int pageSize,
        string sortBy,
        string sortDirection,
        int? categoryId
        );
    }
}
