using System.Linq;
using ToDoAPI.Database;
using ToDoAPI.V1.Models;
using ToDoAPI.V1.Repositories.Interfaces;

namespace ToDoAPI.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ToDoContext _context;
        public TokenRepository(ToDoContext context)
        {
            _context = context;
        }
        public Token Get(string refreshToken)
        {
            return _context.Token.FirstOrDefault(t => t.RefreshToken == refreshToken && t.Used == false);
        }

        public void Add(Token token)
        {
            _context.Token.Add(token);
            _context.SaveChanges();
        }

        public void Update(Token token)
        {
            _context.Token.Update(token);
            _context.SaveChanges();
        }
    }
}
