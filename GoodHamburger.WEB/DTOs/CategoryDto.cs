namespace GoodHamburger.WEB.DTOs
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}
