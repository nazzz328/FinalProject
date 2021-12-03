using System;

namespace FinalProject.Models
{
    public class ViewPatient
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime ReceiptDate { get; set; }
        public string ForAddingVisib { get; set; }
        public string ForEditingVisib { get; set; }
    }
}

