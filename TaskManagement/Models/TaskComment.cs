using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    public class TaskComment
    {
        public int Id { get; set; }

        public int TaskId { get; set; }

        public int UserId { get; set; }

        public string Content { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        public virtual Task? Task { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }
    }
}
