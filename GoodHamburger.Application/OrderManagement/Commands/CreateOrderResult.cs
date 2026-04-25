namespace GoodHamburger.Application.OrderManagement.Commands
{
    public class CreateOrderResult
    {
        public Guid Id { get; set; }
        public List<CreateOrderProductResult> Products { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountRate { get; set; }
        public string DiscountDescription { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class CreateOrderProductResult
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
