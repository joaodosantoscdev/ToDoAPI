using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    return Ok();
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
                ApplicationUser user = new ApplicationUser
                {
                    UserName = userDTO.Email,
                    FullName = userDTO.Name,
                    Email = userDTO.Email
                };
                var result = _userManager.CreateAsync(user, userDTO.Password).Result;

                if (!result.Succeeded)
                {
                    List<string> errorsList = new List<string>();
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
    }
}
