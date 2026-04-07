using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceHub.Models.Transport
{
    public class TransferStop : IValidatableObject
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Укажите маршрут")]
        public int TransferRouteId { get; set; }
        [ForeignKey("TransferRouteId")]
        public TransferRoute? TransferRoute { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Порядковый номер должен быть от 1 до 100")]
        [Display(Name = "Порядковый номер")]
        public int Order { get; set; }

        [Required(ErrorMessage = "Укажите адрес или название")]
        [Display(Name = "Адрес или название")]
        public string Address { get; set; } = string.Empty;

        [DataType(DataType.Time)]
        [Display(Name = "Время прибытия")]
        public TimeSpan ArrivalTime { get; set; }

        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ArrivalTime == TimeSpan.Zero)
            {
                yield return new ValidationResult(
                    "Укажите время прибытия",
                    new[] { nameof(ArrivalTime) }
                );
            }
        }
    }
}