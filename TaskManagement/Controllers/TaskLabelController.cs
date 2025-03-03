using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.DTOs;
using TaskManagement.Interfaces;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/task-labels")]
    public class TaskLabelController : Controller
    {
        private readonly IGenericRepository<Label> _labelRepository;
        private readonly IGenericRepository<Models.Task> _taskRepository;
        private readonly IGenericRepository<TaskLabel> _taskLabelRepository;
        private readonly IMapper _mapper;

        public TaskLabelController(IGenericRepository<Label> labelRepository,
                                    IGenericRepository<Models.Task> taskRepository,
                                    IGenericRepository<TaskLabel> repository,
                                    IMapper mapper)
        {
            _labelRepository = labelRepository;
            _taskRepository = taskRepository;
            _taskLabelRepository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TaskLabel>> GetAll()
        {
            return Ok(_taskLabelRepository.GetAll());
        }

        [Authorize]
        [HttpPost("assign")]
        public ActionResult<IEnumerable<TaskLabel>> AssignTaskLabel([FromBody] TaskLabelDto taskLabelDto)
        {
            var taskLabel = _mapper.Map<TaskLabel>(taskLabelDto);

            if (taskLabel == null)
            {
                return BadRequest("Task label is required");
            }
            var task = _taskRepository.GetById(taskLabel.TaskId);
            if (task == null)
            {
                return NotFound("Task not found!");
            }

            var label = _labelRepository.GetById(taskLabel.LabelId);
            if (label == null)
            {
                return NotFound("Label not found!");
            }

            var existingTaskLabel = _taskLabelRepository.Any(tl => tl.TaskId == taskLabel.TaskId && tl.LabelId == taskLabel.LabelId);

            if (existingTaskLabel)
            {
                return Conflict("Label is already assigned to this task.");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != task.UserId)
            {
                return Forbid();
            }

            try
            {
                _taskLabelRepository.Add(taskLabel);
                return Ok("Label assigned to task successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Error assigning label", error = e.Message });
            }
        }

        [Authorize]
        [HttpDelete("remove/{taskId}/{labelId}")]
        public ActionResult<IEnumerable<TaskLabel>> RemoveLabel(int taskId, int labelId)
        {
            var taskLabel = _taskLabelRepository
                .GetAll()
                .FirstOrDefault(tl => tl.TaskId == taskId && tl.LabelId == labelId);
            if (taskLabel == null)
            {
                return NotFound("Task label not found.");
            }

            var task = _taskRepository.GetById(taskId);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != task.UserId)
            {
                return Forbid();
            }

            try
            {
                _taskLabelRepository.Delete(taskLabel);
                return Ok("Label removed successfully!");
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Error removing label", error = e.Message });
            }
        }
    }
}
