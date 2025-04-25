using Application.DTOs;

namespace Application.Services.Interfaces;

public interface ITodoItemService : IService<TodoItemDto>
{
  // Get items with expiry date greater than now and less than the given dateTime
  Task<IEnumerable<TodoItemDto>> GetIncomingItemsAsync(DateTime dateTime);
  Task<IEnumerable<TodoItemDto>> GetIncomingNextDayAsync();
  Task<IEnumerable<TodoItemDto>> GetIncomingThisWeekAsync();
  Task<IEnumerable<TodoItemDto>> GetIncomingTodayAsync();
  Task<bool> UpdatePercentCompleteAsync(int id, int percentComplete);
  Task<bool> UpdateStatusAsync(int id, bool isCompleted);
}