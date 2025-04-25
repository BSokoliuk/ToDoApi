namespace Application.DTOs;

public record TodoItemDto
{
  public int Id { get; set; }
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int PercentComplete { get; set; } = 0; // 0-100
  public bool IsCompleted { get; set; } = false;
  public DateTime ExpiryDateTime { get; set; } = DateTime.UtcNow;
}