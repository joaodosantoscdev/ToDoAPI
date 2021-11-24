using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ToDoAPI.Models;
using ToDoAPI.Repositories.Interfaces;

namespace ToDoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUserRepository userRepository, SignInManager<ApplicationUser> signIn, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _signIn = signIn;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public ActionResult Login(UserDTO userDTO) 
        {
            ModelState.Remove("ConfirmPassword");
            ModelState.Remove("Name");

            if (ModelState.IsValid)
            {
                ApplicationUser user = _userRepository.Get(userDTO.Email, userDTO.Password);
                if (user != null)
                {
                    //Identity login
                    _signIn.SignInAsync(user, false);

                    // Return JWT Token (need to implement)
                    return Ok(BuildToken(user));
                } else
                {
                    return NotFound("Usuário não localizado");
                }
            }
            else 
            {
                return UnprocessableEntity(ModelState);
            }
        }
        [HttpPost("")]
        public ActionResult Register(UserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    UserName = userDTO.Email,
                    FullName = userDTO.Name,
                    Email = userDTO.Email
                };
                var result = _userManager.CreateAsync(user, userDTO.Password).Result;

                if (!result.Succeeded)
                {
                    List<string> errorsList = new();
                    foreach (var error in result.Errors)
                    {
                        errorsList.Add(error.Description);
                    }
                    return UnprocessableEntity(errorsList);
                }
                else 
                {
                    return Ok(user);
                }
            }
            else 
            {
                return UnprocessableEntity(ModelState);
            }
        }

        public object BuildToken(ApplicationUser user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim (JwtRegisteredClaimNames.Sub, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("key-api-jwt-to-do-application"));
            var sign = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var exp = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new(
                issuer: null,
                audience: null,
                claims: claims,
                expires: exp,
                signingCredentials: sign
                );

            var tokenJwt = new JwtSecurityTokenHandler().WriteToken(token).ToString();


            return new {token = tokenJwt, expiration = exp };
        }
    }
}
