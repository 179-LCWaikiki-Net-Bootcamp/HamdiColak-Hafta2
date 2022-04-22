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
    public class AddressesController : ControllerBase
    {
        private readonly SchoolDBContext _context;
        private readonly IMapper _mapper;

        public AddressesController(SchoolDBContext context, IMapper mapper)
        {
            // Constructor Dependency Injection
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("studentAddress/all")]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> StudentAddresses()
        {
            var mappedStudenAddresses = _mapper.Map<IEnumerable<AddressDTO>>(_context.StudentAddresses);
            return Ok(mappedStudenAddresses);
        }

        [HttpGet("studentAddress/{id}")]
        public async Task<ActionResult<AddressDTO>> StudentAddress(int id)
        {
            var studentAddress = await _context.StudentAddresses.FindAsync(id);

            if (studentAddress == null)
            {
                return NotFound("'StudentAddress' is not found!");
            }

            return _mapper.Map<AddressDTO>(studentAddress);
        }

        [HttpGet("search/studentAddress")]
        public async Task<ActionResult<IEnumerable<Address>>> SearchStudentAddress([FromQuery] AddressSearchQuery searchQuery)
        {
            IQueryable<Address> query = _context.StudentAddresses;

            if (!string.IsNullOrEmpty(searchQuery.State))
            {
                query = query.Where(sh => sh.State == searchQuery.State);
            }
            if (!string.IsNullOrEmpty(searchQuery.Country))
            {
                query = query.Where(sh => sh.Country == searchQuery.Country);
            }
            if (searchQuery.ZipCode != 0)
            {
                query = query.Where(sh => sh.ZipCode == searchQuery.ZipCode);
            }

            return await query.ToListAsync();
        }

        [HttpPut("update/studentAddress/{id}")]
        public async Task<IActionResult> UpdateStudentAddress(int id, [Bind("Id,State,Country,ZipCode,StudentId")] AddressDTO studentAddress)
        {
            if(ModelState.IsValid)
            {
                if (id != studentAddress.Id)
                {
                    return BadRequest("'Id' fields are not matching!");
                }

                if (!AddressExists(studentAddress.Id))
                {
                    return NotFound("Address is not found!");
                }

                // 'StudentId' is a Foreing Key, We check if 'StudentId' exists in the 'Students' table
                // Since there is a '1 to 1' relation between 'Student' and 'StudentAddresses' table
                // we can NOT use the same 'StudentId' for the different addresses.
                // That's why we check 2 things
                // 1 - 'StudentId' must exist in the 'Student' table
                // 2 - 'StudentId' must not match any 'Address'
                if (!_context.Students.Any(s => s.Id == studentAddress.StudentId))
                {
                    return BadRequest("'StudentId' does not exist in the 'Students' table!");
                }
                else if(_context.StudentAddresses.Any(sa => sa.StudentId == studentAddress.StudentId))
                {
                    return BadRequest("'StudentId' is matching with one address in the 'StudentAddress' table!");
                }

                var updatedStudentAddress = _mapper.Map<Address>(studentAddress);
                _context.Update(updatedStudentAddress);

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

        [HttpPost("new/studentAddress")]
        public async Task<ActionResult<Address>> PostAddress([Bind("Id,State,Country,ZipCode,StudentId")] AddressDTO studentAddress)
        {
            if(ModelState.IsValid)
            {
                // 'StudentId' is a Foreing Key, We check if 'StudentId' exists in the 'Students' table
                // Since there is a '1 to 1' relation between 'Student' and 'StudentAddresses' table
                // we can NOT use the same 'StudentId' for the different addresses.
                // That's why we check 2 things
                // 1 - 'StudentId' must exist in the 'Student' table
                // 2 - 'StudentId' must not match any 'Address'
                if (!_context.Students.Any(s => s.Id == studentAddress.StudentId))
                {
                    return BadRequest("'StudentId' does not exist in the 'Students' table!");
                }
                else if(_context.StudentAddresses.Any(sa => sa.StudentId == studentAddress.StudentId))
                {
                    return BadRequest("'StudentId' is matching with one address in the 'StudentAddress' table!");
                }

                _context.StudentAddresses.Add(_mapper.Map<Address>(studentAddress));
                await _context.SaveChangesAsync();

                return CreatedAtAction("StudentAddress", new { id = studentAddress.Id }, studentAddress);
            }

            return BadRequest("Model is not valid!");
        }

        [HttpDelete("delete/studentAddress/{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.StudentAddresses.FindAsync(id);
            if (address == null)
            {
                return NotFound("Student Address is not found!");
            }

            _context.StudentAddresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _context.StudentAddresses.Any(e => e.Id == id);
        }
    }
}
