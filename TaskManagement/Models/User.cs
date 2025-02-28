using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskManagement.Models
{
    public class User : IdentityUser<int>
    {
        //[Key]
        //public int Id { get; set; }
                
        //public string Username { get; set; } = null!;

        //public string Email { get; set; } = null!;

        //public string PasswordHash { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<TaskComment> TaskComments { get; set; } = new List<TaskComment>();

        [JsonIgnore]
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
