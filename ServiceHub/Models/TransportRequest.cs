using ServiceHub.Models;
using ServiceHub.Models.Account;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class TransportRequest : IValidatableObject
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public AuthUser? User { get; set; }

    [Display(Name = "Дата и время")]
    public DateTime TripDateTime { get; set; }

    [Required(ErrorMessage = "Укажите пункт отправления")]
    [Display(Name = "Откуда")]
    public string StartPoint { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите пункт назначения")]
    [Display(Name = "Куда")]
    public string EndPoint { get; set; } = string.Empty;

    [Required(ErrorMessage = "Укажите количество пассажиров")]
    [Range(1, 30, ErrorMessage = "Количество пассажиров должно быть от 1 до 30")]
    [Display(Name = "Количество пассажиров")]
    public int PassengerCount { get; set; }

    [Required(ErrorMessage = "Укажите цель поездки")]
    [Display(Name = "Цель поездки")]
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TripDateTime == default)
        {
            yield return new ValidationResult(
                "Укажите дату и время",
                new[] { nameof(TripDateTime) }
            );
        }
    }
}