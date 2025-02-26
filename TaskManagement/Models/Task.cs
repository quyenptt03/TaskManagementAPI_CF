using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; } = false;

        public int? UserId { get; set; }

        public int? CategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual Category? Category { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

        [JsonIgnore]
        public virtual User? User { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
    }
}
