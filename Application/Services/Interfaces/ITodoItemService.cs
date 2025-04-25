using Application.DTOs;

namespace Application.Services.Interfaces;

public interface ITodoItemService : IService<TodoItemDto>
{
  // Get items with expiry date greater than now and less than the given dateTime
  Task<IEnumerable<TodoItemDto>> GetIncomingItemsAsync(DateTime dateTime);
}