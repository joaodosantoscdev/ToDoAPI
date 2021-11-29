using ToDoAPI.V1.Models;

namespace ToDoAPI.V1.Repositories.Interfaces
{
    public interface ITokenRepository
    {
        void Add(Token token);

        Token Get(string refreshToken);

        void Update(Token token);
    }
}
