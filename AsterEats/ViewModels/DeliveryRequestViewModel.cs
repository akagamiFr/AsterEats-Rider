using System.Windows.Input;
using AsterEats.Models;
using AsterEats.Services;

namespace AsterEats.ViewModels;

public class DeliveryRequestViewModel : BaseViewModel
{
    private readonly MockDataService _dataService;

    public DeliveryOrder Delivery { get; }

    public ICommand AcceptCommand { get; }
    public ICommand RejectCommand { get; }

    public DeliveryRequestViewModel(MockDataService dataService)
    {
        _dataService = dataService;

        Delivery = _dataService.GetCurrentDelivery()
            ?? _dataService.CreateDeliveryRequest();

        AcceptCommand = new Command(async () => await AcceptDelivery());
        RejectCommand = new Command(async () => await RejectDelivery());
    }

    private async Task AcceptDelivery()
    {
        Delivery.Status = DeliveryStatus.Accepted;

        await Shell.Current.GoToAsync(
            "orderdetails",
            new Dictionary<string, object>
            {
                { "Order", Delivery }
            });
    }

    private async Task RejectDelivery()
    {
        Delivery.Status = DeliveryStatus.Rejected;

        _dataService.SetCurrentDelivery(null);

        await Shell.Current.GoToAsync("//main/home");
    }
}