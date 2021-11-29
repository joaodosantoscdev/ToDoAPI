using Microsoft.AspNetCore.Identity;
using System;
using System.Text;
using ToDoAPI.V1.Models;
using ToDoAPI.V1.Repositories.Interfaces;

namespace ToDoAPI.V1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public ApplicationUser Get(string email, string password)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (_userManager.CheckPasswordAsync(user, password).Result)
            {
                return user;
            }
            else 
            {
                // USE >> Domain notification
                throw new Exception("Usuário não encontrado");
            }

        }

        public ApplicationUser GetById(string id)
        {
            return _userManager.FindByIdAsync(id).Result;
            
        }

        public void Add(ApplicationUser user, string password)
        {
            var result = _userManager.CreateAsync(user, password).Result;
            if (!result.Succeeded)
            {
                StringBuilder sb = new();
                foreach (var error in result.Errors) 
                {
                    sb.Append(error.Description);
                }
                // USE >> Domain notification
                throw new Exception($"Usuário nçaio cadastrado {sb}");
            }

        }

    }
}
