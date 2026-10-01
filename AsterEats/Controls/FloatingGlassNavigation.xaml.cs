namespace AsterEats.Controls;

public partial class FloatingGlassNavigation : ContentView
{
    private int _selectedIndex = 0;

    private double _dragStartX;
    private double _currentTranslationX;

    private bool _isDragging;
    private bool _isNavigating;
    private bool _isLoaded;

    public FloatingGlassNavigation()
    {
        InitializeComponent();

        Loaded += OnNavigationLoaded;
        Unloaded += OnNavigationUnloaded;
        SizeChanged += OnNavigationSizeChanged;
    }

    private void OnNavigationLoaded(
        object? sender,
        EventArgs e)
    {
        if (_isLoaded)
            return;

        _isLoaded = true;

        if (Shell.Current is AppShell appShell)
        {
            appShell.MainTabChanged += OnMainTabChanged;
        }

        SyncWithCurrentTab();

        SetSelectionPosition(_selectedIndex);
    }
    private void OnNavigationUnloaded(
      object? sender,
      EventArgs e)
    {
        _isLoaded = false;
    }

    private void OnMainTabChanged(int index)
    {
        if (!_isLoaded)
            return;

        _selectedIndex = index;

        // Immediately move the indicator
        // to the newly selected page.
        SetSelectionPosition(_selectedIndex);
    }

    private void SyncWithCurrentTab()
    {
        if (Shell.Current is AppShell appShell)
        {
            _selectedIndex =
                appShell.GetCurrentMainTabIndex();
        }
        else
        {
            DetectCurrentPage();
        }
    }

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

    private void OnNavigationSizeChanged(
        object? sender,
        EventArgs e)
    {
        if (GlassBar.Width <= 0)
            return;

        if (_isDragging || _isNavigating)
            return;

        SetSelectionPosition(_selectedIndex);
    }

    private async void OnHomeTapped(
        object sender,
        TappedEventArgs e)
    {
        await NavigateToTab(0);
    }

    private async void OnDeliveriesTapped(
        object sender,
        TappedEventArgs e)
    {
        await NavigateToTab(1);
    }

    private async void OnEarningsTapped(
        object sender,
        TappedEventArgs e)
    {
        await NavigateToTab(2);
    }

    private async void OnProfileTapped(
        object sender,
        TappedEventArgs e)
    {
        await NavigateToTab(3);
    }

    private void OnHomePanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        HandlePan(e, 0);
    }

    private void OnDeliveriesPanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        HandlePan(e, 1);
    }

    private void OnEarningsPanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        HandlePan(e, 2);
    }

    private void OnProfilePanUpdated(
        object? sender,
        PanUpdatedEventArgs e)
    {
        HandlePan(e, 3);
    }

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
            case GestureStatus.Started:

                _isDragging = true;

                double startPosition =
                    startingIndex * columnWidth;

                _dragStartX = startPosition;
                _currentTranslationX = startPosition;

                SelectionIndicator.CancelAnimations();

                SelectionIndicator.TranslationX =
                    startPosition;

                SelectionIndicator.Scale = 0.94;

                _ = SelectionIndicator.ScaleTo(
                    0.94,
                    80,
                    Easing.CubicOut);

                break;

            case GestureStatus.Running:

                if (!_isDragging)
                    return;

                double newX =
                    _dragStartX + e.TotalX;

                double maxX =
                    columnWidth * 3;

                newX = Math.Max(
                    0,
                    Math.Min(
                        maxX,
                        newX));

                _currentTranslationX = newX;

                SelectionIndicator.TranslationX =
                    newX;

                break;

            case GestureStatus.Completed:

                if (!_isDragging)
                    return;

                _isDragging = false;

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

                _ = CompleteDrag(nearestIndex);

                break;

            case GestureStatus.Canceled:

                _isDragging = false;

                _ = ReturnGlassToSelectedPosition();

                break;
        }
    }

    private async Task NavigateToTab(int index)
    {
        if (_isNavigating)
            return;

        if (_isDragging)
            return;

        if (Shell.Current is not AppShell appShell)
            return;

        if (_selectedIndex == index)
            return;

        _isNavigating = true;

        try
        {
            _selectedIndex = index;

            // Animate the glass indicator first.
            await AnimateToIndex(index);

            // Navigate immediately after animation.
            await appShell.SelectMainTab(index);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Tab navigation error: {ex.Message}");

            SyncWithCurrentTab();

            SetSelectionPosition(_selectedIndex);
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private async Task CompleteDrag(int index)
    {
        if (Shell.Current is not AppShell appShell)
            return;

        if (_isNavigating)
            return;

        _isNavigating = true;

        try
        {
            _selectedIndex = index;

            // Finish glass movement first.
            await AnimateToIndex(index);

            // Then navigate.
            await appShell.SelectMainTab(index);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Drag navigation error: {ex.Message}");

            SyncWithCurrentTab();

            SetSelectionPosition(_selectedIndex);
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private async Task ReturnGlassToSelectedPosition()
    {
        if (GlassBar.Width <= 0)
            return;

        await AnimateToIndex(_selectedIndex);
    }

    private double GetColumnWidth()
    {
        const double horizontalPadding = 8;

        double availableWidth =
            GlassBar.Width -
            (horizontalPadding * 2);

        return availableWidth / 4;
    }

    private void SetSelectionPosition(int index)
    {
        if (GlassBar.Width <= 0)
            return;

        double columnWidth =
            GetColumnWidth();

        SelectionIndicator.CancelAnimations();

        SelectionIndicator.WidthRequest =
            columnWidth - 4;

        double targetX =
            index * columnWidth;

        SelectionIndicator.TranslationX =
            targetX;

        SelectionIndicator.Scale = 1.0;

        _currentTranslationX =
            targetX;
    }

    private async Task AnimateToIndex(int index)
    {
        if (GlassBar.Width <= 0)
            return;

        double columnWidth =
            GetColumnWidth();

        SelectionIndicator.WidthRequest =
            columnWidth - 4;

        double targetX =
            index * columnWidth;

        SelectionIndicator.CancelAnimations();

        SelectionIndicator.Scale = 0.94;

        Task moveTask =
            SelectionIndicator.TranslateTo(
                targetX,
                0,
                180,
                Easing.CubicOut);

        Task bubbleTask =
            SelectionIndicator.ScaleTo(
                1.04,
                110,
                Easing.CubicOut);

        await Task.WhenAll(
            moveTask,
            bubbleTask);

        _currentTranslationX =
            targetX;

        await SelectionIndicator.ScaleTo(
            0.98,
            45,
            Easing.CubicInOut);

        await SelectionIndicator.ScaleTo(
            1.0,
            65,
            Easing.CubicOut);
    }
}