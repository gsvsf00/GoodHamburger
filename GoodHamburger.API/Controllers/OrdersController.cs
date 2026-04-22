using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly CreateOrderHandler _handler;

    public OrdersController(CreateOrderHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        var result = await _handler.Handle(
            request.SandwichId,
            request.SideId,
            request.DrinkId);

        return Ok(result);
    }

    [HttpGet("pedido/{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var result = await _handler.Get(id);

        return Ok(result);
    }
}