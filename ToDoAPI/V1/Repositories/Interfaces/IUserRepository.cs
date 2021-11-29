using ToDoAPI.V1.Models;

namespace ToDoAPI.V1.Repositories.Interfaces
{
    public interface IUserRepository
    {
        void Add(ApplicationUser user, string password);

        ApplicationUser GetById(string id);

        ApplicationUser Get(string email, string password);
    }
}
