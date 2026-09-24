using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class DeliveriesPage : ContentPage
{
    private readonly DeliveriesViewModel _viewModel;

    public DeliveriesPage(DeliveriesViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadDeliveries();
    }
}