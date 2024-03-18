using System.ComponentModel.DataAnnotations;

namespace AuthService.Services;

public class ValidationService
{
    public bool IsValid(object value, out List<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(value, new ValidationContext(value), validationResults, true);

        return isValid;
    }
}
