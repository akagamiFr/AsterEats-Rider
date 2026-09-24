using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class ActiveDeliveryPage : ContentPage
{
    public ActiveDeliveryPage(ActiveDeliveryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}