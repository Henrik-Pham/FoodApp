using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPFoods_API.Models.Dto;

public class OrderHeaderCreateDTO
{
    [Required] 
    public string PickupName { get; set; } = string.Empty;
    [Required]
    public string PickupPhoneNumber{ get; set; } = string.Empty;
    [Required]
    public string PickupEmail { get; set; } = string.Empty;
    
    // Add foreign key to an existing user
    public string ApplicationUserId { get; set; } = string.Empty;
    
    public Double OrderTotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    
    public List<OrderDetailsCreateDTO> OrderDetails { get; set; } = new();
}