
using Application.DTOs;
using Application.Enums;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemController(
  ITodoItemService todoItemService): ControllerBase
{
  private readonly ITodoItemService _todoItemService = todoItemService;

  [HttpGet]
  public async Task<IActionResult> GetAll()
  {
    var todoItems = await _todoItemService.GetAllAsync();
    return Ok(todoItems);
  }

  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetById(int id)
  {
    var todoItem = await _todoItemService.GetByIdAsync(id);
    if (todoItem is null)
    {
      return NotFound();
    }
    return Ok(todoItem);
  }

  [HttpGet("incoming/{dateTime:datetime}")]
  public async Task<IActionResult> GetIncomingItems(DateTime dateTime)
  {
    var todoItems = await _todoItemService.GetIncomingItemsAsync(dateTime);
    return Ok(todoItems);
  }

  [HttpGet("incoming/today")]
  public async Task<IActionResult> GetIncomingToday()
  {
    var todoItems = await _todoItemService.GetIncomingTodayAsync();
    return Ok(todoItems);
  }

  [HttpGet("incoming/nextday")]
  public async Task<IActionResult> GetIncomingNextDay()
  {
    var todoItems = await _todoItemService.GetIncomingNextDayAsync();
    return Ok(todoItems);
  }

  [HttpGet("incoming/week")]
  public async Task<IActionResult> GetIncomingThisWeek()
  {
    var todoItems = await _todoItemService.GetIncomingThisWeekAsync();
    return Ok(todoItems);
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] TodoItemDto dto)
  {
    if (dto.Id != 0)
    {
      return BadRequest("Id should not be set for POST requests.");
    }

    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    var result = await _todoItemService.AddAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
  }

  [HttpPut("{id:int}")]
  public async Task<IActionResult> Put(int id, [FromBody] TodoItemDto dto)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(ModelState);
    }

    UpdateResult result = await _todoItemService.UpdateAsync(id, dto);
    
    return result switch
    {
      UpdateResult.Success => NoContent(),
      UpdateResult.NotFound => NotFound(),
      UpdateResult.BadRequest => BadRequest("Id mismatch."),
      _ => StatusCode(500, "An unexpected error occurred.")
    };
  }

  [HttpPut("{id:int}/percent-complete")]
  public async Task<IActionResult> UpdatePercentComplete(int id, [FromBody] int percentComplete)
  {
    if (percentComplete < 0 || percentComplete > 100)
    {
      return BadRequest("Percent complete must be between 0 and 100.");
    }

    var result = await _todoItemService.UpdatePercentCompleteAsync(id, percentComplete);
    if (!result)
    {
      return NotFound();
    }
    return NoContent();
  }

  [HttpPut("{id:int}/status")]
  public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool isCompleted)
  {
    var result = await _todoItemService.UpdateStatusAsync(id, isCompleted);
    if (!result)
    {
      return NotFound();
    }
    return NoContent();
  }

  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id)
  {
    var result = await _todoItemService.DeleteAsync(id);
    if (!result)
    {
      return NotFound();
    }
    return NoContent();
  }
}