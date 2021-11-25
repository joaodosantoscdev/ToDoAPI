using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoAPI.Database;
using ToDoAPI.Models;
using ToDoAPI.Repositories.Interfaces;

namespace ToDoAPI.Repositories
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
