
using AsterEats.Models;
using AsterEats.ViewModels;

namespace AsterEats.Views;

public partial class ActiveDeliveryPage : ContentPage
{
    private readonly ActiveDeliveryViewModel _viewModel;

    private double _startDrawerHeight;
    private double _currentDrawerHeight;

    private double _minimumDrawerHeight;
    private double _initialDrawerHeight;
    private double _maximumDrawerHeight;

    private bool _isDragging;
    private bool _isAnimating;

    private CancellationTokenSource? _pulseCancellation;


    public ActiveDeliveryPage(
        ActiveDeliveryViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = _viewModel = viewModel;

        BottomDrawer.SizeChanged +=
            OnBottomDrawerSizeChanged;

        _viewModel.Delivery.PropertyChanged +=
            OnDeliveryPropertyChanged;
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(80);

        CalculateDrawerHeights();

        UpdateProgress();

        StartLocationPulse();

        // Start in lower-half position.
        _currentDrawerHeight =
            _initialDrawerHeight;

        BottomDrawer.HeightRequest =
            _initialDrawerHeight;
    }


    protected override void OnDisappearing()
    {
        StopDrawerAnimation();

        StopLocationPulse();

        base.OnDisappearing();
    }


    // =========================================================
    // DELIVERY STATUS CHANGE
    // =========================================================

    private void OnDeliveryPropertyChanged(
        object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(DeliveryOrder.Status) &&
            e.PropertyName != nameof(DeliveryOrder.StatusText))
        {
            return;
        }

        MainThread.BeginInvokeOnMainThread(
            UpdateProgress);
    }


    // =========================================================
    // CUSTOMER CALL
    // =========================================================

    private async void OnCallCustomerTapped(
        object? sender,
        TappedEventArgs e)
    {
        try
        {
            string phone =
                _viewModel.Delivery.CustomerPhone;

            if (string.IsNullOrWhiteSpace(phone))
                return;

            await Launcher.Default.OpenAsync(
                $"tel:{phone}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Call customer error: {ex.Message}");
        }
    }


    // =========================================================
    // CUSTOMER MESSAGE
    // =========================================================

    private async void OnMessageCustomerTapped(
        object? sender,
        TappedEventArgs e)
    {
        try
        {
            string phone =
                _viewModel.Delivery.CustomerPhone;

            if (string.IsNullOrWhiteSpace(phone))
                return;

            await Launcher.Default.OpenAsync(
                $"sms:{phone}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Message customer error: {ex.Message}");
        }
    }


    // =========================================================
    // DRAWER HEIGHTS
    // =========================================================

  
private void CalculateDrawerHeights()
    {
        double screenHeight =
            DeviceDisplay.Current.MainDisplayInfo.Height /
            DeviceDisplay.Current.MainDisplayInfo.Density;


        // =========================================================
        // POSITION 3 - MAXIMUM
        //
        // Drawer will stop just below the "ON THE WAY" status box.
        // =========================================================

        // Top status box starts around 48px from the top.
        // Its height is roughly 35px.
        // Extra gap keeps drawer slightly below it.
        const double topClearance = 100;

        _maximumDrawerHeight =
            screenHeight - topClearance;


        // =========================================================
        // POSITION 2 - INITIAL
        //
        // Drawer starts in lower-middle position.
        // =========================================================

        _initialDrawerHeight =
            screenHeight * 0.55;


        // =========================================================
        // POSITION 1 - MINIMUM
        //
        // Keep the complete EST. ARRIVAL box visible.
        //
        // 175-185px is enough for:
        // Handle + Header + spacing + ETA card.
        // =========================================================

        _minimumDrawerHeight = 185;


        // =========================================================
        // SAFETY CHECKS
        // =========================================================

        if (_minimumDrawerHeight >
            _maximumDrawerHeight)
        {
            _minimumDrawerHeight =
                _maximumDrawerHeight;
        }


        if (_initialDrawerHeight <
            _minimumDrawerHeight)
        {
            _initialDrawerHeight =
                _minimumDrawerHeight;
        }


        if (_initialDrawerHeight >
            _maximumDrawerHeight)
        {
            _initialDrawerHeight =
                _maximumDrawerHeight;
        }
    }



    private void OnBottomDrawerSizeChanged(
        object? sender,
        EventArgs e)
    {
        if (BottomDrawer.Height <= 0)
            return;

        if (!_isDragging &&
            !_isAnimating)
        {
            _currentDrawerHeight =
                BottomDrawer.Height;
        }
    }


    // =========================================================
    // DRAWER SWIPE
    // =========================================================

    private void OnDrawerPanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        if (_isAnimating)
            return;


        switch (e.StatusType)
        {
            case GestureStatus.Started:

                _isDragging = true;

                StopDrawerAnimation();

                _startDrawerHeight =
                    _currentDrawerHeight;

                break;


            case GestureStatus.Running:

                if (!_isDragging)
                    return;


                /*
                 * Swipe up:
                 * TotalY becomes negative,
                 * therefore drawer height increases.
                 *
                 * Swipe down:
                 * TotalY becomes positive,
                 * therefore drawer height decreases.
                 */

                double newHeight =
                    _startDrawerHeight -
                    e.TotalY;


                // -------------------------------------------------
                // MAXIMUM
                //
                // Stop below ON THE WAY box.
                // -------------------------------------------------

                newHeight =
                    Math.Min(
                        _maximumDrawerHeight,
                        newHeight);


                // -------------------------------------------------
                // MINIMUM
                //
                // Keep EST. ARRIVAL box visible.
                // -------------------------------------------------

                newHeight =
                    Math.Max(
                        _minimumDrawerHeight,
                        newHeight);


                _currentDrawerHeight =
                    newHeight;

                BottomDrawer.HeightRequest =
                    newHeight;

                break;


            case GestureStatus.Completed:

                if (!_isDragging)
                    return;

                _isDragging = false;

                SnapDrawer();

                break;


            case GestureStatus.Canceled:

                _isDragging = false;

                SnapDrawer();

                break;
        }
    }


    // =========================================================
    // SNAP DRAWER
    // =========================================================

private void SnapDrawer()
    {
        // =========================================================
        // THREE SNAP POSITIONS
        //
        // 1. Minimum  = EST. ARRIVAL fully visible
        // 2. Initial  = lower-middle starting position
        // 3. Maximum  = just below ON THE WAY box
        // =========================================================

        double distanceToMinimum =
            Math.Abs(
                _currentDrawerHeight -
                _minimumDrawerHeight);


        double distanceToInitial =
            Math.Abs(
                _currentDrawerHeight -
                _initialDrawerHeight);


        double distanceToMaximum =
            Math.Abs(
                _currentDrawerHeight -
                _maximumDrawerHeight);


        double targetHeight;


        // Find the nearest of the three positions.

        if (distanceToMinimum <= distanceToInitial &&
            distanceToMinimum <= distanceToMaximum)
        {
            targetHeight =
                _minimumDrawerHeight;
        }
        else if (distanceToInitial <= distanceToMaximum)
        {
            targetHeight =
                _initialDrawerHeight;
        }
        else
        {
            targetHeight =
                _maximumDrawerHeight;
        }


        _ = AnimateDrawerToAsync(
            targetHeight);
    }



    // =========================================================
    // DRAWER ANIMATION
    // =========================================================

private async Task AnimateDrawerToAsync(
    double targetHeight)
    {
        if (_isAnimating)
            return;


        _isAnimating = true;


        double startHeight =
            _currentDrawerHeight;


        // Smooth and natural bottom-drawer animation
        const uint animationDuration = 380;


        var tcs =
            new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);


        BottomDrawer.AbortAnimation(
            "DrawerHeightAnimation");


        var animation =
            new Animation(
                callback:
                value =>
                {
                    double height =
                        startHeight +
                        ((targetHeight - startHeight) *
                         value);


                    BottomDrawer.HeightRequest =
                        height;
                },
                start: 0,
                end: 1,
                easing: Easing.SpringOut);


        animation.Commit(
            BottomDrawer,
            "DrawerHeightAnimation",
            16,
            animationDuration,
            finished:
            (value, cancelled) =>
            {
                tcs.TrySetResult(true);
            });


        await tcs.Task;


        BottomDrawer.HeightRequest =
            targetHeight;


        _currentDrawerHeight =
            targetHeight;


        _isAnimating = false;
    }




    private void StopDrawerAnimation()
    {
        BottomDrawer.AbortAnimation(
            "DrawerHeightAnimation");

        _isAnimating = false;
    }


    // =========================================================
    // DELIVERY PROGRESS
    // =========================================================

    private void UpdateProgress()
    {
        if (PickupStep == null ||
            OnWayStep == null ||
            DropoffStep == null ||
            PickupToOnWayConnector == null ||
            OnWayToDropoffConnector == null)
        {
            return;
        }


        string status =
            _viewModel.Delivery.StatusText;


        // Reset steps

        PickupStep.BackgroundColor =
            Color.FromArgb("#E4ECEC");

        OnWayStep.BackgroundColor =
            Color.FromArgb("#E4ECEC");

        DropoffStep.BackgroundColor =
            Color.FromArgb("#E4ECEC");


        // Reset connectors

        PickupToOnWayConnector.BackgroundColor =
            Color.FromArgb("#DDE5E5");

        OnWayToDropoffConnector.BackgroundColor =
            Color.FromArgb("#DDE5E5");


        // Reset labels

        if (PickupStep.Content is Label pickupLabel)
        {
            pickupLabel.Text = "1";

            pickupLabel.TextColor =
                Color.FromArgb("#6E7C7C");
        }


        if (OnWayStep.Content is Label onWayLabel)
        {
            onWayLabel.Text = "2";

            onWayLabel.TextColor =
                Color.FromArgb("#6E7C7C");
        }


        if (DropoffStep.Content is Label dropoffLabel)
        {
            dropoffLabel.Text = "3";

            dropoffLabel.TextColor =
                Color.FromArgb("#6E7C7C");
        }


        // =====================================================
        // PICKUP
        // =====================================================

        if (status == "New Request" ||
            status == "Accepted" ||
            status == "Preparing" ||
            status == "Ready for Pickup")
        {
            SetStepActive(
                PickupStep,
                "1");

            return;
        }


        // =====================================================
        // ON THE WAY
        // =====================================================

        if (status == "Picked Up" ||
            status == "On the Way")
        {
            SetStepCompleted(
                PickupStep,
                "✓");

            SetStepActive(
                OnWayStep,
                "2");

            PickupToOnWayConnector.BackgroundColor =
                Color.FromArgb("#36ABAD");

            return;
        }


        // =====================================================
        // ARRIVED
        // =====================================================

        if (status == "Arrived")
        {
            SetStepCompleted(
                PickupStep,
                "✓");

            SetStepCompleted(
                OnWayStep,
                "✓");

            SetStepActive(
                DropoffStep,
                "3");

            PickupToOnWayConnector.BackgroundColor =
                Color.FromArgb("#2FA84F");

            OnWayToDropoffConnector.BackgroundColor =
                Color.FromArgb("#36ABAD");

            return;
        }


        // =====================================================
        // DELIVERED
        // =====================================================

        if (status == "Delivered")
        {
            SetStepCompleted(
                PickupStep,
                "✓");

            SetStepCompleted(
                OnWayStep,
                "✓");

            SetStepCompleted(
                DropoffStep,
                "✓");

            PickupToOnWayConnector.BackgroundColor =
                Color.FromArgb("#2FA84F");

            OnWayToDropoffConnector.BackgroundColor =
                Color.FromArgb("#2FA84F");
        }
    }


    private void SetStepActive(
        Border step,
        string text)
    {
        step.BackgroundColor =
            Color.FromArgb("#36ABAD");


        if (step.Content is Label label)
        {
            label.Text = text;

            label.TextColor =
                Colors.White;
        }
    }


    private void SetStepCompleted(
        Border step,
        string text)
    {
        step.BackgroundColor =
            Color.FromArgb("#2FA84F");


        if (step.Content is Label label)
        {
            label.Text = text;

            label.TextColor =
                Colors.White;
        }
    }


    // =========================================================
    // LOCATION PULSE
    // =========================================================

    private void StartLocationPulse()
    {
        StopLocationPulse();


        _pulseCancellation =
            new CancellationTokenSource();


        _ = RunLocationPulseAsync(
            _pulseCancellation.Token);
    }


    private void StopLocationPulse()
    {
        if (_pulseCancellation == null)
            return;


        _pulseCancellation.Cancel();

        _pulseCancellation.Dispose();

        _pulseCancellation = null;


        LocationPulseOuter.CancelAnimations();

        LocationPulseOuter.Scale = 1;

        LocationPulseOuter.Opacity = 1;
    }


    private async Task RunLocationPulseAsync(
        CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                LocationPulseOuter.Scale =
                    0.85;

                LocationPulseOuter.Opacity =
                    0.55;


                await LocationPulseOuter.ScaleTo(
                    1.12,
                    1000,
                    Easing.CubicOut);


                if (token.IsCancellationRequested)
                    break;


                await LocationPulseOuter.FadeTo(
                    0.15,
                    800,
                    Easing.CubicInOut);


                if (token.IsCancellationRequested)
                    break;


                await Task.Delay(
                    350,
                    token);
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}

