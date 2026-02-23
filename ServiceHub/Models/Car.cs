using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите марку автомобиля")]
        [Display(Name = "Марка")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите модель автомобиля")]
        [Display(Name = "Модель")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите госномер")]
        [Display(Name = "Госномер")]
        [StringLength(9, ErrorMessage = "Госномер не должен превышать 9 символов")]
        public string? LicensePlate { get; set; }

        [Required(ErrorMessage = "Укажите тип автомобиля")]
        [Display(Name = "Тип автомобиля")]
        public string VehicleType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите вместимость")]
        [Display(Name = "Вместимость (пассажиров)")]
        [Range(1, 30, ErrorMessage = "Вместимость должна быть от 1 до 30")]
        public int? PassengerCapacity { get; set; }

        [Display(Name = "Доступен для заказов")]
        public bool IsAvailable { get; set; } = true;

        public ICollection<TransferRoute> TransferRoutes { get; set; } = new List<TransferRoute>();
    }
}