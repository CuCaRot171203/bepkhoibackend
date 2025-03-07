using BepKhoiBackend.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BepKhoiBackend.DataAccess.Abstract.MenuAbstract
{
    public interface IMenuRepository
    {
        // Create interface of all tasks
        Task<IEnumerable<Menu>> GetAllMenus(); 
        Task<Menu> GetMenuById(int id);
        Task<Menu> AddMenu(Menu menu);
        Task UpdateMenu(Menu menu);
        Task DeleteMenu(int id);
        Task<bool> CheckMenuExistById(int id);
        Task<bool> CheckMenuIsDelete(int id);
    }
}
