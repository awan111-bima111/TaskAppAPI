using System.Collections.Generic;

namespace TaskAppAPI.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        // Relasi ke Task
        public List<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}