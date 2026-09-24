using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class OrderDetailsPage : ContentPage
{
    public OrderDetailsPage(OrderDetailsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//main/home");
    }
}