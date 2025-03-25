using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.Shared.Helpers;


namespace BepKhoiBackend.DataAccess.Abstract.MenuAbstract
{
    // Create interface of all tasks
    public interface IMenuRepository
    {
        Task<bool> CheckMenuExistById(int pId);
        Task<bool> CheckMenuIsDelete(int pId);
        Task<IQueryable<Menu>> GetMenusQueryableAsync();
        Task<Menu?> GetMenuByIdAsync(int pId);
        Task<Menu> AddMenuAsync(Menu menu);
        Task<Menu> UpdateMenuAsync(Menu menu);
        Task DeleteMenuAsync(Menu menu);
        Task<List<Menu>> GetMenusByConditionAsync(int? categoryId, string sortBy, string sortDirection);
        Task<IQueryable<Menu>> GetFilteredMenusAsync(int? categoryId);
        Task<bool> CheckMenuExistByName(string name);
        Task UpdateMenuPriceAsync(Menu menu);
    }
}
