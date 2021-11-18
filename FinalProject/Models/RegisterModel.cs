using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class RegisterModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Не указан номер телефона")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }
        [Required]
        public int RoleId { get; set; }
        public List<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}
