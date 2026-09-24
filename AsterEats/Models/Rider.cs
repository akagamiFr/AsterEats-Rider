namespace AsterEats.Models;

/// <summary>
/// Represents the logged-in delivery rider.
/// IsOnline is the one field that changes while the app is running (the
/// rider toggles it from the dashboard), so this class raises
/// PropertyChanged instead of being a plain data-only record.
/// </summary>
public class Rider : ObservableObject
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Phone { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string VehicleType { get; set; } = string.Empty;
	public string VehicleNumber { get; set; } = string.Empty;
	public double Rating { get; set; }
	public int CompletedDeliveries { get; set; }

	private bool _isOnline;
	public bool IsOnline
	{
		get => _isOnline;
		set
		{
			if (SetProperty(ref _isOnline, value))
			{
				// These are derived purely from IsOnline, so let bindings
				// that use them refresh too.
				OnPropertyChanged(nameof(StatusText));
			}
		}
	}

	/// <summary>Human readable status used directly in bindings (e.g. "Online" / "Offline").</summary>
	public string StatusText => IsOnline ? "Online" : "Offline";
}
