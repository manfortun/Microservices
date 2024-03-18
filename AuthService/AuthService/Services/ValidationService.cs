using System.ComponentModel.DataAnnotations;

namespace AuthService.Services;

public class ValidationService
{
    /// <summary>
    /// Determines if an <paramref name="value"/> is valid based from its credentials
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationResults"></param>
    /// <returns></returns>
    public bool IsValid(object value, out List<ValidationResult> validationResults)
    {
        validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(value, new ValidationContext(value), validationResults, true);

        return isValid;
    }
}
