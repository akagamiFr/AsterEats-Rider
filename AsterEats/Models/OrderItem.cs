namespace AsterEats.Models;

/// <summary>A single food item within a DeliveryOrder (used on the Order Details screen).</summary>
public class OrderItem
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public int Quantity { get; set; }
	public decimal Price { get; set; }
}
