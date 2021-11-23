using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Models;

namespace ToDoAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void Add(ApplicationUser user, string password);

        ApplicationUser Get(string email, string password);
    }
}
