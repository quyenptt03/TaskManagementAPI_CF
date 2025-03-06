using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.DTOs;
using TaskManagement.Interfaces;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("/api/labels")]
    public class LabelController : Controller
    {
        private readonly IGenericRepository<Label> _labelRepository;
        private readonly IMapper _mapper;

        public LabelController(IGenericRepository<Label> labelRepository, IMapper mapper)
        {
            _labelRepository = labelRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LabelDto>>> GetLabels()
        {
            try
            {
                var result = await _labelRepository.GetAll();
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<LabelDto>>> GetLabelById([FromRoute] int id)
        {
            try
            {
                var label = await _labelRepository.GetById(id);
                return label == null ? NotFound("Label not found") : Ok(label);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<LabelDto>>> AddLabel([FromBody] LabelDto labelDto)
        {
            if (labelDto == null)
            {
                return BadRequest("Data cannot be null");
            }
            var label = _mapper.Map<Label>(labelDto);

            try
            {
                if (_labelRepository.Any(l => l.Name == label.Name))
                {
                    return Conflict(new { message = "Label's name already exists." });
                }
                await _labelRepository.Add(label);
                return Ok(label);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
           
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateLabel([FromRoute] int id, [FromBody] LabelDto labelDto)
        {
            var labelExists = await _labelRepository.GetById(id);
            if (labelExists == null)
            {
                return NotFound("Label Not Found!!!!!!");
            }

            if (labelDto == null)
            {
                return BadRequest("Data can not be null");
            }

            var label = _mapper.Map<Label>(labelDto);

            try
            {
                if (_labelRepository.Any(l => l.Id != id && l.Name == label.Name))
                {
                    return Conflict(new { message = "Label's name already exists." });
                }
                labelExists.Name = label.Name;
                await _labelRepository.Update(labelExists);
                return Ok(labelExists);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<IEnumerable<LabelDto>>> DeleteLabel([FromRoute] int id)
        {
            var label = await _labelRepository.GetById(id);
            if (label == null)
            {
                return NotFound("Label not found!!!!!");
            }

            try
            {
                await _labelRepository.Delete(id);
                return Ok("Label Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
    }
}
