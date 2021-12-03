using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
        public string  PassportNumber { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public DateTime ReceiptDate { get; set; }
        public int ProcessingStatus { get; set; }
        public double Temperature { get; set; }
        public string BloodPressure { get; set; }
        public int ObstetId { get; set; }
        public int GynecId { get; set; }
        public int DepHeadId { get; set; }
    }
}
