using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class DocFullViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string PassportNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string RusName { get; set; }

    }
}
