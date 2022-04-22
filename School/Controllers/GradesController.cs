#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using School.Entities;
using School.Entity_DTOs;
using School.Model;
using School.Search_Queries;

namespace School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private readonly SchoolDBContext _context;
        private readonly IMapper _mapper;

        public GradesController(SchoolDBContext context, IMapper mapper)
        {
            // Constructor Dependency Injection
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("grade/all")]
        public async Task<ActionResult<IEnumerable<GradeDTO>>> Grades()
        {
            var mappedGrades = _mapper.Map<IEnumerable<GradeDTO>>(_context.StudentGrades);
            return Ok(mappedGrades);
        }

        [HttpGet("grade/{id}")]
        public async Task<ActionResult<GradeDTO>> Grade(int id)
        {
            var grade = await _context.StudentGrades.FindAsync(id);

            if (grade == null)
            {
                return NotFound("'Grade' is not found!");
            }

            return _mapper.Map<GradeDTO>(grade);
        }

        [HttpGet("search/grade")]
        public async Task<ActionResult<IEnumerable<Grade>>> SearchStudentGrade([FromQuery] GradeSearchQuery searchQuery)
        {
            IQueryable<Grade> query = _context.StudentGrades;

            if (!string.IsNullOrEmpty(searchQuery.Name))
            {
                query = query.Where(sh => sh.Name == searchQuery.Name);
            }
            if (!string.IsNullOrEmpty(searchQuery.Letter))
            {
                query = query.Where(sh => sh.Letter == searchQuery.Letter);
            }

            return await query.ToListAsync();
        }

        [HttpPut("update/grade{id}")]
        public async Task<IActionResult> UpdateGrade(int id, [Bind("Id,Name,Letter")]GradeDTO grade)
        {
            if(ModelState.IsValid)
            {
                if (id != grade.Id)
                {
                    return BadRequest("'Id' fields are not matching!");
                }

                if (!GradeExists(grade.Id))
                {
                    return NotFound("Grade is not found!");
                }

                var updatedGrade = _mapper.Map<Grade>(grade);
                _context.Update(updatedGrade);

                try
                {
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                // in case of concurrent update operation for the same student
                catch (DbUpdateConcurrencyException)
                {
                    return BadRequest("Concurrent Update!");
                }
            }

            return BadRequest("Model is not valid!");
        }

        [HttpPost("new/grade")]
        public async Task<ActionResult<Grade>> PostGrade([Bind("Id,Name,Letter")] GradeDTO grade)
        {
            if(ModelState.IsValid)
            {
                _context.StudentGrades.Add(_mapper.Map<Grade>(grade));
                await _context.SaveChangesAsync();

                return CreatedAtAction("Grade", new { id = grade.Id }, grade);
            }

            return BadRequest("Model is not valid!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _context.StudentGrades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            _context.StudentGrades.Remove(grade);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GradeExists(int id)
        {
            return _context.StudentGrades.Any(e => e.Id == id);
        }
    }
}
