using AsterEats.Services;
using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;
    private readonly MockDataService _dataService;

    private CancellationTokenSource? _titleAnimationCts;

    public HomePage(
        HomeViewModel viewModel,
        MockDataService dataService)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;
        _dataService = dataService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.RefreshCommand.Execute(null);

        if (!_viewModel.HasActiveDelivery)
        {
            DeliveryRequestOverlay.IsVisible = false;
        }

        // Start invisible
        HomeRoot.Opacity = 0;

        UpdateOnlineAnimation();
        StartTitleAnimation();

        // Smooth fade-in
        await HomeRoot.FadeTo(
            1,
            500,
            Easing.CubicInOut);
    }

    protected override void OnDisappearing()
    {
        // Stop title animation when leaving Home page

        base.OnDisappearing();
    }


    // =========================================================
    // AsterEats Title Animation
    // =========================================================

    private void StartTitleAnimation()
    {
        // Make sure an old animation is not still running
        StopTitleAnimation();

        _titleAnimationCts = new CancellationTokenSource();

        _ = RunTitleAnimationAsync(_titleAnimationCts.Token);
    }

    private void StopTitleAnimation()
    {
        if (_titleAnimationCts != null)
        {
            _titleAnimationCts.Cancel();
            _titleAnimationCts.Dispose();
            _titleAnimationCts = null;
        }

        // Reset title/cursor
        EatsTitle.Text = string.Empty;

        TypingCursor.CancelAnimations();
        TypingCursor.Opacity = 0;
    }

    private async Task RunTitleAnimationAsync(CancellationToken token)
    {
        const string fullText = "Eats";

        try
        {
            while (!token.IsCancellationRequested)
            {
                // Cursor appears
                await TypingCursor.FadeTo(
                    1,
                    200,
                    Easing.CubicOut);

                token.ThrowIfCancellationRequested();


                // -------------------------------------------------
                // Type: E → Ea → Eat → Eats
                // -------------------------------------------------

                for (int i = 1; i <= fullText.Length; i++)
                {
                    token.ThrowIfCancellationRequested();

                    EatsTitle.Text = fullText.Substring(0, i);

                    await Task.Delay(180, token);
                }


                // -------------------------------------------------
                // Blink cursor while text stays visible
                // -------------------------------------------------

                for (int i = 0; i < 3; i++)
                {
                    token.ThrowIfCancellationRequested();

                    await TypingCursor.FadeTo(
                        0,
                        300,
                        Easing.CubicInOut);

                    token.ThrowIfCancellationRequested();

                    await TypingCursor.FadeTo(
                        1,
                        300,
                        Easing.CubicInOut);
                }


                // Small pause
                await Task.Delay(500, token);


                // -------------------------------------------------
                // Remove text
                // -------------------------------------------------

                for (int i = fullText.Length - 1; i >= 0; i--)
                {
                    token.ThrowIfCancellationRequested();

                    EatsTitle.Text = fullText.Substring(0, i);

                    await Task.Delay(140, token);
                }


                // Hide cursor
                await TypingCursor.FadeTo(
                    0,
                    200,
                    Easing.CubicOut);


                // Pause before starting again
                await Task.Delay(400, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Animation was intentionally stopped.
        }
    }


    // =========================================================
    // Online Button Animation
    // =========================================================

    private async void OnOnlineButtonTapped(
        object sender,
        TappedEventArgs e)
    {
        // Button press animation
        await OnlineButton.ScaleTo(
            0.92,
            80,
            Easing.CubicOut);

        await OnlineButton.ScaleTo(
            1.0,
            100,
            Easing.CubicIn);


        // Toggle Online / Offline
        if (_viewModel.ToggleOnlineCommand.CanExecute(null))
        {
            _viewModel.ToggleOnlineCommand.Execute(null);
        }

        await Task.Delay(100);


        // Start/stop pulse depending on current status
        UpdateOnlineAnimation();
    }


    private void UpdateOnlineAnimation()
    {
        if (_viewModel.Rider.IsOnline)
        {
            StartPulseAnimation();
        }
        else
        {
            StopPulseAnimation();
        }
    }


    private async void StartPulseAnimation()
    {
        PulseRing.CancelAnimations();

        PulseRing.Scale = 0.85;
        PulseRing.Opacity = 0.35;

        await PulseRing.ScaleTo(
            1.15,
            900,
            Easing.CubicOut);

        await PulseRing.FadeTo(
            0.05,
            900,
            Easing.CubicOut);

        // Continue pulse while online
        if (_viewModel.Rider.IsOnline)
        {
            StartPulseAnimation();
        }
    }


    private void StopPulseAnimation()
    {
        PulseRing.CancelAnimations();

        PulseRing.Scale = 0.85;
        PulseRing.Opacity = 0;
    }


    // =========================================================
    // Notifications
    // =========================================================

    private async void OnNotificationTapped(
        object sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("notifications");
    }


    // =========================================================
    // Current Delivery → Bottom Sheet
    // =========================================================

    private async void OnCurrentDeliveryTapped(
        object sender,
        TappedEventArgs e)
    {
        if (!_viewModel.HasActiveDelivery)
            return;

        if (sender is Frame frame)
        {
            await frame.ScaleTo(
                0.97,
                50,
                Easing.CubicOut);

            await frame.ScaleTo(
                1.0,
                50,
                Easing.CubicIn);
        }

        // Show Delivery Request as bottom sheet
        await DeliveryRequestOverlay.ShowAsync(
            _dataService);
    }
}