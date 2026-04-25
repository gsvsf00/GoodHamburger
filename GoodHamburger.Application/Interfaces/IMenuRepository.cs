using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Interfaces
{
    public interface IMenuRepository
    {
        Task<List<MenuItem>> GetAllAsync();
        Task<MenuItem?> GetByIdAsync(Guid id);
    }
}