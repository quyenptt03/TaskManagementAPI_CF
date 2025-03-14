using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.Interfaces;
using TaskManagement.Models;
using TaskManagement.DTOs;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/task-comments")]
    public class TaskCommentController : Controller
    {
        private readonly IGenericRepository<TaskComment> _repository;
        private readonly IGenericRepository<Models.Task> _taskRepository;
        private readonly IMapper _mapper;

        public TaskCommentController(IGenericRepository<TaskComment> repository, IGenericRepository<Models.Task> taskRepository, IMapper mapper)
        {
            _repository = repository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetTaskComments()
        {
            var result = await _repository.GetAll();
            return Ok(result);
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> AddCommentToTask([FromBody] TaskCommentDto commentDto)
        {
            if (commentDto == null)
            {
                return BadRequest(new { message = "Task comment cannot be null" });
            }

            var comment = _mapper.Map<TaskComment>(commentDto);

            var task = await _taskRepository.GetById(comment.TaskId);
            if (task == null)
            {
                return BadRequest(new { message = "Task not found" });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            comment.UserId = userId;

            if (task.UserId != userId)
            {
                return Forbid();
            }
            else
            {
                try
                {
                    await _repository.Add(comment);
                    return Ok(comment);
                }
                catch (Exception e)
                {
                    return BadRequest(new { message = e.Message });
                }
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteComment([FromRoute] int id)
        {
            var comment = await _repository.GetById(id);
            if (comment == null)
            {
                return NotFound(new { message = "Comment not found!!!!!" });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != comment.UserId)
            {
                return Forbid();
            }

            try
            {
                await _repository.Delete(id);
                return Ok(new { message = "Comment Deleted Successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }

        }
    }
}
