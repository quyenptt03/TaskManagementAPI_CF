using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.DTOs;
using TaskManagement.Interfaces;
using TaskManagement.Models;
using TaskManagement.Repositories;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/task-attachment")]
    public class TaskAttachmentController : Controller
    {
        private readonly ITaskAttachmentRepository _attachmentRepository;
        private readonly IGenericRepository<Models.Task> _taskRepository;
        public TaskAttachmentController(ITaskAttachmentRepository attachmentRepository, IGenericRepository<Models.Task> taskRepository)
        {
            _attachmentRepository = attachmentRepository;
            _taskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAttachments(int taskId)
        {
            var attachments = await _attachmentRepository.GetAttachmentsByTaskIdAsync(taskId);
            return Ok(attachments);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadAttachment(int taskId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty.");
            }

            var task = await _taskRepository.GetById(taskId);
            if (task == null)
            {
                return BadRequest("Task not found");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != task.UserId)
            {
                return Forbid();
            }

            try
            {
                var savedAttachment = await _attachmentRepository.AddAttachmentAsync(taskId, file);
                if (savedAttachment == null)
                {
                    return BadRequest("Fail to upload attachment");
                }
                return Ok(savedAttachment);
            } catch(Exception)
            {
                return BadRequest("Fail to upload attachment");
            }
        }

        [Authorize]
        [HttpPost("upload-multiple/{taskId}")]
        public async Task<IActionResult> UploadMultipleAttachments(int taskId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("No files uploaded.");
            }

            var task = await _taskRepository.GetById(taskId);
            if (task == null)
            {
                return BadRequest("Task not found");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != task.UserId)
            {
                return Forbid();
            }

            try
            {
                foreach (var file in files)
                {
                    await _attachmentRepository.AddAttachmentAsync(taskId, file);
                }
            } catch(Exception)
            {
                return BadRequest("Fail to upload attachments");
            }

            return Ok(new { message = "Files uploaded successfully!"});
        }

        [Authorize]
        [HttpDelete("{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(int attachmentId)
        {
            var attachment = await _attachmentRepository.DeleteAttachmentAsync(attachmentId);
            if (!attachment) return NotFound("Attachment not found.");

            return Ok("Attachment deleted successfully.");
        }
    }
}
