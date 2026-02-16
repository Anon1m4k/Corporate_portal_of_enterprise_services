using ServiceHub.Models.Account;
using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models
{
    public class TransportRequest
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public AuthUser? User { get; set; }

        [Required(ErrorMessage = "Выберите тип поездки")]
        [Display(Name = "Тип поездки")]
        public string TripType { get; set; } = string.Empty; // "Трансфер", "Служебная поездка"

        [Required(ErrorMessage = "Укажите дату и время")]
        [Display(Name = "Дата и время")]
        public DateTime TripDateTime { get; set; }

        [Required(ErrorMessage = "Укажите пункт отправления")]
        [Display(Name = "Откуда")]
        public string StartPoint { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите пункт назначения")]
        [Display(Name = "Куда")]
        public string EndPoint { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите количество пассажиров")]
        [Range(1, 30, ErrorMessage = "Количество пассажиров должно быть от 1 до 50")]
        [Display(Name = "Количество пассажиров")]
        public int PassengerCount { get; set; }

        [Required(ErrorMessage = "Укажите цель поездки")]
        [Display(Name = "Цель поездки")]
        public string Purpose { get; set; } = string.Empty;

        [Required(ErrorMessage = "Выберите тип автомобиля")]
        [Display(Name = "Тип автомобиля")]
        public string VehicleType { get; set; } = string.Empty; // "Легковой", "Минивэн", "Автобус", "Грузовой"

        [Display(Name = "Модель автомобиля (если известна)")]
        public string? VehicleModel { get; set; }

        // Кто утвердил (заполняется администратором)
        public int? ApproverId { get; set; }
        public AuthUser? Approver { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public string Status { get; set; } = "На согласовании"; // Статусы: Создана, На согласовании, Подтверждена, Выполнена, Отклонена

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; } // Дата подтверждения
    }
}