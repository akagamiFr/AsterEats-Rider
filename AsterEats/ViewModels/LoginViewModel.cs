
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace AsterEats.ViewModels;

/// <summary>
/// Backs the Login page.
/// This is a UI prototype, so authentication currently only checks
/// that a phone number has been entered.
/// Real OTP/API authentication can be added in a later phase.
/// </summary>
public class LoginViewModel : BaseViewModel
{
    private string _phoneOrEmail = string.Empty;

    public string PhoneOrEmail
    {
        get => _phoneOrEmail;
        set => SetProperty(ref _phoneOrEmail, value);
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel()
    {
        LoginCommand = new Command(async () => await LoginAsync());
    }

    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(PhoneOrEmail))
        {
            StatusMessage = "Please enter your phone number.";
            return;
        }

        StatusMessage = string.Empty;
        IsBusy = true;

        try
        {
            // Prototype delay to simulate a network request.
            // OTP/API authentication can be connected later.
            await Task.Delay(400);

            // Continue to the rider home screen.
            await Shell.Current.GoToAsync("//main/home");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

