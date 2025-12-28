namespace InventoryService.Models.Entities;

public class SwipeEntity
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;

    public int MovieId { get; set; }

    public bool IsLiked { get; set; }

    public DateTime Timestamp { get; set; }
}