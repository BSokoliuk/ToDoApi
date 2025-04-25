using Domain.Interfaces;

namespace Application.UoW;

public interface IUnitOfWork : IDisposable
{
  ITodoItemRepository TodoItem { get; }
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}