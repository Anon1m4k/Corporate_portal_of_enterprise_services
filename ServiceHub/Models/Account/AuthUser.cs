using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models.Account
{
    public class AuthUser
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [StringLength(50, ErrorMessage = "Email не должен превышать 50 символов")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Не указан пароль")]
        [StringLength(25, MinimumLength = 8, ErrorMessage = "Пароль должен быть от 8 до 25 символов")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
    ErrorMessage = "Пароль должен содержать заглавные и строчные буквы, цифры и специальные символы")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Не указана фамилия")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Не указан отдел")]
        [StringLength(50, ErrorMessage = "Отдел не должен превышать 50 символов")]
        public string Department { get; set; } = string.Empty;

        public string Role { get; set; } = "User";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}