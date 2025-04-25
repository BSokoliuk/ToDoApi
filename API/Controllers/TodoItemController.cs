
using Application.DTOs;
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

    var result = await _todoItemService.UpdateAsync(id, dto);
    if (!result)
    {
      return BadRequest();
    }
    return Ok();
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