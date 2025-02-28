using System.ComponentModel.DataAnnotations;

namespace TaskManagement.DTOs
{
    public class TaskLabelDto
    {
        public int TaskId { get; set; }
        public int LabelId { get; set; }
    }
}
