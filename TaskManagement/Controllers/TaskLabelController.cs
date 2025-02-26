using Microsoft.AspNetCore.Mvc;
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

        public TaskLabelController(IGenericRepository<Label> labelRepository,
                                    IGenericRepository<Models.Task> taskRepository,
                                    IGenericRepository<TaskLabel> repository)
        {
            _labelRepository = labelRepository;
            _taskRepository = taskRepository;
            _taskLabelRepository = repository;
        }

        [HttpGet]
        public ActionResult GetAll()
        {
            return Ok(_taskLabelRepository.GetAll());
        }

        [HttpPost("assign")]
        public ActionResult AssignTaskLabel([FromBody] TaskLabel taskLabel)
        {
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

        [HttpDelete("remove/{taskId}/{labelId}")]
        public ActionResult RemoveLabel(int taskId, int labelId)
        {
            var taskLabel = _taskLabelRepository
                .GetAll()
                .FirstOrDefault(tl => tl.TaskId == taskId && tl.LabelId == labelId);
            if (taskLabel == null)
            {
                return NotFound("Task label not found.");
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
