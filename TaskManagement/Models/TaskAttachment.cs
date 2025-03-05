using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Models
{
    public class TaskAttachment
    {
        [Key]
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string FileName { get; set; } = null!;
        public string FileUrl { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public virtual Task Task { get; set; } = null!;
    }
}
