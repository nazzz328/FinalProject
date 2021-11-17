using System.Collections.Generic;

namespace FinalProject.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RusName { get; set; }
        public List <User> Users { get; set; }
        public Role ()
        {
            Users = new List<User> ();
        }
    }
}
