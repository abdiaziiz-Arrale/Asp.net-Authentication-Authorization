using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;

namespace TodoApp.Controllers
{
      [Route("[Controller]")]
      public class SetupController : ControllerBase
      {
            private readonly ApiDbContext _context;
            private readonly UserManager<IdentityUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly ILogger<SetupController> _loger;

            public SetupController(ApiDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<SetupController> loger)
            {
                  _loger = loger;
                  _context = context;
                  _userManager = userManager;
                  _roleManager = roleManager;
            }
            [HttpGet("GetRole")]
            public async Task<IActionResult> GetRole()
            {
                  var role = _roleManager.Roles.ToList();
                  return Ok(role);
            }
            [HttpGet("GetUsers")]
            public async Task<IActionResult> GetUsers()
            {
                  var users = await _userManager.Users.ToListAsync();
                  return Ok(users);
            }
            [HttpPost("AddRole")]
            public async Task<IActionResult> createRole(string rolename)
            {
                  var roleexist = await _roleManager.RoleExistsAsync(rolename);
                  if (!roleexist)
                  {
                        var roleresult = await _roleManager.CreateAsync(new IdentityRole(rolename));
                        if (roleresult.Succeeded)
                        {
                              _loger.LogInformation($"The Role {rolename} is created Successfully");

                              return Ok(new
                              {
                                    result = $"The Role {rolename} is created Successfully"
                              });
                        }
                        else
                        {
                              return BadRequest(new { error = "Role Has Not been Added Something wrong" });
                        }
                  }
                  return BadRequest(new { error = "Role Already Exist" });
            }

      }
}