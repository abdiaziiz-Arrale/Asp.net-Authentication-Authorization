using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Configuration;
using TodoApp.Models.DTOs.Request;
using TodoApp.Models.DTOs.Request.Responce;

namespace TodoApp.Controllers
{
      [Route("[controller]")]
      [ApiController]
      public class AuthManagement : ControllerBase
      {
            private readonly UserManager<IdentityUser> _userManager;
            private readonly JwtConfig _jwtconfig;


            public AuthManagement(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionMonitor)
            {
                  _userManager = userManager;
                  _jwtconfig = optionMonitor.CurrentValue;
            }

            [HttpPost("RegesterUser")]
            public async Task<IActionResult> RegesterUser([FromBody] UserRegestrationDTO user)
            {
                  if (ModelState.IsValid)
                  {
                        var existingUser = await _userManager.FindByEmailAsync(user.Email);
                        if (existingUser is not null)
                        {
                              return BadRequest(new RegestrationResponces()
                              {
                                    Error = new List<string>(){
                          "Email Already in Use"
                      },
                                    Success = false
                              });
                        }
                        var newUser = new IdentityUser() { Email = user.Email, UserName = user.Username };
                        var isCreated = await _userManager.CreateAsync(newUser, user.password);
                        if (isCreated.Succeeded)
                        {
                              var tokenjwt = GenarateJwtToken(newUser);

                              return Ok(new RegestrationResponces()
                              {
                                    Success = true,
                                    Token = tokenjwt

                              });
                        }
                        else
                        {
                              return BadRequest(new RegestrationResponces()
                              {
                                    Error = isCreated.Errors.Select(x => x.Description).ToList(),
                                    Success = false
                              });
                        }

                  }

                  return BadRequest(new RegestrationResponces()
                  {
                        Error = new List<string>(){
                          "Invalid Regestration Attempt"
                      },
                        Success = false
                  });
            }



            private string GenarateJwtToken(IdentityUser user)
            {
                  var jwtTokenHandler = new JwtSecurityTokenHandler();
                  var keyInput = "random_text_with_at_least_32_chars";
                  var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyInput));

                  var TokenDescriptor = new SecurityTokenDescriptor
                  {
                        Subject = new ClaimsIdentity(new[]
                      {
                new Claim("Id",user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),

                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),

            }),
                        Expires = DateTime.UtcNow.AddHours(6),
                        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                  };
                  var token = jwtTokenHandler.CreateToken(TokenDescriptor);
                  var jwttoken = jwtTokenHandler.WriteToken(token);
                  return jwttoken;
            }
            [HttpPost("login")]
            public async Task<IActionResult> login(UserLoginDTO userLoginDTO)
            {
                  if (ModelState.IsValid)
                  {
                        var existingUser = await _userManager.FindByEmailAsync(userLoginDTO.Email);
                        if (existingUser is null)
                        {
                              return BadRequest(new RegestrationResponces()
                              {
                                    Error = new List<string>(){
                        "invalid Email attempt"
                },
                                    Success = false
                              });
                        }
                        var cheackpassword = await _userManager.CheckPasswordAsync(existingUser, userLoginDTO.password);
                        if (!cheackpassword)
                        {
                              return BadRequest(new RegestrationResponces()
                              {
                                    Error = new List<string>(){
                        "invalid password attempt"
                },
                                    Success = false
                              });
                        }
                        var jwttoken = GenarateJwtToken(existingUser);
                        return Ok(new RegestrationResponces()
                        {
                              Success = true,
                              Token = jwttoken
                        });
                  }
                  return BadRequest(new RegestrationResponces()
                  {
                        Error = new List<string>(){
                        "invalid Login attempt"
                },
                        Success = false
                  });
            }

      }
}