using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.DataAccess;
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
        private readonly IGenericRepository<TaskLabel> _taskLabelRepository;
        private readonly IMapper _mapper;
        protected readonly TaskManagementContext _context;

        public TaskController(IGenericRepository<Task> repository, 
            IMapper mapper, 
            IGenericRepository<Category> catRepository, 
            IGenericRepository<TaskLabel> taskLabelRepository,
            TaskManagementContext context            
            )
        {
            _repository = repository;
            _mapper = mapper;
            _catRepository = catRepository;
            _taskLabelRepository = taskLabelRepository;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasks()
        {
            var result = await _repository.GetAll();
            return Ok(result);
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksByUserId(
        int userId,
        [FromQuery] int? categoryId,
        [FromQuery] int? labelId,
        [FromQuery] bool? isCompleted)
        {
            //var tasks = await _repository.FindByCondition(t =>
            //            t.UserId == userId &&
            //            (!categoryId.HasValue || t.CategoryId == categoryId) &&
            //            (!isCompleted.HasValue || t.IsCompleted == isCompleted));

            //if (labelId.HasValue)
            //{
            //    tasks = tasks.Where(t => _taskLabelRepository
            //        .Any(tl => tl.TaskId == t.Id && tl.LabelId == labelId)).ToList();
            //}

            //return Ok(tasks);
            //var query = from task in _context.Tasks
            //            join category in _context.Categories on task.CategoryId equals category.Id
            //            join taskLabel in _context.TaskLabels on task.Id equals taskLabel.TaskId
            //            join label in _context.Labels on taskLabel.LabelId equals label.Id
            //            where task.UserId == userId
            //                  && (!categoryId.HasValue || task.CategoryId == categoryId.Value)
            //                  && (!isCompleted.HasValue || task.IsCompleted == isCompleted.Value)
            //                  && (!labelId.HasValue || taskLabel.LabelId == labelId.Value)
            //            select new
            //            {
            //                Task = task,
            //                Category = category,
            //                Label = label
            //            };

            var query = from task in _context.Tasks
                        join category in _context.Categories on task.CategoryId equals category.Id 

                        join taskLabel in _context.TaskLabels on task.Id equals taskLabel.TaskId into taskLabelGroup
                        from taskLabel in taskLabelGroup.DefaultIfEmpty() // LEFT JOIN để tránh null

                        join label in _context.Labels on taskLabel.LabelId equals label.Id into labelGroup
                        from label in labelGroup.DefaultIfEmpty() // LEFT JOIN để tránh null
                        where task.UserId == userId
                                          && (!categoryId.HasValue || task.CategoryId == categoryId.Value)
                                          && (!isCompleted.HasValue || task.IsCompleted == isCompleted.Value)
                                          && (!labelId.HasValue || taskLabel.LabelId == labelId.Value)
                        select new
                        {
                            Task = task,
                            Category = category,
                            Label = label
                        };

            var tasks = await query
                .GroupBy(t => new
                {
                    t.Task.Id,
                    t.Task.Title,
                    t.Task.Description,
                    t.Task.CreatedAt,
                    t.Task.IsCompleted,
                    t.Task.UserId,
                    t.Task.CategoryId,
                    CategoryName = t.Category.Name,
                    CategoryDescription = t.Category.Description
                })
                .Select(g => new
                {
                    Id = g.Key.Id,
                    g.Key.Title,
                    g.Key.Description,
                    g.Key.CreatedAt,
                    g.Key.IsCompleted,
                    g.Key.UserId,
                    g.Key.CategoryId,
                    Category = new
                    {
                        Id = g.Key.CategoryId,
                        Name = g.Key.CategoryName,
                        Description = g.Key.CategoryDescription
                    },
                    Labels = g.Select(x => new
                    {
                        Id = x.Label.Id,
                        Name = x.Label.Name
                    }).Distinct().ToList()
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTaskById([FromRoute] int id)
        {
            //var task = await _repository.GetById(id);
            //return task == null ? NotFound(new { message = "Task not found" }) : Ok(task);
            var query = from task in _context.Tasks
                        join category in _context.Categories on task.CategoryId equals category.Id into catGroup
                        from category in catGroup.DefaultIfEmpty() // Left Join để tránh lỗi null
                        join taskLabel in _context.TaskLabels on task.Id equals taskLabel.TaskId into taskLabelGroup
                        from taskLabel in taskLabelGroup.DefaultIfEmpty()
                        join label in _context.Labels on taskLabel.LabelId equals label.Id into labelGroup
                        from label in labelGroup.DefaultIfEmpty()
                        where task.Id == id
                        select new
                        {
                            Task = task,
                            Category = category,
                            Label = label
                        };

            var result = await query
                .GroupBy(t => new
                {
                    t.Task.Id,
                    t.Task.Title,
                    t.Task.Description,
                    t.Task.CreatedAt,
                    t.Task.IsCompleted,
                    t.Task.UserId,
                    t.Task.CategoryId,
                    CategoryName = t.Category != null ? t.Category.Name : null,
                    CategoryDescription = t.Category != null ? t.Category.Description : null
                })
                .Select(g => new
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    Description = g.Key.Description,
                    CreatedAt = g.Key.CreatedAt,
                    IsCompleted = g.Key.IsCompleted,
                    UserId = g.Key.UserId,
                    CategoryId = g.Key.CategoryId,
                    Category = g.Key.CategoryId != null ? new
                    {
                        Id = g.Key.CategoryId,
                        Name = g.Key.CategoryName,
                        Description = g.Key.CategoryDescription
                    } : null,
                    Labels = g.Where(x => x.Label != null)
                             .Select(x => new
                             {
                                 Id = x.Label.Id,
                                 Name = x.Label.Name
                             })
                             .Distinct()
                             .ToList()
                })
                .FirstOrDefaultAsync();

            return result == null ? NotFound(new { message = "Task not found" }) : Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<TaskDto>>> AddTask([FromBody] TaskDto taskDto)
        {
            if (taskDto == null)
            {
                return BadRequest(new { message = "Data cannot be null" });
            }
            var task = _mapper.Map<Task>(taskDto);

            if (task == null)
            {
                return BadRequest(new { message = "Task cannot be null" });
            }
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                return BadRequest(new { message = "Task must have a title." });
            }

            var cat = await _catRepository.GetById(task.CategoryId);
            if (cat == null)
            {
                return BadRequest(new { message = "Category not found!" });
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
                return BadRequest(new { message = "Data cannot be null" });
            }

            var taskExist = await _repository.GetById(id);
            if (taskExist == null)
            {
                return NotFound(new { message = "Task Not Found!!!!!!" });
            }

            var cat = await _catRepository.GetById(taskDto.CategoryId);
            if (cat == null)
            {
                return BadRequest(new { message = "Category not found!" });
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
                return NotFound(new { message = "Task not found!!!!!" });
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            if (userId != task.UserId)
            {
                return Forbid();
            }

            try
            {
                await _repository.Delete(id);
                return Ok(new { message = "Task Deleted Successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
