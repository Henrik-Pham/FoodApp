using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPFoods_API.Models;

public class OrderHeader
{
    [Key]
    public int OrderHeaderId { get; set; }

    [Required] public string PickupName { get; set; } = string.Empty;
    [Required]
    public string PickupPhoneNumber{ get; set; } = string.Empty;
    [Required]
    public string PickupEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    
    // Add foreign key to an existing user
    public string ApplicationUserId { get; set; } = string.Empty;
    [ForeignKey("ApplicationUserId")]
    public ApplicationUser? ApplicationUser { get; set; }
    
    public Double OrderTotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    
    public List<OrderDetail> OrderDetails { get; set; } = new();
}