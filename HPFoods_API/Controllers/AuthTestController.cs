using HPFoods_API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HPFoods_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthTestController : Controller
{
     [HttpGet]
     [Authorize]
     public ActionResult<string> GetSomething()
     {
          return "You are an authorized user";
     }
     
     [HttpGet ("{someValue:int}")]
     [Authorize(Roles = StaticDetail.RoleAdmin)]
     public ActionResult<string> GetSomething(int someValue)
     {
          return "You are an authorized user, with role of admin";
     }
}