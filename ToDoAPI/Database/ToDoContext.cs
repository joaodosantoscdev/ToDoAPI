using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.V1.Models;

namespace ToDoAPI.Database
{
    public class ToDoContext : IdentityDbContext<ApplicationUser>
    {
        public ToDoContext(DbContextOptions<ToDoContext> options) : base(options)
        {
        }

        public DbSet<UserTask> Tasks { get; set; }
        public DbSet<Token> Token { get; set; }
    }
}
