using AsterEats.Models;

namespace AsterEats.ViewModels;

/// <summary>
/// Common base for every ViewModel. Reuses the same ObservableObject that
/// models like Rider use, and adds a couple of properties every page can
/// use for showing a busy indicator or an inline error/status message.
/// </summary>
public abstract class BaseViewModel : ObservableObject
{
	private bool _isBusy;
	public bool IsBusy
	{
		get => _isBusy;
		set => SetProperty(ref _isBusy, value);
	}

	private string _statusMessage = string.Empty;
	public string StatusMessage
	{
		get => _statusMessage;
		set
		{
			if (SetProperty(ref _statusMessage, value))
				OnPropertyChanged(nameof(HasStatusMessage));
		}
	}

	/// <summary>Convenience flag for IsVisible bindings on an inline error/status label.</summary>
	public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);
}
