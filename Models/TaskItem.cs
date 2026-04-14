using System.Text.Json.Serialization;
using TaskAppAPI.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "";
    public int ProjectId { get; set; }

    [JsonIgnore]
    public Project? Project { get; set; }
}