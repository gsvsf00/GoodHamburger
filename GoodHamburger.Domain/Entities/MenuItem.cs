public class MenuItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public ProductType Type { get; set; }
}