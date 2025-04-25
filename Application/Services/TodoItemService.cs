using Application.DTOs;
using Application.Services.Interfaces;
using Application.UoW;
using Domain.Entities;
using Mapster;

namespace Application.Services;

public class TodoItemService(IUnitOfWork unitOfWork) : ITodoItemService
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<IEnumerable<TodoItemDto>> GetAllAsync()
  {
    var result = await _unitOfWork.TodoItem.GetAllAsync();
    return result.Adapt<List<TodoItemDto>>();
  }

  public async Task<TodoItemDto?> GetByIdAsync(int id)
  {
    var result = await _unitOfWork.TodoItem.GetByIdAsync(id);
    return result?.Adapt<TodoItemDto>();
  }

  public async Task<IEnumerable<TodoItemDto>> GetIncomingItemsAsync(DateTime dateTime)
  {
    var result = await _unitOfWork.TodoItem.GetIncomingItemsAsync(dateTime);
    return result.Adapt<List<TodoItemDto>>();
  }

  public async Task<IEnumerable<TodoItemDto>> GetIncomingTodayAsync()
  {
    var endOfDay = DateTime.UtcNow.Date.AddDays(1); // Start of the next day (exclusive)

    var result = await _unitOfWork.TodoItem.GetIncomingItemsAsync(endOfDay);
    return result.Adapt<List<TodoItemDto>>();
  }

  public async Task<IEnumerable<TodoItemDto>> GetIncomingNextDayAsync()
  {
    var startOfNextDay = DateTime.UtcNow.Date.AddDays(1); // Start of the next day
    var endOfNextDay = startOfNextDay.AddDays(1); // Start of the day after next (exclusive)

    var result = await _unitOfWork.TodoItem.GetIncomingItemsAsync(startOfNextDay, endOfNextDay);
    return result.Adapt<List<TodoItemDto>>();
  }

  public async Task<IEnumerable<TodoItemDto>> GetIncomingThisWeekAsync()
  {
    var now = DateTime.UtcNow;
    var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek); // Start of the week (Sunday 00:00:00)
    var endOfWeek = startOfWeek.AddDays(7); // End of the week (exclusive, next Sunday 00:00:00)

    var result = await _unitOfWork.TodoItem.GetIncomingItemsAsync(endOfWeek);
    return result.Adapt<List<TodoItemDto>>();
  }

  public async Task<TodoItemDto> AddAsync(TodoItemDto model)
  {
    // Ignore the Id from the client
    model.Id = 0;

    ValidateCompletionState(model);

    TodoItem todoItem = model.Adapt<TodoItem>();
    var result = await _unitOfWork.TodoItem.AddAsync(todoItem);
    await _unitOfWork.SaveChangesAsync();
    return result.Adapt<TodoItemDto>();
  }

  public async Task<bool> UpdateAsync(int id, TodoItemDto model)
  {
    if (model.Id != id)
      return false;

    var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id);
    if (todoItem is null)
      return false;

    ValidateCompletionState(model);

    todoItem = model.Adapt<TodoItem>();
    
    _unitOfWork.TodoItem.Update(todoItem);
    await _unitOfWork.SaveChangesAsync();
    return true;
  }

  public async Task<bool> UpdatePercentCompleteAsync(int id, int percentComplete)
  {
    var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id);
    if (todoItem is null)
      return false;

    todoItem.PercentComplete = percentComplete;

    if (percentComplete == 100)
    {
      todoItem.IsCompleted = true; // Automatically set IsCompleted to true if PercentComplete is 100
    }
    else if (percentComplete < 100)
    {
      todoItem.IsCompleted = false; // Set IsCompleted to false if PercentComplete is less than 100
    }
    else
    {
      throw new InvalidOperationException("PercentComplete must be between 0 and 100.");
    }

    _unitOfWork.TodoItem.Update(todoItem);
    await _unitOfWork.SaveChangesAsync();
    return true;
  }

  public async Task<bool> UpdateStatusAsync(int id, bool isCompleted)
  {
    var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id);
    if (todoItem is null)
      return false;

    todoItem.IsCompleted = isCompleted;

    if (isCompleted)
    {
      todoItem.PercentComplete = 100; // Automatically set PercentComplete to 100 if IsCompleted is true
    }
    else if (!isCompleted && todoItem.PercentComplete == 100)
    {
      throw new InvalidOperationException("PercentComplete cannot be 100 if IsCompleted is false.");
    }

    _unitOfWork.TodoItem.Update(todoItem);
    await _unitOfWork.SaveChangesAsync();
    return true;
  }

  public async Task<bool> DeleteAsync(int id)
  {
    var todoItem = await _unitOfWork.TodoItem.GetByIdAsync(id);
    if (todoItem is null)
      return false;

    _unitOfWork.TodoItem.Delete(todoItem);
    await _unitOfWork.SaveChangesAsync();
    return true;
  }

  private static void ValidateCompletionState(TodoItemDto model)
  {
    if (model.IsCompleted && model.PercentComplete != 100)
    {
      model.PercentComplete = 100; // Automatically set PercentComplete to 100 if IsCompleted is true
    }
    else if (!model.IsCompleted && model.PercentComplete == 100)
    {
      throw new InvalidOperationException("PercentComplete cannot be 100 if IsCompleted is false.");
    }
  }
}