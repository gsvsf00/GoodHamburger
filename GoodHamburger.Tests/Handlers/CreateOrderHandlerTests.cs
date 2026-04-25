namespace GoodHamburger.Tests.Handlers
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly Mock<IMenuRepository> _mockMenuRepo;
        private readonly CreateOrderHandler _handler;

        public CreateOrderHandlerTests()
        {
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockMenuRepo = new Mock<IMenuRepository>();
            var discountService = new DiscountService();
            _handler = new CreateOrderHandler(_mockOrderRepo.Object, _mockMenuRepo.Object, discountService);
        }

        private MenuItem CreateMenuItem(string code, string name, decimal price)
        {
            return new MenuItem
            {
                Id = Guid.NewGuid(),
                Name = name,
                Price = price,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = code, Name = name }
            };
        }

        [Fact]
        public async Task ShouldCreateOrder_WithValidSandwichAndDrink()
        {
            // Arrange
            var sandwichId = Guid.NewGuid();
            var drinkId = Guid.NewGuid();

            var sandwich = CreateMenuItem("SANDWICH", "X Burger", 50);
            sandwich.Id = sandwichId;

            var drink = CreateMenuItem("DRINK", "Refrigerante", 10);
            drink.Id = drinkId;

            _mockMenuRepo.Setup(r => r.GetByIdAsync(sandwichId))
                .ReturnsAsync(sandwich);
            _mockMenuRepo.Setup(r => r.GetByIdAsync(drinkId))
                .ReturnsAsync(drink);

            // Act
            var result = await _handler.CreateOrder(sandwichId, null, drinkId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(60m, result.Subtotal); // 50 + 10
            Assert.Equal(9m, result.Discount);  // 60 * 0.15
            Assert.Equal(51m, result.Total);    // 60 - 9
            _mockOrderRepo.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task ShouldCreateOrder_WithFullCombo()
        {
            // Arrange
            var sandwichId = Guid.NewGuid();
            var sideId = Guid.NewGuid();
            var drinkId = Guid.NewGuid();

            var sandwich = CreateMenuItem("SANDWICH", "X Burger", 50);
            sandwich.Id = sandwichId;
            var side = CreateMenuItem("SIDE", "Batata Frita", 20);
            side.Id = sideId;
            var drink = CreateMenuItem("DRINK", "Refrigerante", 10);
            drink.Id = drinkId;

            _mockMenuRepo.Setup(r => r.GetByIdAsync(sandwichId))
                .ReturnsAsync(sandwich);
            _mockMenuRepo.Setup(r => r.GetByIdAsync(sideId))
                .ReturnsAsync(side);
            _mockMenuRepo.Setup(r => r.GetByIdAsync(drinkId))
                .ReturnsAsync(drink);

            // Act
            var result = await _handler.CreateOrder(sandwichId, sideId, drinkId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(80m, result.Subtotal);  // 50 + 20 + 10
            Assert.Equal(16m, result.Discount);  // 80 * 0.20
            Assert.Equal(64m, result.Total);     // 80 - 16
            Assert.Equal(3, result.Products.Count);
        }

        [Fact]
        public async Task ShouldThrowException_WhenMultipleItemsSameCategoryPresent()
        {
            // Arrange
            var sandwichId1 = Guid.NewGuid();
            var sandwichId2 = Guid.NewGuid();

            var sandwich1 = CreateMenuItem("SANDWICH", "X Burger", 50);
            sandwich1.Id = sandwichId1;
            var sandwich2 = CreateMenuItem("SANDWICH", "X Egg", 55);
            sandwich2.Id = sandwichId2;

            _mockMenuRepo.Setup(r => r.GetByIdAsync(sandwichId1))
                .ReturnsAsync(sandwich1);
            _mockMenuRepo.Setup(r => r.GetByIdAsync(sandwichId2))
                .ReturnsAsync(sandwich2);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(
                () => _handler.CreateOrder(sandwichId1, null, sandwichId2)
            );

            Assert.Contains("permitido", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ShouldThrowException_WhenItemNotFound()
        {
            // Arrange
            var unknownId = Guid.NewGuid();
            _mockMenuRepo.Setup(r => r.GetByIdAsync(unknownId))
                .ReturnsAsync((MenuItem?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(
                () => _handler.CreateOrder(unknownId, null, null)
            );

            Assert.Contains("nao encontrado", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ShouldApplyDiscount_WhenCreatingComboOrder()
        {
            // Arrange
            var sandwichId = Guid.NewGuid();
            var sideId = Guid.NewGuid();

            var sandwich = CreateMenuItem("SANDWICH", "X Burger", 50);
            sandwich.Id = sandwichId;
            var side = CreateMenuItem("SIDE", "Batata Frita", 20);
            side.Id = sideId;

            _mockMenuRepo.Setup(r => r.GetByIdAsync(sandwichId))
                .ReturnsAsync(sandwich);
            _mockMenuRepo.Setup(r => r.GetByIdAsync(sideId))
                .ReturnsAsync(side);

            // Act
            var result = await _handler.CreateOrder(sandwichId, sideId, null);

            // Assert
            Assert.Equal(70m, result.Subtotal);  // 50 + 20
            Assert.Equal(7m, result.Discount);   // 70 * 0.10
            Assert.True(result.DiscountRate > 0);
            Assert.Contains("10%", result.DiscountDescription);
        }

        [Fact]
        public async Task ShouldReturnOrderWithProductDetails()
        {
            // Arrange
            var sandwichId = Guid.NewGuid();

            var sandwich = CreateMenuItem("SANDWICH", "X Burger", 50);
            sandwich.Id = sandwichId;

            _mockMenuRepo.Setup(r => r.GetByIdAsync(sandwichId))
                .ReturnsAsync(sandwich);

            // Act
            var result = await _handler.CreateOrder(sandwichId, null, null);

            // Assert
            Assert.Single(result.Products);
            var product = result.Products.First();
            Assert.Equal("X Burger", product.Name);
            Assert.Equal(50m, product.Price);
            Assert.Equal(sandwichId, product.Id);
        }
    }
}