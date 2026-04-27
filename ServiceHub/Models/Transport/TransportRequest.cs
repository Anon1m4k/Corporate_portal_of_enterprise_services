using ServiceHub.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models.Transport
{
    public class TransportRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public AuthUser? User { get; set; }

        [Required(ErrorMessage = "Укажите дату и время поездки")]
        [Display(Name = "Дата и время")]
        public DateTime TripDateTime { get; set; }

        [Required(ErrorMessage = "Укажите пункт отправления")]
        [StringLength(30, ErrorMessage = "Точка отправления не должна превышать 30 символов")]
        [Display(Name = "Точка отправления")]
        public string StartPoint { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите пункт назначения")]
        [StringLength(30, ErrorMessage = "Пункт назначения не должн превышать 30 символов")]
        [Display(Name = "Пункт назначения")]
        public string EndPoint { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите количество пассажиров")]
        [Range(1, 30, ErrorMessage = "Количество пассажиров должно быть от 1 до 30")]
        [Display(Name = "Количество пассажиров")]
        public int PassengerCount { get; set; }

        [Required(ErrorMessage = "Укажите цель поездки")]
        [Display(Name = "Цель поездки")]
        [StringLength(100, ErrorMessage = "Цель поездки не должна превышать 100 символов")]
        public string Purpose { get; set; } = string.Empty;

        [Display(Name = "Автомобиль")]
        public int? CarId { get; set; }
        public Car? Car { get; set; }

        public int? ApproverId { get; set; }
        public AuthUser? Approver { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public string Status { get; set; } = "На согласовании";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }
}