using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class RegisterCredentials
{
    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    public string Role { get; set; }
}
