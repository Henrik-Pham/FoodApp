using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPFoods_API.Models.Dto;

public class OrderHeaderUpdateDTO
{
    [Required]
    public int OrderHeaderId { get; set; }
    [Required]
    public string PickupName { get; set; } = string.Empty;
    [Required]
    public string PickupPhoneNumber{ get; set; } = string.Empty;
    [Required]
    public string PickupEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}