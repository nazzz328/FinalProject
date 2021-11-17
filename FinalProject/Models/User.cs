using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Models
{
    public class User
    {
        public int id { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public Role Role { get; set; }
    }
}
