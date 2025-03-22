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
        public async Task<ActionResult<IEnumerable<TaskLabel>>> GetAll()
        {
            return Ok(await _taskLabelRepository.GetAll());
        }

        [Authorize]
        [HttpPost("assign")]
        public async Task<ActionResult<IEnumerable<TaskLabel>>> AssignTaskLabel([FromBody] TaskLabelDto taskLabelDto)
        {
            if (taskLabelDto == null)
            {
                return BadRequest(new { message = "Data cannot be null" });
            }
            var taskLabel = _mapper.Map<TaskLabel>(taskLabelDto);

            var task = await _taskRepository.GetById(taskLabel.TaskId);
            if (task == null)
            {
                return NotFound(new { message = "Task not found!" });
            }

            var label = await _labelRepository.GetById(taskLabel.LabelId);
            if (label == null)
            {
                return NotFound(new { message = "Label not found!" });
            }

            var existingTaskLabel = _taskLabelRepository.Any(tl => tl.TaskId == taskLabel.TaskId && tl.LabelId == taskLabel.LabelId);

            if (existingTaskLabel)
            {
                return Conflict(new { message = "Label is already assigned to this task." });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != task.UserId)
            {
                return Forbid();
            }

            try
            {
                await _taskLabelRepository.Add(taskLabel);
                return Ok(new { message = "Label assigned to task successfully!" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Error assigning label", error = e.Message });
            }
        }

        [Authorize]
        [HttpDelete("remove/{taskId}/{labelId}")]
        public async Task<ActionResult> RemoveLabel(int taskId, int labelId)
        {
            var taskLabels = await _taskLabelRepository.GetAll();
            var taskLabel = taskLabels.FirstOrDefault(tl => tl.TaskId == taskId && tl.LabelId == labelId);

            if (taskLabel == null)
            {
                return NotFound(new { message = "Task label not found." });
            }

            var task = await _taskRepository.GetById(taskId);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != task.UserId)
            {
                return Forbid();
            }

            try
            {
                await _taskLabelRepository.Delete(taskLabel);
                return Ok(new { message = "Label removed successfully!" });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { message = "Error removing label", error = e.Message });
            }
        }
    }
}
