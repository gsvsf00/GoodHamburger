namespace GoodHamburger.WEB.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid? SandwichId { get; set; }
        public Guid? SideId { get; set; }
        public Guid? DrinkId { get; set; }
        public decimal Total { get; set; }
    }
}
