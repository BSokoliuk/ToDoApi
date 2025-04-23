using Domain.Common;

namespace Domain.Entities;

public class TodoItem : EntityBase<int>
{
  public string Title { get; set; } = string.Empty;
  public string Description { get; set; } = string.Empty;
  public int PercentComplete { get; set; } = 0; // 0-100
  public bool IsCompleted { get; set; } = false;
  public DateTime ExpiryDateTime { get; set; } = DateTime.UtcNow;
}