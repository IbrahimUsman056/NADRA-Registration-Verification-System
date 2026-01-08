using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NADRASystem.Data;
using NADRASystem.DTOs;
using NADRASystem.Models;
using System.Security.Claims;

namespace NADRASystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CitizenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CitizenController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/citizen
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CitizenDTO>>> GetCitizens()
        {
            var citizens = await _context.Citizens
                .Select(c => new CitizenDTO
                {
                    CitizenId = c.CitizenId,
                    FullName = c.FullName,
                    CNIC = c.CNIC,
                    FatherName = c.FatherName,
                    DateOfBirth = c.DateOfBirth,
                    Gender = c.Gender,
                    Address = c.Address,
                    MaritalStatus = c.MaritalStatus,
                    Nationality = c.Nationality,
                    IsAlive = c.IsAlive
                })
                .ToListAsync();

            return Ok(citizens);
        }

        // GET: api/citizen/verify/{cnic}
        [HttpGet("verify/{cnic}")]
        [Authorize(Roles = "Admin,DepartmentOfficer")]
        public async Task<ActionResult<CitizenDTO>> VerifyCitizen(string cnic)
        {
            var citizen = await _context.Citizens
                .FirstOrDefaultAsync(c => c.CNIC == cnic);

            if (citizen == null)
                return NotFound("Citizen not found");

            var citizenDto = new CitizenDTO
            {
                CitizenId = citizen.CitizenId,
                FullName = citizen.FullName,
                CNIC = citizen.CNIC,
                FatherName = citizen.FatherName,
                DateOfBirth = citizen.DateOfBirth,
                Gender = citizen.Gender,
                Address = citizen.Address,
                MaritalStatus = citizen.MaritalStatus,
                Nationality = citizen.Nationality,
                IsAlive = citizen.IsAlive
            };

            return Ok(citizenDto);
        }

        // POST: api/citizen
        [HttpPost]
        [Authorize(Roles = "Admin,DepartmentOfficer")]
        public async Task<ActionResult<Citizen>> CreateCitizen(CitizenDTO citizenDto)
        {
            // Check if user is from Union Council (only Union Council can register new citizens)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            // If not admin, check if user is from Union Council
            if (!User.IsInRole("Admin") &&
                (user.Department == null || user.Department.DepartmentName != "Union Council"))
            {
                return Forbid("Only Union Council officers can register new citizens");
            }

            // Check if CNIC already exists
            if (await _context.Citizens.AnyAsync(c => c.CNIC == citizenDto.CNIC))
                return BadRequest("CNIC already exists");

            var citizen = new Citizen
            {
                FullName = citizenDto.FullName,
                CNIC = citizenDto.CNIC,
                FatherName = citizenDto.FatherName,
                DateOfBirth = citizenDto.DateOfBirth,
                Gender = citizenDto.Gender,
                Address = citizenDto.Address,
                MaritalStatus = citizenDto.MaritalStatus,
                Nationality = "Pakistani",
                IsAlive = true,
                CreatedDate = DateTime.UtcNow
            };

            _context.Citizens.Add(citizen);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(VerifyCitizen), new { cnic = citizen.CNIC }, citizen);
        }

        // PUT: api/citizen/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCitizen(int id, CitizenDTO citizenDto)
        {
            if (id != citizenDto.CitizenId)
                return BadRequest();

            var citizen = await _context.Citizens.FindAsync(id);
            if (citizen == null)
                return NotFound();

            // CNIC cannot be changed
            if (citizen.CNIC != citizenDto.CNIC)
                return BadRequest("CNIC cannot be modified");

            citizen.FullName = citizenDto.FullName;
            citizen.FatherName = citizenDto.FatherName;
            citizen.DateOfBirth = citizenDto.DateOfBirth;
            citizen.Gender = citizenDto.Gender;
            citizen.Address = citizenDto.Address;
            citizen.MaritalStatus = citizenDto.MaritalStatus;
            citizen.Nationality = citizenDto.Nationality;
            citizen.IsAlive = citizenDto.IsAlive;

            _context.Entry(citizen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CitizenExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        private bool CitizenExists(int id)
        {
            return _context.Citizens.Any(e => e.CitizenId == id);
        }
    }
}