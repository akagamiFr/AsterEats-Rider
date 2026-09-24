namespace AsterEats.Models;

/// <summary>In-app notification (e.g. "New delivery request", "Payment received"). Used from Phase 3 onward.</summary>
public class Notification
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Message { get; set; } = string.Empty;
	public DateTime DateUtc { get; set; }
	public bool IsRead { get; set; }
}
