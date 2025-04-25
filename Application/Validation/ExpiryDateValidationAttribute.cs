using System.ComponentModel.DataAnnotations;

namespace Application.Validation;

public class ExpiryDateValidationAttribute : ValidationAttribute
{
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value is DateTime expiryDate)
    {
      if (expiryDate <= DateTime.UtcNow)
      {
        return new ValidationResult("ExpiryDateTime must be in the future.");
      }
    }

    return ValidationResult.Success;
  }
}