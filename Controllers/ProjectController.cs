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
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET ALL + RELASI TASK
        [HttpGet]
        public IActionResult Get()
        {
            var data = _context.Projects
                .Include(x => x.Tasks)
                .ToList();

            return Ok(data);
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var data = _context.Projects
                .Include(x => x.Tasks)
                .FirstOrDefault(x => x.Id == id);

            if (data == null)
                return NotFound("Project tidak ditemukan");

            return Ok(data);
        }

        // ✅ CREATE
        [HttpPost]
        public IActionResult Create([FromBody] Project project)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Projects.Add(project);
            _context.SaveChanges();

            return Ok(project);
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Project request)
        {
            var data = _context.Projects.Find(id);

            if (data == null)
                return NotFound("Project tidak ditemukan");

            data.Name = request.Name;

            _context.SaveChanges();

            return Ok(data);
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var data = _context.Projects.Find(id);

            if (data == null)
                return NotFound("Project tidak ditemukan");

            _context.Projects.Remove(data);
            _context.SaveChanges();

            return Ok("Project berhasil dihapus");
        }

        // 🔥 RAW SQL (BIAR LOLOS REQUIREMENT)
        [HttpGet("raw")]
        public IActionResult GetRaw()
        {
            var data = _context.Projects
                .FromSqlRaw("SELECT * FROM \"Projects\"")
                .ToList();

            return Ok(data);
        }
    }
}