using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Models;
using ToDoAPI.Repositories.Interfaces;

namespace ToDoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        
        public UserTaskController(IUserTaskRepository userTaskRepository, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _userTaskRepository = userTaskRepository;
        }

        [Authorize]
        [HttpPost("sinc")]
        public ActionResult Restauration(DateTime date)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            return Ok(_userTaskRepository.Restauration(user, date));
        }

        [Authorize]
        [HttpGet("restaurar")]
        public ActionResult Sinc(List<UserTask> tasks) 
        {
            return Ok(_userTaskRepository.Sinc(tasks));
        }
    }
}
