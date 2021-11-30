using System.Linq;
using ToDoAPI.Database;
using ToDoAPI.V1.Models;
using ToDoAPI.V1.Repositories.Interfaces;

namespace ToDoAPI.V1.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        // Dependencies Injected | Constructor
        #region DI - Injected
        private readonly ToDoContext _context;
        public TokenRepository(ToDoContext context)
        {
            _context = context;
        }
        #endregion

        // Get Token on DB
        #region GET Token - Database
        public Token Get(string refreshToken)
        {
            return _context.Token.FirstOrDefault(t => t.RefreshToken == refreshToken && t.Used == false);
        }
        #endregion

        // Add Token on DB
        #region ADD Token - Database
        public void Add(Token token)
        {
            _context.Token.Add(token);
            _context.SaveChanges();
        }
        #endregion

        // Update Token on DB
        #region UPDATE Token - Database
        public void Update(Token token)
        {
            _context.Token.Update(token);
            _context.SaveChanges();
        }
        #endregion

    }
}
