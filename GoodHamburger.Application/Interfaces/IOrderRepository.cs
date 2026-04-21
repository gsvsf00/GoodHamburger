public interface IOrderRepository
{
	Task AddAsync(Order order);
	Task<Order?> GetByIdAsync(Guid id);
	Task<List<Order>> GetAllAsync();
	Task UpdateAsync(Order order);
	Task DeleteAsync(Order order);
}