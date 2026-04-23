namespace GoodHamburger.WEB.DTOs
{
    public class CreateOrderResultDto
    {
        public Guid Id { get; set; }
        public List<CreateOrderProductResultDto> Products { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountRate { get; set; }
        public string DiscountDescription { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class CreateOrderProductResultDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}