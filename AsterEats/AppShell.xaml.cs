namespace AsterEats;

public partial class AppShell : Shell
{
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
}