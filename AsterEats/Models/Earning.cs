namespace AsterEats.Models;

/// <summary>
/// A single earnings record, created once a DeliveryOrder is completed.
/// The Earnings and Home dashboards summarize a list of these rather than
/// storing separate "today's total" fields anywhere.
/// </summary>
public class Earning
{
	public int Id { get; set; }
	public int DeliveryOrderId { get; set; }
	public string RestaurantName { get; set; } = string.Empty;
	public string AreaLabel { get; set; } = string.Empty;
	public decimal Amount { get; set; }
	public DateTime DateUtc { get; set; }
}
