using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Interfaces;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : Controller
    {
        private readonly IGenericRepository<Category> _repository;

        public CategoryController(IGenericRepository<Category> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult GetCategories()
        {
            List<Category> result = (List<Category>)_repository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult Get([FromRoute] int id)
        {
            Category cat = _repository.GetById(id);
            return cat == null ? NotFound("Category not found") : Ok(cat);
        }

        [HttpPost]
        public ActionResult AddCategory([FromBody] Category category)
        {
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

        [HttpPut("{id}")]
        public ActionResult UpdateCategory(int id, [FromBody] Category category)
        {
            var cat = _repository.GetById(id);
            if (cat == null)
            {
                return NotFound("Category Not Found!!!!!!");
            }
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

        [HttpDelete("{id}")]
        public ActionResult DeleteCategory([FromRoute] int id)
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
