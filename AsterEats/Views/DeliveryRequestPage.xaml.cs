using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class DeliveryRequestPage : ContentPage
{
    public DeliveryRequestPage(DeliveryRequestViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}