namespace GoodHamburger.Domain.Entities
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;
    }
}