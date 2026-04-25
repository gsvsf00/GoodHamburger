namespace GoodHamburger.Tests.Services
{
    public class DiscountServiceTests
    {
        private readonly DiscountService _service = new();

        [Fact]
        public void ShouldApply20PercentDiscount_WhenSandwichSideAndDrinkPresent()
        {
            // Arrange
            var items = new List<MenuItem>
        {
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "X Burger",
                Price = 50m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "SANDWICH", Name = "Sanduiche" }
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Batata Frita",
                Price = 20m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "SIDE", Name = "Extra" }
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Refrigerante",
                Price = 10m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "DRINK", Name = "Bebida" }
            }
        };

            // Act
            var discount = _service.Calculate(items);

            // Assert
            var subtotal = 50 + 20 + 10; // 80m
            var expectedDiscount = subtotal * 0.20m; // 16m
            Assert.Equal(expectedDiscount, discount);
        }

        [Fact]
        public void ShouldApply15PercentDiscount_WhenSandwichAndDrinkPresent()
        {
            // Arrange
            var items = new List<MenuItem>
        {
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "X Burger",
                Price = 50m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "SANDWICH", Name = "Sanduiche" }
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Refrigerante",
                Price = 10m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "DRINK", Name = "Bebida" }
            }
        };

            // Act
            var discount = _service.Calculate(items);

            // Assert
            var subtotal = 50 + 10; // 60m
            var expectedDiscount = subtotal * 0.15m; // 9m
            Assert.Equal(expectedDiscount, discount);
        }

        [Fact]
        public void ShouldApply10PercentDiscount_WhenSandwichAndSidePresent()
        {
            // Arrange
            var items = new List<MenuItem>
        {
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "X Burger",
                Price = 50m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "SANDWICH", Name = "Sanduiche" }
            },
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "Batata Frita",
                Price = 20m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "SIDE", Name = "Extra" }
            }
        };

            // Act
            var discount = _service.Calculate(items);

            // Assert
            var subtotal = 50 + 20; // 70m
            var expectedDiscount = subtotal * 0.10m; // 7m
            Assert.Equal(expectedDiscount, discount);
        }

        [Fact]
        public void ShouldReturnZeroDiscount_WhenOnlySandwichPresent()
        {
            // Arrange
            var items = new List<MenuItem>
        {
            new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = "X Burger",
                Price = 50m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "SANDWICH", Name = "Sanduiche" }
            }
        };

            // Act
            var discount = _service.Calculate(items);

            // Assert
            Assert.Equal(0m, discount);
        }

        [Theory]
        [InlineData(20, 10, 0)] // Side and drink only = no discount
        [InlineData(10, 0, 0)]  // Side only = no discount
        [InlineData(0, 10, 0)]  // Drink only = no discount
        public void ShouldReturnZeroDiscount_WhenMissingSandwich(decimal sidePrice, decimal drinkPrice, decimal expectedDiscount)
        {
            // Arrange
            var items = new List<MenuItem>();

            if (sidePrice > 0)
                items.Add(new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Batata Frita",
                    Price = sidePrice,
                    CategoryId = Guid.NewGuid(),
                    Category = new Category { Code = "SIDE", Name = "Extra" }
                });

            if (drinkPrice > 0)
                items.Add(new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Refrigerante",
                    Price = drinkPrice,
                    CategoryId = Guid.NewGuid(),
                    Category = new Category { Code = "DRINK", Name = "Bebida" }
                });

            // Act
            var discount = _service.Calculate(items);

            // Assert
            Assert.Equal(expectedDiscount, discount);
        }
    }

}