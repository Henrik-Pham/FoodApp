using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using HPFoods_API.Models;
using HPFoods_API.Models.Dto;
using HPFoods_API.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HPFoods_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApiResponse _response;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly string _secretKey;

    public AuthController(
        UserManager<ApplicationUser> userManager, 
        RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _secretKey = configuration.GetValue<string>("ApiSettings:Secret") ?? "";
        _response = new ApiResponse();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
    {
        if (!ModelState.IsValid)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            foreach (var error in ModelState.Values)
            {
                foreach (var item in error.Errors)
                {
                    _response.ErrorMessages.Add(item.ErrorMessage);
                }
            }
            return BadRequest(_response);
        }

        ApplicationUser newUser = new()
        {
            Email = model.Email,
            UserName = model.Email,
            Name = model.Name,
            NormalizedEmail = model.Email.ToUpper(),
        };
        
        var result = await _userManager.CreateAsync(newUser, model.Password);
        
        if (result.Succeeded)
        {
            // If statement works only for the first time when creating roles
            if (!_roleManager.RoleExistsAsync(StaticDetail.RoleAdmin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticDetail.RoleAdmin));
                await _roleManager.CreateAsync(new IdentityRole(StaticDetail.RoleCustomer));
            }

            // Assign user to a role
            if (model.Role.Equals(StaticDetail.RoleAdmin, StringComparison.InvariantCultureIgnoreCase))
            {
                await _userManager.AddToRoleAsync(newUser, StaticDetail.RoleAdmin);
            }
            else
            {
                await _userManager.AddToRoleAsync(newUser, StaticDetail.RoleCustomer);
            }
            
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        
        // Handle registration errors
        foreach (var error in result.Errors)
        {
            _response.ErrorMessages.Add(error.Description);
        }
        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        return BadRequest(_response);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        if (!ModelState.IsValid)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            foreach (var error in ModelState.Values)
            {
                foreach (var item in error.Errors)
                {
                    _response.ErrorMessages.Add(item.ErrorMessage);
                }
            }
            return BadRequest(_response);
        }
        
        var userFromDb = await _userManager.FindByEmailAsync(model.Email);

        if (userFromDb != null)
        {
            bool isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);

            if (!isValid)
            {
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Invalid credentials");
                return BadRequest(_response);
            }
            
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_secretKey);

            if (userFromDb.Email != null)
            {
                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(
                    [
                        new("fullname", userFromDb.Name),
                        new("id", userFromDb.Id),
                        new(ClaimTypes.Email, userFromDb.Email),
                        new(ClaimTypes.Role, _userManager.GetRolesAsync(userFromDb).Result.FirstOrDefault() ?? string.Empty)
                    ]),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                LoginResponseDto loginResponse = new()
                {
                    Email = userFromDb.Email,
                    Token = tokenHandler.WriteToken(token),
                    Role = _userManager.GetRolesAsync(userFromDb).Result.FirstOrDefault() ?? string.Empty,
                };
                
                _response.Result = loginResponse;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
        }
        _response.Result = new LoginResponseDto();
        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        _response.ErrorMessages.Add("Invalid credentials");
        return BadRequest(_response);
    }
}