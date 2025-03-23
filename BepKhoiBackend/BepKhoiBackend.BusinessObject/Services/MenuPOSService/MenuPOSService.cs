using BepKhoiBackend.DataAccess.Repository.MenuPOSRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepKhoiBackend.DataAccess.Models;
using BepKhoiBackend.BusinessObject.dtos.MenuPOSDto;
using BepKhoiBackend.BusinessObject.dtos.ProductCategoryDto;
using BepKhoiBackend.BusinessObject.dtos.MenuDto;

namespace BepKhoiBackend.BusinessObject.Services.MenuPOSService
{
    public class MenuPOSService : IMenuPOSService
    {
        private readonly IMenuPOSRepository _menuPOSRepository;

        public MenuPOSService(IMenuPOSRepository menuPOSRepository)
        {
            _menuPOSRepository = menuPOSRepository;
        }

        public List<MenuPOSDto> GetAllMenuPos(int limit, int offset)
        {
            var menus = _menuPOSRepository.GetAllMenuPos(limit, offset);

            return menus.Select(m => new MenuPOSDto
            {
                ProductId = m.ProductId,
                ProductName = m.ProductName,
                CategoryName = m.ProductCategory.ProductCategoryTitle, // Lấy tên danh mục
                SellPrice = m.SellPrice,
                SalePrice = m.SalePrice,
                ProductVat = m.ProductVat,
                UnitTitle = m.Unit.UnitTitle, // Lấy tên đơn vị
                IsAvailable = m.IsAvailable,
                Status = m.Status
            }).ToList();
        }

        public List<ProductCategoryDTO> GetAllProductCategories()
        {
            var categories = _menuPOSRepository.GetAllProductCategories();

            return categories.Select(c => new ProductCategoryDTO
            {
                ProductCategoryId = c.ProductCategoryId,
                ProductCategoryTitle = c.ProductCategoryTitle
            }).ToList();
        }

        public List<MenuPOSDto> FilterProductPos(int? productCategoryId, int limit, int offset)
        {
            var menus = _menuPOSRepository.FilterProductPos(productCategoryId, limit, offset);
            return menus.Select(m => new MenuPOSDto
            {
                ProductId = m.ProductId,
                ProductName = m.ProductName,
                ProductCategoryId = m.ProductCategoryId,
                CategoryName = m.ProductCategory?.ProductCategoryTitle,
                SellPrice = m.SellPrice,
                SalePrice = m.SalePrice,
                ProductVat = m.ProductVat,
                UnitId = m.UnitId,
                UnitTitle = m.Unit?.UnitTitle,
                IsAvailable = m.IsAvailable,
                Status = m.Status
            }).ToList();
        }
    }
}
