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

// This controller handles user registration and login
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
        // Get the secret key from appsettings.json
        _secretKey = configuration.GetValue<string>("ApiSettings:Secret") ?? "";
        _response = new ApiResponse();
    }

    // This method creates a new user account
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO model)
    {
        // Check if the data the user sent is valid
        if (!ModelState.IsValid)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            // Get all the error messages
            foreach (var error in ModelState.Values)
            {
                foreach (var item in error.Errors)
                {
                    _response.ErrorMessages.Add(item.ErrorMessage);
                }
            }
            return BadRequest(_response);
        }

        // Create a new user with the info they gave us
        ApplicationUser newUser = new()
        {
            Email = model.Email,
            UserName = model.Email, // I'm using email as the username
            Name = model.Name,
            NormalizedEmail = model.Email.ToUpper(), // This helps with searching
        };
        
        // Try to create the user in the database
        var result = await _userManager.CreateAsync(newUser, model.Password);
        
        if (result.Succeeded)
        {
            // Create roles if they don't exist yet (first time setup)
            if (!_roleManager.RoleExistsAsync(StaticDetail.RoleAdmin).GetAwaiter().GetResult())
            {
                await _roleManager.CreateAsync(new IdentityRole(StaticDetail.RoleAdmin));
                await _roleManager.CreateAsync(new IdentityRole(StaticDetail.RoleCustomer));
            }

            // Give the user a role - admin or customer
            if (model.Role.Equals(StaticDetail.RoleAdmin, StringComparison.InvariantCultureIgnoreCase))
            {
                await _userManager.AddToRoleAsync(newUser, StaticDetail.RoleAdmin);
            }
            else
            {
                // Most users will be customers
                await _userManager.AddToRoleAsync(newUser, StaticDetail.RoleCustomer);
            }
            
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
        
        // If something went wrong, send back the error messages
        foreach (var error in result.Errors)
        {
            _response.ErrorMessages.Add(error.Description);
        }
        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        return BadRequest(_response);
    }
    
    // This method logs in a user and gives them a token
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        // Make sure they filled out everything correctly
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
        
        // Try to find the user by their email
        var userFromDb = await _userManager.FindByEmailAsync(model.Email);

        if (userFromDb != null)
        {
            // Check if their password is correct
            bool isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);

            if (!isValid)
            {
                // Don't tell them if the email exists or not for security
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Invalid credentials");
                return BadRequest(_response);
            }
            
            // Create a JWT token for them to use
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_secretKey);

            if (userFromDb.Email != null)
            {
                // Put user info into the token
                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = new ClaimsIdentity(
                    [
                        new("fullname", userFromDb.Name), // Their name
                        new("id", userFromDb.Id), // Their user ID
                        new(ClaimTypes.Email, userFromDb.Email), // Their email
                        new(ClaimTypes.Role, _userManager.GetRolesAsync(userFromDb).Result.FirstOrDefault() ?? string.Empty) // Their role (admin or customer)
                    ]),
                    Expires = DateTime.UtcNow.AddDays(7), // Token expires in 7 days
                    SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                                                                                                                                                   
                // Actually create the token
                SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
                
                // Send back the user info and their token
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
        
        // If we get here, something went wrong with login
        _response.Result = new LoginResponseDto();
        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        _response.ErrorMessages.Add("Invalid credentials");
        return BadRequest(_response);
    }
}