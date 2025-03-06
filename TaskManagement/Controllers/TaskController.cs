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
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var result = await _repository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTaskById([FromRoute] int id)
        {
            var task = await _repository.GetById(id);
            return task == null ? NotFound("Task not found") : Ok(task);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TaskDto>>> AddTask([FromBody] TaskDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest("Data cannot be null");
            }
            var task = _mapper.Map<Task>(taskDto);

            if (task == null)
            {
                return BadRequest("Task cannot be null");
            }
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                return BadRequest("Task must have a title.");
            }

            var cat = await _catRepository.GetById(task.CategoryId);
            if (cat == null)
            {
                return BadRequest("Category not found!");
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                task.UserId = userId;
                await _repository.Add(task);
                return Ok(task);
            }
            catch (Exception e)
            {
                return BadRequest(e);
                
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> UpdateTask(int id, [FromBody] TaskDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest("Data cannot be null");
            }

            var taskExist = await _repository.GetById(id);
            if (taskExist == null)
            {
                return NotFound("Task Not Found!!!!!!");
            }

            var cat = await _catRepository.GetById(taskDto.CategoryId);
            if (cat == null)
            {
                return BadRequest("Category not found!");
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
                await _repository.Update(taskExist);
                return Ok(taskExist);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> DeleteTask([FromRoute] int id)
        {
            var task = await _repository.GetById(id);
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
                await _repository.Delete(id);
                return Ok("Task Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}
