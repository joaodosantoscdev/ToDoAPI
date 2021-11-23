using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Database;
using ToDoAPI.Models;
using ToDoAPI.Repositories.Interfaces;

namespace ToDoAPI.Repositories
{
    public class UserTaskRepository : IUserTaskRepository
    {
        private readonly ToDoContext _context;

        public UserTaskRepository(ToDoContext context)
        {
            _context = context;
        }

        public List<UserTask> Restauration(ApplicationUser user, DateTime? dateLastSinc)
        {

            var query = _context.Tasks.Where(t => t.UserId == user.Id).AsQueryable();
            if (dateLastSinc != null)
            {
                query.Where(t => t.Created >= dateLastSinc || t.Att >= dateLastSinc);
            }

            return query.ToList<UserTask>();
        }

        public List<UserTask> Sinc(List<UserTask> tasks)
        {
            var newTasks = tasks.Where(t => t.IdTaskApi == 0);
            //Register new record
            if (tasks.Count > 0)
            {
                foreach (var task in newTasks)
                {
                    _context.Tasks.Add(task);
                }
            }

            var excludedOrUpdatedTasks = tasks.Where(t => t.IdTaskApi != 0);
            // Update records (Excluded)
            if (excludedOrUpdatedTasks.Count() > 0)
            {
                foreach (var task in excludedOrUpdatedTasks)
                {
                    _context.Tasks.Update(task);
                }
            }

            _context.SaveChanges();
            return newTasks.ToList();
        }
    }
}
