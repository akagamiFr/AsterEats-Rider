namespace AsterEats.Models;

/// <summary>Pickup point for a delivery - the restaurant the rider collects food from.</summary>
public class Restaurant
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Address { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
}
