using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public record LoginDto(
    [Required] string Username,
    [Required] string Password
);