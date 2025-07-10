using System.Net;
using HPFoods_API.Data;
using HPFoods_API.Models;
using HPFoods_API.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HPFoods_API.Controllers;

[Route("api/MenuItem")]
[ApiController]
public class MenuItemController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ApiResponse _response;
    private readonly IWebHostEnvironment _env;

    public MenuItemController(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _response = new ApiResponse();
        _env = env;
    }

    [HttpGet]
    public IActionResult GetMenuItems()
    {
        _response.Result = _db.MenuItems.ToList();
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    [HttpGet("{id:int}", Name = "GetMenuItem")]
    public IActionResult GetMenuItem(int id)
    {
        if (id == 0)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            return BadRequest(_response);
        }

        MenuItem menuItem = _db.MenuItems.FirstOrDefault(i => i.Id == id);
        _response.Result = menuItem;
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (menuItemCreateDto.File == null || menuItemCreateDto.File.Length == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = ["File is required"];
                    return BadRequest(_response);
                }

                var imagesPath = Path.Combine(_env.WebRootPath, "images");
                if (!Directory.Exists(imagesPath))
                {
                    Directory.CreateDirectory(imagesPath);
                }

                var filePath = Path.Combine(imagesPath, menuItemCreateDto.File.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Upload the image
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await menuItemCreateDto.File.CopyToAsync(stream);
                }

                MenuItem menuItem = new MenuItem()
                {
                    Name = menuItemCreateDto.Name,
                    Description = menuItemCreateDto.Description,
                    Price = menuItemCreateDto.Price,
                    Category = menuItemCreateDto.Category,
                    SpecialTag = menuItemCreateDto.SpecialTag,
                    Image = "images/" + menuItemCreateDto.File.FileName,
                };

                _db.MenuItems.Add(menuItem);
                await _db.SaveChangesAsync();

                _response.Result = menuItemCreateDto;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, _response);

            }
            else
            {
                _response.IsSuccess = false;
            }
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [e.ToString()];
        }

        return BadRequest(_response);
    }

    [HttpPut]
    public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (menuItemUpdateDto.File == null || menuItemUpdateDto.Id != id)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                MenuItem? menuItemFromDb = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id);

                if (menuItemFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                menuItemFromDb.Name = menuItemUpdateDto.Name;
                menuItemFromDb.Description = menuItemUpdateDto.Description;
                menuItemFromDb.Price = menuItemUpdateDto.Price;
                menuItemFromDb.Category = menuItemUpdateDto.Category;
                menuItemFromDb.SpecialTag = menuItemUpdateDto.SpecialTag;

                if (menuItemUpdateDto.File != null && menuItemUpdateDto.File.Length > 0)
                {
                    var imagesPath = Path.Combine(_env.WebRootPath, "images");
                    if (!Directory.Exists(imagesPath))
                    {
                        Directory.CreateDirectory(imagesPath);
                    }

                    var filePath = Path.Combine(imagesPath, menuItemUpdateDto.File.FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }

                    var filePathOldFile = Path.Combine(_env.WebRootPath, menuItemFromDb.Image);
                    if (System.IO.File.Exists(filePathOldFile))
                    {
                        System.IO.File.Delete(filePathOldFile);
                    }

                    // uplading image
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await menuItemUpdateDto.File.CopyToAsync(stream);
                    }

                    menuItemFromDb.Image = "images/" + menuItemUpdateDto.File.FileName;
                }

                _db.MenuItems.Update(menuItemFromDb);
                await _db.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);

            }
            else
            {
                _response.IsSuccess = false;
            }
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [e.ToString()];
        }

        return BadRequest(_response);
    }

    [HttpDelete]
    public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                MenuItem? menuItemFromDb = await _db.MenuItems.FirstOrDefaultAsync(u => u.Id == id);

                if (menuItemFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }


                var filePathOldFile = Path.Combine(_env.WebRootPath, menuItemFromDb.Image);
                if (System.IO.File.Exists(filePathOldFile))
                {
                    System.IO.File.Delete(filePathOldFile);
                }
                
                _db.MenuItems.Remove(menuItemFromDb);
                await _db.SaveChangesAsync();

                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);

            }
            else
            {
                _response.IsSuccess = false;
            }
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [e.ToString()];
        }

        return BadRequest(_response);
    }
}