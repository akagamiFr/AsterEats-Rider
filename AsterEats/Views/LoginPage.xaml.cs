using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnGoogleLoginTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert(
            "Google Sign In",
            "Google sign-in will be connected in a later phase.",
            "OK");
    }
    private async void OnEmailLoginTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlert(
            "Email Sign In",
            "Email sign-in will be connected in a later phase.",
            "OK");
    }
}