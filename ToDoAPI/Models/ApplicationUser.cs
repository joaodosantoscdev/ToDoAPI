using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<UserTask> Tasks { get; set; }
        [ForeignKey("UserId")]
        public virtual ICollection<Token> Tokens { get; set; }
    }
}
