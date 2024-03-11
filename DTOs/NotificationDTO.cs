namespace HaBuddies.Models;

public class NotificationDTO
{
    public required TypeStatus Type { get; set; }
    public required string UserId { get; set; }
    public string? FromUserId { get; set; }
    public required string EventId { get; set; }
    public required bool IsHost { get; set; }
    public required bool IsViewed { get; set; }
}