using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskAppAPI.Data;
using TaskAppAPI.Models;

namespace TaskAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get()
        {
            return Ok(_context.Projects.Include(x => x.Tasks).ToList());
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
            return Ok(project);
        }
    }
}