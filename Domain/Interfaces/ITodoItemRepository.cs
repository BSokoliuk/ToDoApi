using Domain.Entities;

namespace Domain.Interfaces;

public interface ITodoItemRepository : IRepository<TodoItem>
{
  // Get items with expiry date greater than now and less than the given dateTime
  Task<IEnumerable<TodoItem>> GetIncomingItemsAsync(DateTime dateTime);
  Task<IEnumerable<TodoItem>> GetIncomingItemsAsync(DateTime startDateTime, DateTime endDateTime);
}