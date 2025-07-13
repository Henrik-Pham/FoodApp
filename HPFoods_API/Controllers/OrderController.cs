using System.Net;
using HPFoods_API.Data;
using HPFoods_API.Models;
using HPFoods_API.Models.Dto;
using HPFoods_API.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPFoods_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly ApiResponse _response;

    public OrderController(ApplicationDbContext db)
    {
        _db = db;
        _response = new ApiResponse();
    }

    [HttpGet]
    public ActionResult<ApiResponse> GetOrders(string userId = "")
    {
        // Get all orders from database with their details and menu items
        IEnumerable<OrderHeader> orderHeaderList = _db.OrderHeaders.Include(u => u.OrderDetails)
            .ThenInclude(u => u.MenuItem).OrderByDescending(u => u.OrderHeaderId);

        if (!string.IsNullOrEmpty(userId))
        {
            orderHeaderList = orderHeaderList.Where(u => u.ApplicationUserId == userId);
        }

        // Return all the orders found
        _response.Result = orderHeaderList;
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    [HttpGet("{orderId:int}")]
    public ActionResult<ApiResponse> GetOrderById(int orderId)
    {
        if (orderId == 0)
        {
            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages.Add("Invalid order ID");
            return BadRequest(_response);
        }

        // Look for the order in the database with all its details
        OrderHeader? orderHeader = _db.OrderHeaders.Include(u => u.OrderDetails)
            .ThenInclude(u => u.MenuItem).FirstOrDefault(u => u.OrderHeaderId == orderId);

        if (orderHeader == null)
        {
            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.ErrorMessages.Add("Order not found");
            return NotFound(_response);
        }

        // Return the order that was found
        _response.Result = orderHeader;
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
    {
        try
        {
            OrderHeader orderHeader = new OrderHeader()
            {
                PickupName = orderHeaderDTO.PickupName,
                PickupPhoneNumber = orderHeaderDTO.PickupPhoneNumber,
                PickupEmail = orderHeaderDTO.PickupEmail,
                OrderDate = DateTime.Now,
                OrderTotalPrice = orderHeaderDTO.OrderTotalPrice,
                Status = StaticDetail.status_confirmed,
                TotalItems = orderHeaderDTO.TotalItems,
                ApplicationUserId = orderHeaderDTO.ApplicationUserId,
            };

            // Add the order header to database first
            _db.OrderHeaders.Add(orderHeader);
            await _db.SaveChangesAsync();

            // Create order details for each item in the order
            for (int i = 0; i < orderHeaderDTO.OrderDetailsDTO.Count; i++)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    OrderHeaderId = orderHeader.OrderHeaderId,
                    MenuItemId = orderHeaderDTO.OrderDetailsDTO[i].MenuItemId,
                    Quantity = orderHeaderDTO.OrderDetailsDTO[i].Quantity,
                    ItemName = orderHeaderDTO.OrderDetailsDTO[i].ItemName,
                    Price = orderHeaderDTO.OrderDetailsDTO[i].Price
                };
                _db.OrderDetails.Add(orderDetail);
            }
            await _db.SaveChangesAsync();
            
            _response.Result = orderHeader;
            _response.StatusCode = HttpStatusCode.Created;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.InternalServerError;
            _response.ErrorMessages.Add(e.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError, _response);
        }
    }
}