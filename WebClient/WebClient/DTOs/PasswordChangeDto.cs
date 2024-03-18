using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebClient.DTOs;

public class PasswordChangeDto
{
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Password)]
    [DisplayName("Enter new password")]
    public string NewPassword { get; set; } = default!;

    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    [DisplayName("Confirm New Password")]
    public string ConfirmNewPassword { get; set; } = default!;
}
