using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models.Account
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Введите текущий пароль")]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 25 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите новый пароль")]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 25 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 25 символов")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}