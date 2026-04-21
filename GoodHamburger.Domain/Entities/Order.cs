public class Order
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid? SandwichId { get; private set; }
    public Guid? SideId { get; private set; }
    public Guid? DrinkId { get; private set; }

    public decimal Subtotal { get; private set; }
    public decimal Discount { get; private set; }
    public decimal Total { get; private set; }

    public void SetItems(Guid? sandwich, Guid? side, Guid? drink)
    {
        SandwichId = sandwich;
        SideId = side;
        DrinkId = drink;
    }

    public void ApplyTotals(decimal subtotal, decimal discount)
    {
        Subtotal = subtotal;
        Discount = discount;
        Total = subtotal - discount;
    }
}