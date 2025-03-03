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
        public ActionResult GetTaskComments()
        {
            var result = (List<TaskComment>)_repository.GetAll();
            return Ok(result);
        }


        [Authorize]
        [HttpPost]
        public ActionResult AddCommentToTask([FromBody] TaskCommentDto commentDto)
        {
            var comment = _mapper.Map<TaskComment>(commentDto);

            if (comment == null)
            {
                return BadRequest("Task comment cannot be null");
            }
            var task = _taskRepository.GetById(comment.TaskId);
            if (task == null)
            {
                return BadRequest("Task not found");
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
                    _repository.Add(comment);
                    return Ok(comment);
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteComment([FromRoute] int id)
        {
            var comment = _repository.GetById(id);
            if (comment == null)
            {
                return NotFound("Comment not found!!!!!");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != comment.UserId)
            {
                return Forbid();
            }

            try
            {
                _repository.Delete(id);
                return Ok("Comment Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
    }
}
