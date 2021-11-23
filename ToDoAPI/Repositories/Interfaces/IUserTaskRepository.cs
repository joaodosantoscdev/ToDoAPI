using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Models;

namespace ToDoAPI.Repositories.Interfaces
{
    public interface IUserTaskRepository
    {
        List<UserTask> Sinc(List<UserTask> tasks);

        List<UserTask> Restauration(ApplicationUser user, DateTime? dateLastSinc);
    }
}
