using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.DTOs;
using TaskManagement.Interfaces;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : Controller
    {
        private readonly IGenericRepository<Category> _repository;
        private readonly IMapper _mapper;

        public CategoryController(IGenericRepository<Category> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetCategories()
        {
            var result = await _repository.GetAll();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> Get([FromRoute] int id)
        {
            Category cat = await _repository.GetById(id);
            return cat == null ? NotFound(new { message = "Category not found" }) : Ok(cat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> AddCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest(new { message = "Data cannot be null" });
            }

            var category = _mapper.Map<Category>(categoryDto);

            if (category == null)
            {
                return BadRequest(new { message = "Category is required" });
            }
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest(new { message = "Category must have a name." });
            }
            else
            {
                try
                {
                    if (_repository.Any(l => l.Name == category.Name))
                    {
                        return Conflict(new { message = "Name already exists." });
                    }
                    await _repository.Add(category);
                    return Ok(category);
                }
                catch (Exception e)
                {
                    return BadRequest(new { message = e.Message });
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            var cat = await _repository.GetById(id);
            if (cat == null)
            {
                return NotFound(new { message = "Category Not Found!!!!!!" });
            }

            if (categoryDto == null)
            {
                return BadRequest(new { message = "Data can not be null" });
            }

            var category = _mapper.Map<Category>(categoryDto);

            try
            {
                if (_repository.Any(c => c.Id != id && c.Name == category.Name))
                {
                    return Conflict(new { message = "Category's name already exists." });
                }
                cat.Name = category.Name;
                cat.Description = category.Description;
                await _repository.Update(cat);
                return Ok(cat);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> DeleteCategory([FromRoute] int id)
        {
            var category = await _repository.GetById(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found!!!!!" });
            }

            try
            {
                await _repository.Delete(id);
                return Ok(new { message = "Category Deleted Successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }

        }
    }
}
