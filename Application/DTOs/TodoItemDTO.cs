using System.ComponentModel.DataAnnotations;
using Application.Validation;

namespace Application.DTOs;

public record TodoItemDto
{
  public int Id { get; set; }

  [Required(ErrorMessage = "Title is required.")]
  [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
  public string Title { get; set; } = string.Empty;

  [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
  public string Description { get; set; } = string.Empty;

  [Range(0, 100, ErrorMessage = "PercentComplete must be between 0 and 100.")]
  public int PercentComplete { get; set; } = 0;

  public bool IsCompleted { get; set; } = false;

  [ExpiryDateValidation]
  public DateTime ExpiryDateTime { get; set; } = DateTime.UtcNow;
}