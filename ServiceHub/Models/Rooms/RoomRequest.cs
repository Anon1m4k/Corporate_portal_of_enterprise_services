using ServiceHub.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models.Rooms
{
    public class RoomRequest : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public AuthUser? User { get; set; }

        [Required(ErrorMessage = "Выберите помещение")]
        public int RoomId { get; set; }
        public Room? Room { get; set; }

        [Required(ErrorMessage = "Укажите дату и время начала")]
        [Display(Name = "Начало")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Укажите дату и время окончания")]
        [Display(Name = "Окончание")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Укажите количество участников")]
        [Range(1, 100, ErrorMessage = "Количество участников от 1 до 100")]
        [Display(Name = "Количество участников")]
        public int ParticipantsCount { get; set; }

        [Required(ErrorMessage = "Укажите тему встречи")]
        [Display(Name = "Тема встречи")]
        [StringLength(50, ErrorMessage = "Тема встречи не должна превышать 50 символов")]
        public string Topic { get; set; } = string.Empty;

        [Display(Name = "Дополнительные пожелания")]
        [StringLength(100, ErrorMessage = "Дополнительные пожелания не должны превышать 100 символов")]
        public string? Description { get; set; }

        [Display(Name = "Статус")]
        public string Status { get; set; } = "На согласовании";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public int? ApproverId { get; set; }
        public AuthUser? Approver { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "Время окончания должно быть позже времени начала",
                    new[] { nameof(EndTime) }
                );
            }

            if (StartTime < DateTime.Now)
            {
                yield return new ValidationResult(
                    "Нельзя бронировать помещение на прошедшее время",
                    new[] { nameof(StartTime) }
                );
            }

            if (EndTime - StartTime < TimeSpan.FromMinutes(30))
            {
                yield return new ValidationResult(
                    "Бронирование не может быть менее 30 минут",
                    new[] { nameof(EndTime) }
                );
            }

            if (StartTime > DateTime.Now.AddDays(30))
            {
                yield return new ValidationResult(
                    "Бронирование доступно не более чем на 30 дней вперёд",
                    new[] { nameof(StartTime) }
                );
            }
        }
    }
}