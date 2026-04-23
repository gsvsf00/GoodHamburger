namespace GoodHamburger.WEB.DTOs
{
    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryDto Category { get; set; } = default!;
    }
}
