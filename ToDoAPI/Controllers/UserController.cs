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
using ToDoAPI.Repositories;
using ToDoAPI.Repositories.Interfaces;

namespace ToDoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUserRepository userRepository, 
                              SignInManager<ApplicationUser> signIn, 
                              UserManager<ApplicationUser> userManager, 
                              ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _signIn = signIn;
            _userManager = userManager;
            _tokenRepository = tokenRepository;
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
                    /*_signIn.SignInAsync(user, false);*/

                    // Return JWT Token 
                    return GenerateToken(user);
                }
                else
                {
                    return NotFound("Usuário não localizado");
                }
            }
            else 
            {
                return UnprocessableEntity(ModelState);
            }
        }

        [HttpPost("renovar")]
        public IActionResult Renew(TokenDTO tokenDTO)
        {
           var refreshTokenDB = _tokenRepository.Get(tokenDTO.RefreshToken);
           
            if (refreshTokenDB == null)
            {
                return NotFound();
            }

            // old RefreshToken, update and disable
            refreshTokenDB.Att = DateTime.Now;
            refreshTokenDB.Used = true;
            _tokenRepository.Update(refreshTokenDB);

            //Generate a new token and save
            var user = _userRepository.GetById(refreshTokenDB.UserId);

            return GenerateToken(user);
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

        public TokenDTO BuildToken(ApplicationUser user)
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

            var refreshToken = Guid.NewGuid().ToString();
            var expRefreshToken = DateTime.UtcNow.AddHours(2);

            var tokenDTO = new TokenDTO{Token = tokenJwt, Expiration = exp, ExpirationRefreshToken = expRefreshToken, RefreshToken = refreshToken};

            return tokenDTO;
        }

        private ActionResult GenerateToken(ApplicationUser user)
        {
            var token = BuildToken(user);
            var tokenModel = new Token()
            {
                RefreshToken = token.RefreshToken,
                ExpirationToken = token.Expiration,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                User = user,
                Created = DateTime.Now,
                Used = false
            };
            _tokenRepository.Add(tokenModel);
            return Ok(token);
        }
    }
}
