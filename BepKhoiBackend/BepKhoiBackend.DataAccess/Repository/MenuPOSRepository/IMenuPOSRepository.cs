using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.DataAccess.Models;
namespace BepKhoiBackend.DataAccess.Repository.MenuPOSRepository
{
    public interface IMenuPOSRepository
    {
        List<Menu> GetAllMenuPos(int limit, int offset);
        List<ProductCategory> GetAllProductCategories();
        List<Menu> FilterProductPos(int? productCategoryId, int limit, int offset);

    }
}
