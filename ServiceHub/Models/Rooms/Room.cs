using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models.Rooms
{
    public class Room
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Укажите название помещения")]
        [Display(Name = "Название")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Расположение (этаж, корпус)")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите вместимость")]
        [Range(1, 100, ErrorMessage = "Вместимость должна быть от 1 до 100")]
        [Display(Name = "Вместимость (человек)")]
        public int Capacity { get; set; }

        [Display(Name = "Оснащение")]
        public string Equipment { get; set; } = string.Empty;

        [Display(Name = "Активно")]
        public bool IsActive { get; set; } = true;

        public ICollection<RoomRequest> RoomRequests { get; set; } = new List<RoomRequest>();
    }
}