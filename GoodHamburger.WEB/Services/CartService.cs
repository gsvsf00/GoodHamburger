using GoodHamburger.WEB.DTOs;

namespace GoodHamburger.WEB.Services
{
    public class CartService
    {
        private readonly Dictionary<string, MenuItemDto> _itemsByCategory = new();
        public event Action? OnChange;

        public IReadOnlyCollection<MenuItemDto> Items => _itemsByCategory.Values;

        public decimal Subtotal => _itemsByCategory.Values.Sum(i => i.Price);

        public decimal Discount => CalculateDiscount();

        public decimal Total => Subtotal - Discount;

        public string DiscountDescription => CalculateDiscountDescription();

        public bool HasSandwich => FindItemByCategoryKind(CategoryKind.Sandwich) is not null;

        public void Add(MenuItemDto item)
        {
            if (item.Category is null)
            {
                return;
            }

            var key = BuildCategoryKey(item.Category.Name, item.Category.Code);

            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            _itemsByCategory[key] = item;
            NotifyStateChanged();
        }

        public void Remove(string categoryCode)
        {
            var key = Normalize(categoryCode);
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            _itemsByCategory.Remove(key);
            NotifyStateChanged();
        }

        public void Clear()
        {
            _itemsByCategory.Clear();
            NotifyStateChanged();
        }

        public CreateOrderRequest ToCreateOrderRequest()
        {
            return new CreateOrderRequest
            {
                SandwichId = FindItemByCategoryKind(CategoryKind.Sandwich)?.Id,
                SideId = FindItemByCategoryKind(CategoryKind.Side)?.Id,
                DrinkId = FindItemByCategoryKind(CategoryKind.Drink)?.Id
            };
        }

        private MenuItemDto? FindItemByCategoryKind(CategoryKind kind)
        {
            return _itemsByCategory.Values.FirstOrDefault(item => MatchesCategoryKind(item.Category?.Name, item.Category?.Code, kind));
        }

        private static string BuildCategoryKey(string? name, string? code)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                return name.Trim().ToUpperInvariant();
            }

            if (!string.IsNullOrWhiteSpace(code))
            {
                return code.Trim().ToUpperInvariant();
            }

            return string.Empty;
        }

        private static bool MatchesCategoryKind(string? name, string? code, CategoryKind kind)
        {
            var normalizedName = Normalize(name);
            var normalizedCode = Normalize(code);

            return kind switch
            {
                CategoryKind.Sandwich => normalizedName is "SANDUICHE" or "SANDWICH" || normalizedCode is "SANDWICH" or "SANDUICHE",
                CategoryKind.Side => normalizedName is "EXTRA" or "SIDE" || normalizedCode is "SIDE" or "EXTRA",
                CategoryKind.Drink => normalizedName is "BEBIDA" or "DRINK" || normalizedCode is "DRINK" or "BEBIDA",
                _ => false
            };
        }

        private static string Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : value.Trim().ToUpperInvariant();
        }

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }

        private decimal CalculateDiscount()
        {
            var subtotal = Subtotal;
            if (subtotal <= 0)
            {
                return 0;
            }

            var hasSandwich = FindItemByCategoryKind(CategoryKind.Sandwich) is not null;
            var hasSide = FindItemByCategoryKind(CategoryKind.Side) is not null;
            var hasDrink = FindItemByCategoryKind(CategoryKind.Drink) is not null;

            if (hasSandwich && hasSide && hasDrink)
                return subtotal * 0.20m;

            if (hasSandwich && hasDrink)
                return subtotal * 0.15m;

            if (hasSandwich && hasSide)
                return subtotal * 0.10m;

            return 0;
        }

        private string CalculateDiscountDescription()
        {
            var hasSandwich = FindItemByCategoryKind(CategoryKind.Sandwich) is not null;
            var hasSide = FindItemByCategoryKind(CategoryKind.Side) is not null;
            var hasDrink = FindItemByCategoryKind(CategoryKind.Drink) is not null;

            if (hasSandwich && hasSide && hasDrink)
                return "Combo aplicado - 20%";

            if (hasSandwich && hasDrink)
                return "Combo aplicado - 15%";

            if (hasSandwich && hasSide)
                return "Combo aplicado - 10%";

            return "Nenhum desconto aplicado";
        }

        private enum CategoryKind
        {
            Sandwich,
            Side,
            Drink
        }
    }
}