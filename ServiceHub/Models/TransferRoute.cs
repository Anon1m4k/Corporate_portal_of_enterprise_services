using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceHub.Models
{
    public class TransferRoute
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название маршрута")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Укажите автомобиль")]
        public int CarId { get; set; }
        [ForeignKey("CarId")]
        public Car? Car { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<TransferStop> Stops { get; set; } = new List<TransferStop>();  
    }
}