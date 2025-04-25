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