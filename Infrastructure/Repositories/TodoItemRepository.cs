using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TodoItemRepository(TodoItemDbContext context) : ITodoItemRepository
{
  private readonly TodoItemDbContext _context = context;

  public async Task<TodoItem?> GetByIdAsync(int id)
  {
    return await _context.TodoItems.AsNoTracking().FirstOrDefaultAsync(ti => ti.Id == id);
  }

  public async Task<IEnumerable<TodoItem>> GetAllAsync()
  {
    return await _context.TodoItems
      .AsNoTracking()
      .ToListAsync();
  }

  public async Task<IEnumerable<TodoItem>> GetIncomingItemsAsync(DateTime dateTime)
  {
    return await _context.TodoItems
      .AsNoTracking()
      .Where(ti => ti.ExpiryDateTime > DateTime.UtcNow && ti.ExpiryDateTime < dateTime)
      .ToListAsync();
  }

  public async Task<IEnumerable<TodoItem>> GetIncomingItemsAsync(DateTime startDateTime, DateTime endDateTime)
  {
    return await _context.TodoItems
      .AsNoTracking()
      .Where(ti => ti.ExpiryDateTime > startDateTime && ti.ExpiryDateTime < endDateTime)
      .ToListAsync();
  }

  public async Task<TodoItem> AddAsync(TodoItem entity)
  {
    var result = await _context.TodoItems.AddAsync(entity);
    return result.Entity;
  }

  public void Update(TodoItem model)
  {
    _context.TodoItems.Update(model);
  }

  public void Delete(TodoItem model)
  {
    _context.TodoItems.Remove(model);
  }
}