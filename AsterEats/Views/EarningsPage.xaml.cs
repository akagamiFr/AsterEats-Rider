using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class EarningsPage : ContentPage
{
    private readonly EarningsViewModel _viewModel;

    public EarningsPage(EarningsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadEarnings();
    }
}