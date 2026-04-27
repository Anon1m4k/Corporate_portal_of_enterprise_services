using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiceHub.Models.Transport
{
    public class TransferRoute
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название маршрута")]
        [StringLength(30, ErrorMessage = "Название маршрута не должно превышать 30 символов")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Описание")]
        [StringLength(100, ErrorMessage = "Описание не должно превышать 100 символов")]
        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Укажите автомобиль")]
        public int CarId { get; set; }
        [ForeignKey("CarId")]
        public Car? Car { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<TransferStop> Stops { get; set; } = new List<TransferStop>();  
    }
}