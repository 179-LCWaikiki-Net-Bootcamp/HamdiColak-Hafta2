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
    public class StudentsController : ControllerBase
    {
        private readonly SchoolDBContext _context;
        private readonly IMapper _mapper;
        public StudentsController(SchoolDBContext context, IMapper mapper)
        {
            // Constructor Dependency Injection
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("student/all")]
        public async Task<ActionResult<IEnumerable<StudentDTO>>> Students()
        {
            var mappedStudents = _mapper.Map<IEnumerable<StudentDTO>>(_context.Students);
            return Ok(mappedStudents);
        }

        [HttpGet("student/{id}")]
        public async Task<ActionResult<StudentDTO>> Student(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound("Student is not found!");
            }

            return _mapper.Map<StudentDTO>(student);
        }

        [HttpGet("search/student")]
        public async Task<ActionResult<IEnumerable<Student>>> SearchStudent([FromQuery] StudentSearchQuery searchQuery)
        {
            IQueryable<Student> query = _context.Students;

            if (!string.IsNullOrEmpty(searchQuery.Name))
            {
                query = query.Where(sh => sh.Name == searchQuery.Name);
            }
            if (!string.IsNullOrEmpty(searchQuery.LastName))
            {
                query = query.Where(sh => sh.LastName == searchQuery.LastName);
            }
            if (!string.IsNullOrEmpty(searchQuery.FatherName))
            {
                query = query.Where(sh => sh.FatherName == searchQuery.FatherName);
            }
            if (searchQuery.Age != 0)
            {
                query = query.Where(sh => sh.Age == searchQuery.Age);
            }

            return await query.ToListAsync();
        }

        [HttpPut("update/student/{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [Bind("Id,Name,LastName,Age,DadName,GradeId")] StudentDTO student)
        {
            if (ModelState.IsValid)
            {
                if (id != student.Id)
                {
                    return BadRequest("'Id' fields are not matching!");
                }

                if(!StudentExists(student.Id))
                {
                    return NotFound("Student is not found!");
                }

                // Since 'GradeId' is a Foreing Key, We check if 'GradeId' exists in the 'StudentGrades'
                if(!_context.StudentGrades.Any(g => g.Id == student.GradeId))
                {
                    return BadRequest("'GradeId' does not exist in the 'StudentGrades' table!");
                }

                var updatedStudent = _mapper.Map<Student>(student);
                _context.Update(updatedStudent);

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
            return BadRequest("Model is not valid");
        }

        [HttpPost("new/student")]
        public async Task<ActionResult<Student>> NewStudent([Bind("Id,Name,LastName,Age,DadName,GradeId")] StudentDTO student)
        {
            if(ModelState.IsValid)
            {
                // 'GradeId' is a Foreing Key, We check if 'GradeId' exists in the 'StudentGrades'
                // Since there is a '1 to many' relation between 'Studens' and 'StudentGrades' tables 
                // we can use the same 'GradeId' for different students
                if (!_context.StudentGrades.Any(g => g.Id == student.GradeId))
                {
                    return BadRequest("'GradeId' does not exist in the 'StudentGrades' table!");
                }

                _context.Students.Add(_mapper.Map<Student>(student));
                await _context.SaveChangesAsync();

                return CreatedAtAction("Student", new { id = student.Id }, student);
            }
            return BadRequest("ModelState is not valid!");
        }

        [HttpDelete("delete/student/{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound("Student is not found!");
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
}
