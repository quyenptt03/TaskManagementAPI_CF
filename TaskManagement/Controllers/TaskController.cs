using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.DTOs;
using TaskManagement.Interfaces;
using TaskManagement.Models;
using TaskManagement.Repositories;
using Task = TaskManagement.Models.Task;

namespace TaskManagement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks")]
    public class TaskController : Controller
    {
        private readonly IGenericRepository<Task> _repository;
        private readonly IGenericRepository<Category> _catRepository;
        private readonly IMapper _mapper;

        public TaskController(IGenericRepository<Task> repository, IMapper mapper, IGenericRepository<Category> catRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _catRepository = catRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TaskDto>> GetTasks()
        {
            var result = _repository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<TaskDto>> GetTaskById([FromRoute] int id)
        {
            var task = _repository.GetById(id);
            return task == null ? NotFound("Task not found") : Ok(task);
        }

        [Authorize]
        [HttpPost]
        public ActionResult<IEnumerable<TaskDto>> AddTask([FromBody] TaskDto taskDto)
        {
            var task = _mapper.Map<Task>(taskDto);

            if (task == null)
            {
                return BadRequest("Task cannot be null");
            }
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                return BadRequest("Task must have a title.");
            }

            var cat = _catRepository.GetById(task.CategoryId);
            if (cat == null)
            {
                return NotFound("Category not found!");
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                task.UserId = userId;
                _repository.Add(task);
                return Ok(task);
            }
            catch (Exception e)
            {
                return BadRequest(e);
                
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public ActionResult<IEnumerable<TaskDto>> UpdateTask(int id, [FromBody] TaskDto taskDto)
        {
            var taskExist = _repository.GetById(id);
            if (taskExist == null)
            {
                return NotFound("Task Not Found!!!!!!");
            }

            var cat = _catRepository.GetById(taskDto.CategoryId);
            if (cat == null)
            {
                return NotFound("Category not found!");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != taskExist.UserId)
            {
                return Forbid();
            }

            _mapper.Map(taskDto, taskExist);

            try
            {
                taskExist.Title = taskDto.Title;
                taskExist.Description = taskDto.Description;
                taskExist.CategoryId = taskDto.CategoryId;
                taskExist.IsCompleted = taskDto.IsCompleted;
                taskExist.UserId = userId;
                _repository.Update(taskExist);
                return Ok(taskExist);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult<IEnumerable<TaskDto>> DeleteTask([FromRoute] int id)
        {
            var task = _repository.GetById(id);
            if (task == null)
            {
                return NotFound("Task not found!!!!!");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != task.UserId)
            {
                return Forbid();
            }

            try
            {
                _repository.Delete(id);
                return Ok("Task Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
