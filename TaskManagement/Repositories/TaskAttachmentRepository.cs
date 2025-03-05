using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagement.DataAccess;
using TaskManagement.Interfaces;
using TaskManagement.Models;

namespace TaskManagement.Repositories
{
    public class TaskAttachmentRepository : ITaskAttachmentRepository
    {
        private readonly TaskManagementContext _context;
        private readonly IAzureBlobStorageService _blobStorageService;

        public TaskAttachmentRepository(TaskManagementContext context, IAzureBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        public async Task<IEnumerable<TaskAttachment>> GetAttachmentsByTaskIdAsync(int taskId)
        {
            return await _context.TaskAttachment.Where(a => a.TaskId == taskId).ToListAsync();
        }

        public async Task<TaskAttachment> AddAttachmentAsync(int taskId, IFormFile file)
        {
            string fileUrl = await _blobStorageService.UploadFileAsync(file);

            var attachment = new TaskAttachment
            {
                TaskId = taskId,
                FileName = file.FileName,
                FileUrl = fileUrl
            };
            _context.TaskAttachment.Add(attachment);
            await _context.SaveChangesAsync();
            return attachment;
        }

        public async Task<bool> DeleteAttachmentAsync(int id)
        {
            var attachment = await _context.TaskAttachment.FindAsync(id);
            if (attachment == null) return false;

            await _blobStorageService.DeleteFileAsync(attachment.FileUrl);
       
            _context.TaskAttachment.Remove(attachment);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
