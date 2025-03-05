using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("UserId")]

        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual Category? Category { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }

        [JsonIgnore]
        public virtual ICollection<TaskComment>? TaskComments { get; set; } = new List<TaskComment>();


        [JsonIgnore]
        public virtual ICollection<TaskLabel>? TaskLabels { get; set; } = new List<TaskLabel>();

        [JsonIgnore]
        public virtual ICollection<TaskAttachment>? Attachments { get; set; } = new List<TaskAttachment>();
    }
}
