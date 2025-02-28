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
        public ActionResult<IEnumerable<CategoryDto>> GetCategories()
        {
            List<Category> result = (List<Category>)_repository.GetAll();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<IEnumerable<CategoryDto>> Get([FromRoute] int id)
        {
            Category cat = _repository.GetById(id);
            return cat == null ? NotFound("Category not found") : Ok(cat);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<IEnumerable<CategoryDto>> AddCategory([FromBody] CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            if (category == null)
            {
                return BadRequest("Category is required");
            }
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                return BadRequest("Category must have a name.");
            }
            else
            {
                try
                {
                    if (_repository.Any(l => l.Name == category.Name))
                    {
                        return Conflict(new { message = "Name already exists." });
                    }
                    _repository.Add(category);
                    return Ok(category);
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public ActionResult<IEnumerable<CategoryDto>> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            var cat = _repository.GetById(id);
            if (cat == null)
            {
                return NotFound("Category Not Found!!!!!!");
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
                _repository.Update(cat);
                return Ok(cat);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public ActionResult<IEnumerable<CategoryDto>> DeleteCategory([FromRoute] int id)
        {
            var category = _repository.GetById(id);
            if (category == null)
            {
                return NotFound("Category not found!!!!!");
            }

            try
            {
                _repository.Delete(id);
                return Ok("Category Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
    }
}
