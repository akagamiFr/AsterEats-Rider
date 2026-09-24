namespace AsterEats.Controls;

public partial class FloatingGlassNavigation : ContentView
{
    private int _selectedIndex = 0;

    private double _dragStartX;
    private double _currentTranslationX;

    private bool _isDragging;
    private bool _isNavigating;

    private bool _isLoaded;


    // =========================================================
    // CONSTRUCTOR
    // =========================================================

    public FloatingGlassNavigation()
    {
        InitializeComponent();

        Loaded += OnNavigationLoaded;
        Unloaded += OnNavigationUnloaded;
        SizeChanged += OnNavigationSizeChanged;
    }


    // =========================================================
    // LOADED
    // =========================================================

    private async void OnNavigationLoaded(
        object? sender,
        EventArgs e)
    {
        if (_isLoaded)
            return;

        _isLoaded = true;

        // Listen for Shell navigation changes
        if (Shell.Current != null)
        {
            Shell.Current.Navigated += OnShellNavigated;
        }

        // Detect current page
        DetectCurrentPage();

        await Task.Delay(80);

        if (!_isDragging)
        {
            SetSelectionPosition(_selectedIndex);
        }
    }


    // =========================================================
    // UNLOADED
    // =========================================================

    private void OnNavigationUnloaded(
        object? sender,
        EventArgs e)
    {
        if (!_isLoaded)
            return;

        _isLoaded = false;

        if (Shell.Current != null)
        {
            Shell.Current.Navigated -= OnShellNavigated;
        }
    }


    // =========================================================
    // SHELL NAVIGATION EVENT
    // =========================================================

    private async void OnShellNavigated(
        object? sender,
        ShellNavigatedEventArgs e)
    {
        if (!_isLoaded)
            return;

        // Give Shell enough time to finish changing page
        await Task.Delay(60);

        if (!_isLoaded)
            return;

        if (_isDragging)
            return;

        DetectCurrentPage();

        await Task.Delay(20);

        if (!_isLoaded)
            return;

        // Animate glass to actual current page
        await AnimateToIndex(
            _selectedIndex);
    }


    // =========================================================
    // DETECT CURRENT PAGE
    // =========================================================

    private void DetectCurrentPage()
    {
        var location =
            Shell.Current?
            .CurrentState?
            .Location?
            .OriginalString;

        if (string.IsNullOrWhiteSpace(location))
        {
            _selectedIndex = 0;
            return;
        }


        if (location.Contains(
            "/deliveries",
            StringComparison.OrdinalIgnoreCase))
        {
            _selectedIndex = 1;
        }
        else if (location.Contains(
            "/earnings",
            StringComparison.OrdinalIgnoreCase))
        {
            _selectedIndex = 2;
        }
        else if (location.Contains(
            "/profile",
            StringComparison.OrdinalIgnoreCase))
        {
            _selectedIndex = 3;
        }
        else
        {
            _selectedIndex = 0;
        }
    }


    // =========================================================
    // SIZE CHANGED
    // =========================================================

    private void OnNavigationSizeChanged(
        object? sender,
        EventArgs e)
    {
        if (GlassBar.Width <= 0)
            return;

        if (_isDragging)
            return;

        SetSelectionPosition(
            _selectedIndex);
    }


    // =========================================================
    // HOME TAP
    // =========================================================

    private async void OnHomeTapped(
        object sender,
        TappedEventArgs e)
    {
        await NavigateToTab(
            "home",
            0);
    }


    // =========================================================
    // DELIVERIES TAP
    // =========================================================

    private async void OnDeliveriesTapped(
        object sender,
        TappedEventArgs e)
    {
        await NavigateToTab(
            "deliveries",
            1);
    }


    // =========================================================
    // EARNINGS TAP
    // =========================================================

    private async void OnEarningsTapped(
        object sender,
        TappedEventArgs e)
    {
        await NavigateToTab(
            "earnings",
            2);
    }


    // =========================================================
    // PROFILE TAP
    // =========================================================

    private async void OnProfileTapped(
        object sender,
        TappedEventArgs e)
    {
        await NavigateToTab(
            "profile",
            3);
    }


    // =========================================================
    // HOME PAN
    // =========================================================

    private void OnHomePanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        HandlePan(
            e,
            0);
    }


    // =========================================================
    // DELIVERIES PAN
    // =========================================================

    private void OnDeliveriesPanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        HandlePan(
            e,
            1);
    }


    // =========================================================
    // EARNINGS PAN
    // =========================================================

    private void OnEarningsPanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        HandlePan(
            e,
            2);
    }


    // =========================================================
    // PROFILE PAN
    // =========================================================

    private void OnProfilePanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        HandlePan(
            e,
            3);
    }


    // =========================================================
    // HANDLE PAN
    // =========================================================

    private void HandlePan(
        PanUpdatedEventArgs e,
        int startingIndex)
    {
        if (GlassBar.Width <= 0)
            return;

        if (_isNavigating)
            return;


        double columnWidth =
            GetColumnWidth();


        switch (e.StatusType)
        {
            // =================================================
            // START
            // =================================================

            case GestureStatus.Started:

                _isDragging = true;


                double startPosition =
                    startingIndex *
                    columnWidth;


                SelectionIndicator
                    .TranslationX =
                    startPosition;


                _dragStartX =
                    startPosition;


                _currentTranslationX =
                    startPosition;


                _ = SelectionIndicator.ScaleTo(
                    0.94,
                    80,
                    Easing.CubicOut);

                break;


            // =================================================
            // RUNNING
            // =================================================

            case GestureStatus.Running:

                if (!_isDragging)
                    return;


                double newX =
                    _dragStartX +
                    e.TotalX;


                double maxX =
                    columnWidth * 3;


                newX = Math.Max(
                    0,
                    Math.Min(
                        maxX,
                        newX));


                _currentTranslationX =
                    newX;


                SelectionIndicator
                    .TranslationX =
                    newX;

                break;


            // =================================================
            // COMPLETED
            // =================================================

            case GestureStatus.Completed:

                if (!_isDragging)
                    return;


                _isDragging = false;


                _ = SelectionIndicator.ScaleTo(
                    1.0,
                    100,
                    Easing.CubicOut);


                int nearestIndex =
                    (int)Math.Round(
                        _currentTranslationX /
                        columnWidth);


                nearestIndex =
                    Math.Max(
                        0,
                        Math.Min(
                            3,
                            nearestIndex));


                _ = CompleteDrag(
                    nearestIndex);

                break;


            // =================================================
            // CANCELED
            // =================================================

            case GestureStatus.Canceled:

                _isDragging = false;


                _ = SelectionIndicator.ScaleTo(
                    1.0,
                    100,
                    Easing.CubicOut);


                SetSelectionPosition(
                    _selectedIndex);

                break;
        }
    }


    // =========================================================
    // NORMAL TAP NAVIGATION
    // =========================================================

    private async Task NavigateToTab(
        string route,
        int index)
    {
        if (_isNavigating)
            return;

        if (_isDragging)
            return;

        if (_selectedIndex == index)
            return;


        _isNavigating = true;


        try
        {
            // Immediately remember target page
            _selectedIndex = index;


            // Smoothly move glass
            await AnimateToIndex(
                index);


            // Navigate
            await Shell.Current.GoToAsync(
                $"//main/{route}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Navigation error: {ex.Message}");

            // Restore actual route if navigation fails
            DetectCurrentPage();

            SetSelectionPosition(
                _selectedIndex);
        }
        finally
        {
            _isNavigating = false;
        }
    }


    // =========================================================
    // COMPLETE DRAG
    // =========================================================

    private async Task CompleteDrag(
        int index)
    {
        string route = index switch
        {
            0 => "home",
            1 => "deliveries",
            2 => "earnings",
            3 => "profile",
            _ => "home"
        };


        // Remember selected destination
        _selectedIndex = index;


        // Smooth snap
        await AnimateToIndex(
            index);


        if (_isNavigating)
            return;


        _isNavigating = true;


        try
        {
            await Shell.Current.GoToAsync(
                $"//main/{route}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Navigation error: {ex.Message}");

            DetectCurrentPage();

            SetSelectionPosition(
                _selectedIndex);
        }
        finally
        {
            _isNavigating = false;
        }
    }


    // =========================================================
    // COLUMN WIDTH
    // =========================================================

    private double GetColumnWidth()
    {
        const double horizontalPadding = 8;


        double availableWidth =
            GlassBar.Width -
            (horizontalPadding * 2);


        return availableWidth / 4;
    }


    // =========================================================
    // SET POSITION
    // =========================================================

    private void SetSelectionPosition(
        int index)
    {
        if (GlassBar.Width <= 0)
            return;


        double columnWidth =
            GetColumnWidth();


        SelectionIndicator.WidthRequest =
            columnWidth - 4;


        double targetX =
            index * columnWidth;


        SelectionIndicator
            .TranslationX =
            targetX;


        _currentTranslationX =
            targetX;
    }


    // =========================================================
    // SMOOTH ANIMATION
    // =========================================================

    private async Task AnimateToIndex(
        int index)
    {
        if (GlassBar.Width <= 0)
            return;


        double columnWidth =
            GetColumnWidth();


        SelectionIndicator.WidthRequest =
            columnWidth - 4;


        double targetX =
            index * columnWidth;


        // Cancel previous animation
        SelectionIndicator.CancelAnimations();


        // Smooth slide
        await SelectionIndicator.TranslateTo(
            targetX,
            0,
            320,
            Easing.CubicInOut);


        _currentTranslationX =
            targetX;


        // Small glass press effect
        await SelectionIndicator.ScaleTo(
            0.96,
            60,
            Easing.CubicOut);


        await SelectionIndicator.ScaleTo(
            1.0,
            120,
            Easing.CubicInOut);
    }
}