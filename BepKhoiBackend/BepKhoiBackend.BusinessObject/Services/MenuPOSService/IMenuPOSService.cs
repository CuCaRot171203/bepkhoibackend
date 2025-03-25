using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;
using BepKhoiBackend.BusinessObject.dtos.MenuPOSDto;
using BepKhoiBackend.BusinessObject.dtos.ProductCategoryDto;
using BepKhoiBackend.DataAccess.Models;

namespace BepKhoiBackend.BusinessObject.Services.MenuPOSService
{
    public interface IMenuPOSService
    {
        List<MenuPOSDto> GetAllMenuPos(int limit, int offset);
        List<ProductCategoryDTO> GetAllProductCategories();
        List<MenuPOSDto> FilterProductPos(int? productCategoryId, int limit, int offset);

    }
}
