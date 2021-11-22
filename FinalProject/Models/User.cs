using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string HashedPassword { get; set; }
        [ForeignKey("RoleId")]
        public Role Role { get; set; }
        public int RoleId { get; set; }
    }
}
