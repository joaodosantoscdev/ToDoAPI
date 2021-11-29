using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoAPI.V1.Models
{
    public class UserTask
    {
        [Key]
        public int IdTaskApi { get; set; }
        public int IdTaskApp { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Place { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Done { get; set; }
        public bool Excluded { get; set; }
        public DateTime Created { get; set; }
        public DateTime Att { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
