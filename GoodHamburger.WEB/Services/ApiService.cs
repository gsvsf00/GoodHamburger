namespace GoodHamburger.WEB.Services
{
    using GoodHamburger.WEB.DTOs;

    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MenuItemDto>?> GetMenu()
            => await _http.GetFromJsonAsync<List<MenuItemDto>>("api/menu");

        public async Task<List<CreateOrderResultDto>?> GetOrders()
            => await _http.GetFromJsonAsync<List<CreateOrderResultDto>>("api/orders/pedidos");

        public async Task<CreateOrderResultDto?> CreateOrder(CreateOrderRequest request)
        {
            var response = await _http.PostAsJsonAsync("api/orders", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CreateOrderResultDto>();
        }

        public async Task UpdateOrder(Guid id, UpdateOrderRequest request)
        {
            var response = await _http.PutAsJsonAsync($"api/orders/pedido/{id}", request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteOrder(Guid id)
        {
            var response = await _http.DeleteAsync($"api/orders/pedido/{id}");
            response.EnsureSuccessStatusCode();
        }
    }

    public class CreateOrderRequest
    {
        public Guid? SandwichId { get; set; }
        public Guid? SideId { get; set; }
        public Guid? DrinkId { get; set; }
    }

    public class UpdateOrderRequest
    {
        public Guid? SandwichId { get; set; }
        public Guid? SideId { get; set; }
        public Guid? DrinkId { get; set; }
    }
}
