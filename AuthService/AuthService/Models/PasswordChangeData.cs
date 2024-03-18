using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class PasswordChangeData
{
    [Required]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = "This field is required.")]
    public string NewPassword { get; set; } = default!;

    [Required(ErrorMessage = "This field is required.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    public string ConfirmNewPassword { get; set; } = default!;
}
