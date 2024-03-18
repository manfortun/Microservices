namespace WebClient.DTOs;

public class RegisterCredentialsDto
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = default!;
}
