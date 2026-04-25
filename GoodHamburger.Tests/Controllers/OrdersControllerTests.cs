using GoodHamburger.API.DTOs;

namespace GoodHamburger.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderRepository> _orderRepo;
        private readonly Mock<IMenuRepository> _menuRepo;
        private readonly DiscountService _discount;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _orderRepo = new Mock<IOrderRepository>();
            _menuRepo = new Mock<IMenuRepository>();
            _discount = new DiscountService();

            var handler = new CreateOrderHandler(
                _orderRepo.Object,
                _menuRepo.Object,
                _discount
            );

            _controller = new OrdersController(handler);
        }

        private CreateOrderResult CreateMockOrderResult(Guid orderId = default)
        {
            if (orderId == default)
                orderId = Guid.NewGuid();

            return new CreateOrderResult
            {
                Id = orderId,
                Products = new List<CreateOrderProductResult>
                {
                    new CreateOrderProductResult
                    {
                        Id = Guid.NewGuid(),
                        Name = "X Burger",
                        Type = "Sanduiche",
                        Price = 50m
                    }
                },
                Subtotal = 50m,
                Discount = 0m,
                DiscountRate = 0m,
                DiscountDescription = "Nenhum desconto aplicado",
                Total = 50m
            };
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WithValidOrder()
        {
            var sandwichId = Guid.NewGuid();

            var menuItem = new MenuItem
            {
                Id = sandwichId,
                Name = "X Burger",
                Price = 50m,
                Category = new Category { Code = "SANDWICH", Name = "Sanduiche" }
            };

            _menuRepo.Setup(r => r.GetByIdAsync(sandwichId))
                .ReturnsAsync(menuItem);

            var request = new CreateOrderRequest
            {
                SandwichId = sandwichId
            };

            var response = await _controller.Create(request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<CreateOrderResult>(okResult.Value);

            Assert.Equal(50m, result.Subtotal);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WhenOrderExists()
        {
            var orderId = Guid.NewGuid();

            var order = new Order(orderId);
            order.SetItems(orderId, null, null);

            _orderRepo.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _menuRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new MenuItem
                {
                    Id = orderId,
                    Name = "X Burger",
                    Price = 50m,
                    Category = new Category { Code = "SANDWICH", Name = "Sanduiche" }
                });

            var response = await _controller.Get(orderId);

            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<CreateOrderResult>(okResult.Value);

            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task Get_ShouldThrowException_WhenOrderNotFound()
        {
            var orderId = Guid.NewGuid();

            _orderRepo.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync((Order?)null);

            await Assert.ThrowsAsync<Exception>(() => _controller.Get(orderId));
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithOrders()
        {
            var orders = new List<Order>
            {
                new Order(),
                new Order(),
                new Order()
            };

            _orderRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(orders);

            _menuRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Item",
                    Price = 10m,
                    Category = new Category { Code = "SIDE", Name = "Extra" }
                });

            var response = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<List<CreateOrderResult>>(okResult.Value);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WithUpdatedOrder()
        {
            var orderId = Guid.NewGuid();

            var order = new Order(orderId);
            _orderRepo.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            _menuRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new MenuItem
                {
                    Id = Guid.NewGuid(),
                    Name = "Item",
                    Price = 10m,
                    Category = new Category { Code = "SIDE", Name = "Extra" }
                });

            var request = new UpdateOrderRequest
            {
                SandwichId = Guid.NewGuid()
            };

            var response = await _controller.Update(orderId, request);

            var okResult = Assert.IsType<OkObjectResult>(response);
            var result = Assert.IsType<CreateOrderResult>(okResult.Value);

            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task Delete_ShouldReturnNoContent()
        {
            var orderId = Guid.NewGuid();

            var order = new Order();
            _orderRepo.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync(order);

            var response = await _controller.Delete(orderId);

            Assert.IsType<NoContentResult>(response);
            _orderRepo.Verify(r => r.DeleteAsync(order), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrowException_WhenOrderNotFound()
        {
            var orderId = Guid.NewGuid();

            _orderRepo.Setup(r => r.GetByIdAsync(orderId))
                .ReturnsAsync((Order?)null);

            await Assert.ThrowsAsync<Exception>(() => _controller.Delete(orderId));
        }
    }
}