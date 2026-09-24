namespace AsterEats.Models;

/// <summary>
/// The lifecycle states a delivery moves through. Modeled as an enum
/// (rather than free-form strings) so the Active Delivery state machine in
/// Phase 2 can switch on it safely.
/// </summary>
public enum DeliveryStatus
{
	Requested,
	Accepted,
	Preparing,
	ReadyForPickup,
	PickedUp,
	OnTheWay,
	Arrived,
	Delivered,
	Rejected
}

/// <summary>
/// Represents one delivery job: pickup restaurant, drop-off customer,
/// money involved, and current status. This is the object every delivery
/// related screen (Request, Order Details, Active Delivery, History)
/// binds to - none of these screens hardcode order text directly.
/// </summary>
public class DeliveryOrder : ObservableObject
{
	public int Id { get; set; }

	public Restaurant Restaurant { get; set; } = new();

	public string CustomerName { get; set; } = string.Empty;
	public string CustomerPhone { get; set; } = string.Empty;
	public string DeliveryAddress { get; set; } = string.Empty;

	public List<OrderItem> Items { get; set; } = new();

	public decimal OrderAmount { get; set; }
	public decimal DeliveryFee { get; set; }
	public string PaymentMethod { get; set; } = string.Empty;
	public double DistanceKm { get; set; }

	private DeliveryStatus _status = DeliveryStatus.Requested;
	public DeliveryStatus Status
	{
		get => _status;
		set
		{
			if (SetProperty(ref _status, value))
				OnPropertyChanged(nameof(StatusText));
		}
	}

	/// <summary>Friendly label for the current status, used directly in bindings.</summary>
	public string StatusText => Status switch
	{
		DeliveryStatus.Requested => "New Request",
		DeliveryStatus.Accepted => "Accepted",
		DeliveryStatus.Preparing => "Preparing",
		DeliveryStatus.ReadyForPickup => "Ready for Pickup",
		DeliveryStatus.PickedUp => "Picked Up",
		DeliveryStatus.OnTheWay => "On the Way",
		DeliveryStatus.Arrived => "Arrived",
		DeliveryStatus.Delivered => "Delivered",
		DeliveryStatus.Rejected => "Rejected",
		_ => Status.ToString()
	};

	/// <summary>When this delivery was completed (used for grouping into History/Earnings). Null until Delivered.</summary>
	public DateTime? CompletedAtUtc { get; set; }
}
