using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models.Account
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}