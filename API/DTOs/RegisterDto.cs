using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public record RegisterDto(
    [Required] string Username,
    [Required] string Password
);