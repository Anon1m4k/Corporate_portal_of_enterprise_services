using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceHub.Models
{
    public class TransferStop
    {
        public int Id { get; set; }

        [Required]
        public int TransferRouteId { get; set; }
        [ForeignKey("TransferRouteId")]
        public TransferRoute? TransferRoute { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Порядковый номер")]
        public int Order { get; set; }

        [Required]
        [Display(Name = "Адрес или название")]
        public string Address { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Время прибытия")]
        public TimeSpan ArrivalTime { get; set; }

        // Можно добавить комментарий
        public string? Notes { get; set; }
    }
}