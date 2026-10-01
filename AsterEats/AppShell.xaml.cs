namespace AsterEats;

public partial class AppShell : Shell
{
    public event Action<int>? MainTabChanged;

    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            "deliveryrequest",
            typeof(Views.DeliveryRequestPage));

        Routing.RegisterRoute(
            "orderdetails",
            typeof(Views.OrderDetailsPage));

        Routing.RegisterRoute(
            "activeDelivery",
            typeof(Views.ActiveDeliveryPage));

        Routing.RegisterRoute(
            "notifications",
            typeof(Views.NotificationsPage));
    }

    public int GetCurrentMainTabIndex()
    {
        var location =
            CurrentState?
            .Location?
            .OriginalString;

        if (string.IsNullOrWhiteSpace(location))
            return 0;

        if (location.Contains(
            "/deliveries",
            StringComparison.OrdinalIgnoreCase))
        {
            return 1;
        }

        if (location.Contains(
            "/earnings",
            StringComparison.OrdinalIgnoreCase))
        {
            return 2;
        }

        if (location.Contains(
            "/profile",
            StringComparison.OrdinalIgnoreCase))
        {
            return 3;
        }

        return 0;
    }

    public async Task SelectMainTab(int index)
    {
        string route = index switch
        {
            0 => "//main/home",
            1 => "//main/deliveries",
            2 => "//main/earnings",
            3 => "//main/profile",
            _ => "//main/home"
        };

        try
        {
            var currentRoute =
                CurrentState?
                .Location?
                .OriginalString;

            if (string.Equals(
                currentRoute,
                route,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            await GoToAsync(route);

            // Tell every FloatingGlassNavigation
            // that the main tab has changed.
            MainTabChanged?.Invoke(index);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Tab navigation error: {ex.Message}");
        }
    }
}