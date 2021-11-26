using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class History
    {
        public int Id { get; set; }
        public string Complaints { get; set; }
        public string Anamnesis { get; set; }
        public string Inspection { get; set; }
        public string Treatment { get; set; }
        public string Conclusion { get; set; }
        [ForeignKey("TestId")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }
        public DateTime CreatedAt { get; set; }
        [ForeignKey("TestId")]
        public int TestId { get; set; }
        public Test Test { get; set; }


    }
}
