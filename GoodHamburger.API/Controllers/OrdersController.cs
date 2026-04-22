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

    [HttpPut("pedido/{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateOrderRequest request)
    {
        var result = await _handler.UpdateOrder(
            id,
            request.SandwichId,
            request.SideId,
            request.DrinkId);

        return Ok(result);
    }

    [HttpDelete("pedido/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _handler.DeleteOrder(id);

        return NoContent();
    }
}