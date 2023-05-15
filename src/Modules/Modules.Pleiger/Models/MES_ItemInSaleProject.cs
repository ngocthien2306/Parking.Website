namespace Modules.Pleiger.Models
{
    public class MES_ItemInSaleProject
    {
        public string ItemCode { get; set; }
        public decimal RequestQuantity { get; set; }
        public decimal QuantityInStock { get; set; }
        public decimal? PurchaseQuantity { get; set; }
    }
}
