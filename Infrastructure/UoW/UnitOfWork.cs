using Application.UoW;
using Domain.Interfaces;
using Infrastructure.DbContexts;

namespace Infrastructure.UoW;

public sealed class UnitOfWork(
  TodoItemDbContext dbContext,
  ITodoItemRepository todoItemRepository) : IUnitOfWork
{
  public ITodoItemRepository TodoItem { get; private set; } = todoItemRepository;

  private readonly TodoItemDbContext _dbContext = dbContext;

  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await _dbContext.SaveChangesAsync(cancellationToken);
  }

  public void Dispose()
  {
    _dbContext.Dispose();
  }
}