public interface IMenuRepository
{
    Task<List<MenuItem>> GetAllAsync();
    Task<MenuItem?> GetByIdAsync(Guid id);
}