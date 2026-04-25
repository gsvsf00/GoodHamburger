using GoodHamburger.Application.Interfaces;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class MenuRepository : IMenuRepository
{
    private readonly AppDbContext _context;

    public MenuRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuItem>> GetAllAsync()
        => await _context.MenuItems
            .Include(item => item.Category)
            .ToListAsync();

    public async Task<MenuItem?> GetByIdAsync(Guid id)
        => await _context.MenuItems
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == id);
}