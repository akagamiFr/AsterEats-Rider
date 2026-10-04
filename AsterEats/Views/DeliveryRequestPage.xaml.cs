using AsterEats.Services;
using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class DeliveryRequestPage : ContentView
{
    private double _startTranslationY;
    private double _currentTranslationY;

    private const double MaxDown = 140;
    private const double DismissThreshold = 90;

    private bool _isAnimating;

    public DeliveryRequestPage()
    {
        InitializeComponent();
    }

    public async Task ShowAsync(
        MockDataService dataService)
    {
        if (_isAnimating)
            return;

        var viewModel =
            new DeliveryRequestViewModel(dataService);

        BindingContext = viewModel;

        IsVisible = true;

        await Task.Yield();

        BottomSheet.TranslationY =
            Math.Max(Height, 700);

        _currentTranslationY =
            BottomSheet.TranslationY;

        _isAnimating = true;

        await BottomSheet.TranslateTo(
            0,
            0,
            240,
            Easing.CubicOut);

        _currentTranslationY = 0;

        _isAnimating = false;
    }

    public async Task HideAsync()
    {
        if (_isAnimating)
            return;

        _isAnimating = true;

        await BottomSheet.TranslateTo(
            0,
            Math.Max(Height, 700),
            200,
            Easing.CubicIn);

        _currentTranslationY =
            BottomSheet.TranslationY;

        IsVisible = false;

        _isAnimating = false;
    }

    private async void OnAcceptClicked(
        object? sender,
        EventArgs e)
    {
        if (BindingContext is not DeliveryRequestViewModel viewModel)
            return;

        if (viewModel.AcceptCommand.CanExecute(null))
        {
            viewModel.AcceptCommand.Execute(null);
        }
    }

    private async void OnRejectClicked(
        object? sender,
        EventArgs e)
    {
        if (BindingContext is not DeliveryRequestViewModel viewModel)
            return;

        if (viewModel.RejectCommand.CanExecute(null))
        {
            viewModel.RejectCommand.Execute(null);
        }

        await HideAsync();
    }

    private void OnBottomSheetPanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        if (_isAnimating)
            return;

        switch (e.StatusType)
        {
            case GestureStatus.Started:

                _startTranslationY =
                    _currentTranslationY;

                break;

            case GestureStatus.Running:

                double newY =
                    _startTranslationY + e.TotalY;

                newY =
                    Math.Max(0, newY);

                newY =
                    Math.Min(MaxDown, newY);

                _currentTranslationY =
                    newY;

                BottomSheet.TranslationY =
                    newY;

                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:

                if (_currentTranslationY >= DismissThreshold)
                {
                    _ = HideAsync();
                }
                else
                {
                    _ = ReturnToOriginalPositionAsync();
                }

                break;
        }
    }

    private async Task ReturnToOriginalPositionAsync()
    {
        if (_isAnimating)
            return;

        _isAnimating = true;

        await BottomSheet.TranslateTo(
            0,
            0,
            180,
            Easing.CubicOut);

        _currentTranslationY = 0;

        _isAnimating = false;
    }
}