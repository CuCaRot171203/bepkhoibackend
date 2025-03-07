﻿namespace BepKhoiBackend.BusinessObject.dtos.MenuDto
{
    public class MenuDto
    {
        public int? ProductId { get; set; } // couldn't required when add
        public string ProductName { get; set; } = null!;
        public int ProductCategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProductVat { get; set; }
        public string? Description { get; set; }
        public int UnitId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? Status { get; set; }
        public bool? IsDelete { get; set; }
    }
}
