public class CreateOrderHandler
{
    private readonly IOrderRepository _orderRepo;
    private readonly IMenuRepository _menuRepo;
    private readonly DiscountService _discount;

    public CreateOrderHandler(
        IOrderRepository orderRepo,
        IMenuRepository menuRepo,
        DiscountService discount)
    {
        _orderRepo = orderRepo;
        _menuRepo = menuRepo;
        _discount = discount;
    }

    public async Task<Guid> Handle(Guid? sandwichId, Guid? sideId, Guid? drinkId)
    {
        var ids = new[] { sandwichId, sideId, drinkId }
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();

        var items = new List<MenuItem>();

        foreach (var id in ids)
        {
            var item = await _menuRepo.GetByIdAsync(id)
                ?? throw new Exception("Item not found");

            if (items.Any(i => i.Type == item.Type))
                throw new Exception($"Only one {item.Type} allowed");

            items.Add(item);
        }

        var subtotal = items.Sum(i => i.Price);
        var discount = _discount.Calculate(items);

        var order = new Order();
        order.SetItems(sandwichId, sideId, drinkId);
        order.ApplyTotals(subtotal, discount);

        await _orderRepo.AddAsync(order);

        return order.Id;
    }
}