using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
