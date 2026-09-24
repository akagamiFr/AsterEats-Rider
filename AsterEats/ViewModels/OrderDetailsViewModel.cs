using System.Windows.Input;
using AsterEats.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace AsterEats.ViewModels;

public class OrderDetailsViewModel : BaseViewModel, IQueryAttributable
{
    private DeliveryOrder? _order;

    public DeliveryOrder? Order
    {
        get => _order;
        private set => SetProperty(ref _order, value);
    }

    public ICommand StartDeliveryCommand { get; }
    public ICommand CallRestaurantCommand { get; }
    public ICommand CallCustomerCommand { get; }

    public OrderDetailsViewModel()
    {
        StartDeliveryCommand = new Command(async () => await StartDeliveryAsync());

        CallRestaurantCommand = new Command(async () =>
        {
            await CallAsync(Order?.Restaurant?.Phone);
        });

        CallCustomerCommand = new Command(async () =>
        {
            await CallAsync(Order?.CustomerPhone);
        });
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Order", out var value) &&
            value is DeliveryOrder order)
        {
            Order = order;
        }
    }

    private async Task StartDeliveryAsync()
    {
        if (Order is null)
            return;

        Order.Status = DeliveryStatus.Accepted;

        await Shell.Current.GoToAsync(
            "activeDelivery",
            new Dictionary<string, object>
            {
                { "Order", Order }
            });
    }

    private async Task CallAsync(string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return;

        try
        {
            await Launcher.Default.OpenAsync($"tel:{phoneNumber}");
        }
        catch
        {
            StatusMessage = "Calling isn't supported on this device.";
        }
    }
}