namespace BepKhoiBackend.BusinessObject.dtos.MenuDto
{
    public class UpdatePriceDto
    {
        public int ProductId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? ProductVat { get; set; }
    }
}
