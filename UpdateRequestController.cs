using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NADRASystem.Data;
using NADRASystem.Models;
using System.Security.Claims;

namespace NADRASystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UpdateRequestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UpdateRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/updaterequest
        [HttpPost]
        [Authorize(Roles = "DepartmentOfficer")]
        public async Task<ActionResult<CitizenUpdateRequest>> CreateUpdateRequest(CitizenUpdateRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || user.DepartmentId == null)
                return Unauthorized();

            // Check if citizen exists
            var citizen = await _context.Citizens.FindAsync(request.CitizenId);
            if (citizen == null)
                return NotFound("Citizen not found");

            // Get current value of the field
            string oldValue = GetFieldValue(citizen, request.RequestedField);
            request.OldValue = oldValue;
            request.RequestedByDepartmentId = user.DepartmentId.Value;
            request.RequestedDate = DateTime.UtcNow;
            request.Status = "Pending";

            // Cannot request to change CNIC or DateOfBirth
            if (request.RequestedField == "CNIC" || request.RequestedField == "DateOfBirth")
                return BadRequest($"Cannot request changes to {request.RequestedField}");

            _context.CitizenUpdateRequests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUpdateRequest", new { id = request.RequestId }, request);
        }

        // GET: api/updaterequest/pending
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<CitizenUpdateRequest>>> GetPendingRequests()
        {
            var requests = await _context.CitizenUpdateRequests
                .Include(r => r.Citizen)
                .Include(r => r.Department)
                .Where(r => r.Status == "Pending")
                .ToListAsync();

            return Ok(requests);
        }

        // PUT: api/updaterequest/{id}/approve
        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRequest(int id)
        {
            var request = await _context.CitizenUpdateRequests
                .Include(r => r.Citizen)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
                return BadRequest("Request is not pending");

            // Update citizen record
            var citizen = request.Citizen;
            UpdateCitizenField(citizen, request.RequestedField, request.NewValue);

            request.Status = "Approved";

            _context.Entry(citizen).State = EntityState.Modified;
            _context.Entry(request).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/updaterequest/{id}/reject
        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var request = await _context.CitizenUpdateRequests.FindAsync(id);
            if (request == null)
                return NotFound();

            if (request.Status != "Pending")
                return BadRequest("Request is not pending");

            request.Status = "Rejected";
            _context.Entry(request).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string GetFieldValue(Citizen citizen, string fieldName)
        {
            return fieldName switch
            {
                "FullName" => citizen.FullName,
                "FatherName" => citizen.FatherName,
                "Gender" => citizen.Gender,
                "Address" => citizen.Address,
                "MaritalStatus" => citizen.MaritalStatus,
                "Nationality" => citizen.Nationality,
                "IsAlive" => citizen.IsAlive.ToString(),
                _ => string.Empty
            };
        }

        private void UpdateCitizenField(Citizen citizen, string fieldName, string newValue)
        {
            switch (fieldName)
            {
                case "FullName":
                    citizen.FullName = newValue;
                    break;
                case "FatherName":
                    citizen.FatherName = newValue;
                    break;
                case "Gender":
                    citizen.Gender = newValue;
                    break;
                case "Address":
                    citizen.Address = newValue;
                    break;
                case "MaritalStatus":
                    citizen.MaritalStatus = newValue;
                    break;
                case "Nationality":
                    citizen.Nationality = newValue;
                    break;
                case "IsAlive":
                    citizen.IsAlive = bool.Parse(newValue);
                    break;
            }
        }
    }
}