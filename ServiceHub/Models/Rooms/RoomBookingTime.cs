using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models.Rooms
{
    public class RoomBookingTime : IValidatableObject
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today;

        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Date.Date < DateTime.Today)
                yield return new ValidationResult("Дата не может быть раньше сегодняшнего дня", new[] { nameof(Date) });
            if (Date.Date > DateTime.Today.AddDays(30))
                yield return new ValidationResult("Бронирование возможно не более чем на 30 дней вперёд", new[] { nameof(Date) });
            if (EndTime <= StartTime)
                yield return new ValidationResult("Время окончания должно быть позже времени начала", new[] { nameof(EndTime) });
            if (EndTime - StartTime < TimeSpan.FromMinutes(30))
                yield return new ValidationResult("Минимальная длительность бронирования — 30 минут", new[] { nameof(EndTime) });
        }
    }
}