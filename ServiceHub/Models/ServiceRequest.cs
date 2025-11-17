using System.ComponentModel.DataAnnotations;

namespace ServiceHub.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public string ServiceType { get; set; } = string.Empty; // "Транспорт", "Поддержка", "Питание", "Помещения"

        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Ожидание"; // "Ожидание", "В работе", "Завершено", "Отменено"

        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}