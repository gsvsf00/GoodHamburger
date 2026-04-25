namespace GoodHamburger.API.DTOs
{
    public class CreateOrderRequest
    {
        public Guid? SandwichId { get; set; }
        public Guid? SideId { get; set; }
        public Guid? DrinkId { get; set; }
    }
}