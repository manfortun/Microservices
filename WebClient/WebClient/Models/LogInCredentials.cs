using System.ComponentModel.DataAnnotations;

namespace WebClient.Models;

public class LogInCredentials
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;
}
