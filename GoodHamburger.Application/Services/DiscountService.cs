using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Services
{
    public class DiscountService
    {
        public decimal Calculate(List<MenuItem> items)
        {
            var hasSandwich = items.Any(i => i.Category.Code == "SANDWICH");
            var hasSide = items.Any(i => i.Category.Code == "SIDE");
            var hasDrink = items.Any(i => i.Category.Code == "DRINK");

            var subtotal = items.Sum(i => i.Price);

            if (hasSandwich && hasSide && hasDrink)
                return subtotal * 0.20m;

            if (hasSandwich && hasDrink)
                return subtotal * 0.15m;

            if (hasSandwich && hasSide)
                return subtotal * 0.10m;

            return 0;
        }
    }
}