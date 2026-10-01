using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class DeliveryRequestPage : ContentPage
{
    private double _startTranslationY;
    private double _currentTranslationY;

    // Up = 0 means the sheet cannot move upward
    private const double MaxUp = 0;

    // Maximum amount the sheet can move downward
    private const double MaxDown = 120;

    public DeliveryRequestPage(DeliveryRequestViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }

    private void OnBottomSheetPanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:

                _startTranslationY = _currentTranslationY;

                break;

            case GestureStatus.Running:

                double newY = _startTranslationY + e.TotalY;

                // Cannot move upward
                newY = Math.Max(MaxUp, newY);

                // Cannot move more than 120px downward
                newY = Math.Min(MaxDown, newY);

                _currentTranslationY = newY;

                BottomSheet.TranslationY = newY;

                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:

                ReturnToOriginalPosition();

                break;
        }
    }

    private async void ReturnToOriginalPosition()
    {
        _currentTranslationY = 0;

        await BottomSheet.TranslateTo(
            0,
            0,
            220,
            Easing.CubicOut);
    }
}