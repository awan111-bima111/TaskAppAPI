using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Data;
using TaskAppAPI.Models;

namespace TaskAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TaskController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL + INCLUDE PROJECT
        [HttpGet]
        public IActionResult Get()
        {
            var data = _context.TaskItems
                .Include(t => t.Project)
                .ToList();

            return Ok(data);
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var data = _context.TaskItems
                .Include(t => t.Project)
                .FirstOrDefault(t => t.Id == id);

            if (data == null)
                return NotFound("Task tidak ditemukan");

            return Ok(data);
        }

        // ✅ CREATE
        [HttpPost]
        public IActionResult Create([FromBody] TaskItem task)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 🔥 VALIDASI PROJECT
            var projectExists = _context.Projects.Any(p => p.Id == task.ProjectId);
            if (!projectExists)
                return BadRequest("Project tidak ditemukan");

            _context.TaskItems.Add(task);
            _context.SaveChanges();

            return Ok(task);
        }

        // ✅ UPDATE FULL
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TaskItem request)
        {
            var data = _context.TaskItems.Find(id);

            if (data == null)
                return NotFound("Task tidak ditemukan");

            var projectExists = _context.Projects.Any(p => p.Id == request.ProjectId);
            if (!projectExists)
                return BadRequest("Project tidak ditemukan");

            data.Title = request.Title;
            data.Status = request.Status;
            data.ProjectId = request.ProjectId;

            _context.SaveChanges();

            return Ok(data);
        }

        // ✅ UPDATE STATUS (DTO BIAR AMAN)
        [HttpPatch("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
        {
            var data = _context.TaskItems.Find(id);

            if (data == null)
                return NotFound("Task tidak ditemukan");

            data.Status = dto.Status;
            _context.SaveChanges();

            return Ok(data);
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var data = _context.TaskItems.Find(id);

            if (data == null)
                return NotFound("Task tidak ditemukan");

            _context.TaskItems.Remove(data);
            _context.SaveChanges();

            return Ok("Task berhasil dihapus");
        }
    }

    // 🔥 DTO BIAR PATCH GAK ERROR
    public class UpdateStatusDto
    {
        public string Status { get; set; } = "";
    }
}