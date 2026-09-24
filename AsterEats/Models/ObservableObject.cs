using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AsterEats.Models;

/// <summary>
/// Small shared base class that implements INotifyPropertyChanged.
/// Both models (like Rider, whose IsOnline flag changes at runtime) and
/// ViewModels inherit from this so the UI updates automatically whenever
/// a bound property changes - without needing an extra NuGet package.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value))
			return false;

		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}

	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}
