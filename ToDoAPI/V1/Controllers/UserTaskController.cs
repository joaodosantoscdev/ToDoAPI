using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ToDoAPI.V1.Models;
using ToDoAPI.V1.Repositories.Interfaces;

namespace ToDoAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class UserTaskController : ControllerBase
    {
        // Dependencies Injected | Constructor
        #region DI Injected
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly UserManager<ApplicationUser> _userManager;
      
        public UserTaskController(IUserTaskRepository userTaskRepository, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _userTaskRepository = userTaskRepository;
        }
        #endregion

        // Restauration of the Local Data based on the User
        #region Restauration UserTask Method - Controller
        /// <summary>
        /// Restaura os dados contidos no App de Tarefas.
        /// </summary>
        /// <param name="date">Data de Restauração</param>
        /// <response code="200">Sucesso</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <returns>Tarefas restauradas para o App (Backup)</returns>
        [Authorize]
        [HttpGet("restaurar")]
        public ActionResult Restauration(DateTime? date)
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;

            return Ok(_userTaskRepository.Restauration(user, date));
        }
        #endregion

        // Syncronize the DB data and update  User Local Data
        #region Sinc UserTask Method - Controller
        /// <summary>
        /// Sincroniza e atualiza os dados contidos no App de Tarefas.
        /// </summary>
        /// <param name="tasks">Tarefas</param>
        /// <response code="200">Sucesso</response>
        /// <response code="401">Usuário não autorizado.</response>
        /// <returns>Atualiza as tarefas para o App (Backup)</returns>
        [Authorize]
        [HttpPost("sinc")]
        public ActionResult Sinc(List<UserTask> tasks) 
        {
            return Ok(_userTaskRepository.Sinc(tasks));
        }
        #endregion

    }
}
