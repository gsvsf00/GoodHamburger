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

    public async Task<CreateOrderResult> Handle(Guid? sandwichId, Guid? sideId, Guid? drinkId)
    {
        var ids = new[] { sandwichId, sideId, drinkId }
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();

        var menuItems = new List<MenuItem>();

        foreach (var id in ids)
        {
            var item = await _menuRepo.GetByIdAsync(id)
                ?? throw new Exception("Item não encontrado");

            if (menuItems.Any(i => i.Category.Code == item.Category.Code))
                throw new Exception($"Somente um {item.Category.Name} é permitido");

            menuItems.Add(item);
        }

        var subtotal = menuItems.Sum(i => i.Price);
        var discount = _discount.Calculate(menuItems);

        var order = new Order();
        order.SetItems(sandwichId, sideId, drinkId);
        order.ApplyTotals(subtotal, discount);

        await _orderRepo.AddAsync(order);

        return OrderResult(order, menuItems);
    }

    public async Task<CreateOrderResult> Get(Guid id)
    {
        Order order = await _orderRepo.GetByIdAsync(id)
            ?? throw new Exception("Pedido não encontrado");

        var menuItems = new List<MenuItem>();

        if (order.SandwichId.HasValue)
        {
            var sandwich = await _menuRepo.GetByIdAsync(order.SandwichId.Value)
                ?? throw new Exception(" Sanduíche não encontrado");
            menuItems.Add(sandwich);
        }
        if (order.SideId.HasValue)
        {
            var side = await _menuRepo.GetByIdAsync(order.SideId.Value)
                ?? throw new Exception("Extra não encontrado");
            menuItems.Add(side);
        }
        if (order.DrinkId.HasValue)
        {
            var drink = await _menuRepo.GetByIdAsync(order.DrinkId.Value)
                ?? throw new Exception("Refrigerante não encontrado");
            menuItems.Add(drink);
        }

        return OrderResult(order, menuItems);
    }

    private static CreateOrderResult OrderResult(Order order, List<MenuItem> menuItems)
    {
        decimal discountRate = order.Subtotal == 0 ? 0 : order.Discount / order.Subtotal;

        return new CreateOrderResult
        {
            Id = order.Id,
            Products = menuItems.Select(i => new CreateOrderProductResult
            {
                Id = i.Id,
                Name = i.Name,
                Type = i.Category.Code,
                Price = i.Price
            }).ToList(),
            Subtotal = order.Subtotal,
            Discount = order.Discount,
            DiscountRate = discountRate,
            DiscountDescription = GetDiscountDescription(menuItems),
            Total = order.Total
        };
    }

    private static string GetDiscountDescription(List<MenuItem> items)
    {
        var categories = items
            .Select(i => i.Category)
            .Distinct()
            .ToDictionary(c => c.Code, c => c.Name);

        bool hasSandwich = categories.ContainsKey("SANDWICH");
        bool hasSide = categories.ContainsKey("SIDE");
        bool hasDrink = categories.ContainsKey("DRINK");

        if (hasSandwich && hasSide && hasDrink)
            return $"Combo ({categories["SANDWICH"]} + {categories["SIDE"]} + {categories["DRINK"]}) - 20%";

        if (hasSandwich && hasDrink)
            return $"Combo ({categories["SANDWICH"]} + {categories["DRINK"]}) - 15%";

        if (hasSandwich && hasSide)
            return $"Combo ({categories["SANDWICH"]} + {categories["SIDE"]}) - 10%";

        return "No discount applied";
    }
}