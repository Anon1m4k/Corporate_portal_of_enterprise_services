using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models.Account
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [StringLength(50, ErrorMessage = "Email не должен превышать 50 символов")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 25 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Пароль должен содержать заглавные и строчные буквы, цифры и специальные символы")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Пожалуйста, подтвердите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Не указан отдел")]
        [StringLength(50, ErrorMessage = "Отдел не должен превышать 50 символов")]
        public string Department { get; set; }
    }
}
