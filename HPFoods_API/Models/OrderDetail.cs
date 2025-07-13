using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPFoods_API.Models;

public class OrderDetail
{
    [Key]
    public int OrderDetailId { get; set; }
    [Required]
    public int OrderHeaderId { get; set; }
    [Required]
    public long MenuItemId { get; set; }
    
    // Foreign key to a menu item
    [ForeignKey("MenuItemId")]
    public MenuItem? MenuItem { get; set; } 
    
    [Required]
    public int Quantity { get; set; }
    // Shows the menu item name ordered in case it got updated on the web page
    [Required]
    public string ItemName { get; set; } = string.Empty;
    // Shows the price of the menu item in case the price is updated on the web page
    [Required]
    public int Price { get; set; }
}