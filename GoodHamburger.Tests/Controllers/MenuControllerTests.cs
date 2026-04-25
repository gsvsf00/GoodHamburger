namespace GoodHamburger.Tests.Controllers
{
    public class MenuControllerTests
    {
        private readonly Mock<IMenuRepository> _mockMenuRepo;
        private readonly MenuController _controller;

        public MenuControllerTests()
        {
            _mockMenuRepo = new Mock<IMenuRepository>();
            _controller = new MenuController(_mockMenuRepo.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WithMenuItems()
        {
            // Arrange
            var menuItems = new List<MenuItem>
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

            _mockMenuRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(menuItems);

            // Act
            var response = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnedItems = okResult.Value as List<MenuItem>;
            Assert.NotNull(returnedItems);
            Assert.Equal(3, returnedItems.Count);
        }

        [Fact]
        public async Task Get_ShouldReturnOk_WithEmptyList()
        {
            // Arrange
            var emptyMenu = new List<MenuItem>();
            _mockMenuRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(emptyMenu);

            // Act
            var response = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnedItems = okResult.Value as List<MenuItem>;
            Assert.NotNull(returnedItems);
            Assert.Empty(returnedItems);
        }

        [Fact]
        public async Task Get_ShouldCallRepository_ExactlyOnce()
        {
            // Arrange
            _mockMenuRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<MenuItem>());

            // Act
            await _controller.Get();

            // Assert
            _mockMenuRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task Get_ShouldReturnMenuItemsWithCorrectProperties()
        {
            // Arrange
            var sandwichId = Guid.NewGuid();
            var menuItems = new List<MenuItem>
        {
            new MenuItem
            {
                Id = sandwichId,
                Name = "X Burger",
                Price = 50m,
                CategoryId = Guid.NewGuid(),
                Category = new Category { Code = "SANDWICH", Name = "Sanduiche" }
            }
        };

            _mockMenuRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(menuItems);

            // Act
            var response = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(response);
            var returnedItems = okResult.Value as List<MenuItem>;
            var item = returnedItems?.First();

            Assert.NotNull(item);
            Assert.Equal(sandwichId, item.Id);
            Assert.Equal("X Burger", item.Name);
            Assert.Equal(50m, item.Price);
            Assert.Equal("SANDWICH", item.Category.Code);
        }
    }
}