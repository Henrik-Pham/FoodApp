using System.ComponentModel.DataAnnotations;

namespace HPFoods_API.Models.Dto;

public class ReqisterRequestDto
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}