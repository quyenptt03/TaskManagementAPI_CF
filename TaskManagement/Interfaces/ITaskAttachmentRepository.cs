using TaskManagement.Models;

namespace TaskManagement.Interfaces
{
    public interface ITaskAttachmentRepository
    {
        Task<IEnumerable<TaskAttachment>> GetAttachmentsByTaskIdAsync(int taskId);
        Task<TaskAttachment> AddAttachmentAsync(int taskId, IFormFile file);
        Task<bool> DeleteAttachmentAsync(int id);
    }
}
