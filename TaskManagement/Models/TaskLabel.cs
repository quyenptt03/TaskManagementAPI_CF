using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    public class TaskLabel
    {
        [Key]
        public int TaskId { get; set; }

        [Key]
        public int LabelId { get; set; }

        [JsonIgnore]
        [ForeignKey("LabelId")]
        public virtual Label? Label { get; set; }
        
        [JsonIgnore]
        [ForeignKey("TaskId")]
        public virtual Task? Task { get; set; }
    }
}
