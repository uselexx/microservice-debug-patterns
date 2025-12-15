namespace backend.Models;

using System.Text.Json.Serialization;

public class Widget
{
    public int Id { get; set; }
    public int DashboardId { get; set; }
    public string Title { get; set; } = null!;
    public string? Content { get; set; }
    public int? DisplayOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Foreign key
    [JsonIgnore]
    public Dashboard Dashboard { get; set; } = null!;
}
