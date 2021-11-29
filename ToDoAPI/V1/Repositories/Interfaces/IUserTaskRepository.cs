using System;
using System.Collections.Generic;
using ToDoAPI.V1.Models;

namespace ToDoAPI.V1.Repositories.Interfaces
{
    public interface IUserTaskRepository
    {
        List<UserTask> Sinc(List<UserTask> tasks);

        List<UserTask> Restauration(ApplicationUser user, DateTime? dateLastSinc);
    }
}
