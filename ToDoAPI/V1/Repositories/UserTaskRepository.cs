using System;
using System.Collections.Generic;
using System.Linq;
using ToDoAPI.Database;
using ToDoAPI.V1.Models;
using ToDoAPI.V1.Repositories.Interfaces;

namespace ToDoAPI.V1.Repositories
{
    public class UserTaskRepository : IUserTaskRepository
    {
        // Dependencies Injected | Constructor
        #region DI Injected
        private readonly ToDoContext _context;

        public UserTaskRepository(ToDoContext context)
        {
            _context = context;
        }
        #endregion

        // Restauration for local backup acording to the DB
        #region UserTask RESTAURATION - Database
        public List<UserTask> Restauration(ApplicationUser user, DateTime? dateLastSinc)
        {
            var query = _context.Tasks.Where(t => t.UserId == user.Id).AsQueryable();
            if (dateLastSinc != null)
            {
                query.Where(t => t.Created >= dateLastSinc || t.Att >= dateLastSinc);
            }

            return query.ToList<UserTask>();
        }
        #endregion

        // Synchronize the local user data with the user data on DB
        #region UserTask SINC - Database
        public List<UserTask> Sinc(List<UserTask> tasks)
        {
            var newTasks = tasks.Where(t => t.IdTaskApi == 0).ToList(); ;
            var excludedOrUpdatedTasks = tasks.Where(t => t.IdTaskApi != 0).ToList();
            //Register new record
            if (tasks.Count > 0)
            {
                foreach (var task in newTasks)
                {
                    _context.Tasks.Add(task);
                }
            }
          
            // Update records (Excluded)
            if (excludedOrUpdatedTasks.Count > 0)
            {
                foreach (var task in excludedOrUpdatedTasks)
                {
                    _context.Tasks.Update(task);
                }
            }

            _context.SaveChanges();
            return newTasks.ToList();
        }
        #endregion
    }
}
