using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    public class Label
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
    }
}
