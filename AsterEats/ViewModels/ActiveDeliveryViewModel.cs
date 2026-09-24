using System.Windows.Input;
using AsterEats.Models;
using AsterEats.Services;

namespace AsterEats.ViewModels;

public class ActiveDeliveryViewModel : BaseViewModel
{
    private readonly MockDataService _dataService;

    public DeliveryOrder Delivery { get; }

    public string StatusTitle => Delivery.StatusText;

    public string ActionButtonText => Delivery.Status switch
    {
        DeliveryStatus.Preparing => "MARK AS READY",
        DeliveryStatus.ReadyForPickup => "MARK AS PICKED UP",
        DeliveryStatus.PickedUp => "START DELIVERY",
        DeliveryStatus.OnTheWay => "I'VE ARRIVED",
        DeliveryStatus.Arrived => "COMPLETE DELIVERY",
        _ => "CONTINUE"
    };

    public string StatusDescription => Delivery.Status switch
    {
        DeliveryStatus.Accepted =>
            "The order has been accepted. Waiting for the restaurant to prepare the food.",

        DeliveryStatus.Preparing =>
            "The restaurant is preparing the order.",

        DeliveryStatus.ReadyForPickup =>
            "The food is ready. Go to the restaurant and collect the order.",

        DeliveryStatus.PickedUp =>
            "You have picked up the order. Deliver it to the customer.",

        DeliveryStatus.OnTheWay =>
            "You are on the way to the customer's location.",

        DeliveryStatus.Arrived =>
            "You have arrived at the customer's location.",

        _ => string.Empty
    };

    public ICommand ActionCommand { get; }

    public ActiveDeliveryViewModel(MockDataService dataService)
    {
        _dataService = dataService;

        Delivery = _dataService.GetCurrentDelivery()
            ?? throw new InvalidOperationException("No active delivery found.");

        Delivery.PropertyChanged += Delivery_PropertyChanged;

        ActionCommand = new Command(AdvanceDelivery);
    }

    private void Delivery_PropertyChanged(object? sender,
        System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DeliveryOrder.Status) ||
            e.PropertyName == nameof(DeliveryOrder.StatusText))
        {
            OnPropertyChanged(nameof(StatusTitle));
            OnPropertyChanged(nameof(ActionButtonText));
            OnPropertyChanged(nameof(StatusDescription));
        }
    }

    private async void AdvanceDelivery()
    {
        switch (Delivery.Status)
        {
            case DeliveryStatus.Accepted:
                Delivery.Status = DeliveryStatus.Preparing;
                break;

            case DeliveryStatus.Preparing:
                Delivery.Status = DeliveryStatus.ReadyForPickup;
                break;

            case DeliveryStatus.ReadyForPickup:
                Delivery.Status = DeliveryStatus.PickedUp;
                break;

            case DeliveryStatus.PickedUp:
                Delivery.Status = DeliveryStatus.OnTheWay;
                break;

            case DeliveryStatus.OnTheWay:
                Delivery.Status = DeliveryStatus.Arrived;
                break;

            case DeliveryStatus.Arrived:

                _dataService.CompleteDelivery(Delivery);

                await Shell.Current.GoToAsync("//main/home");
                break;
        }
    }
}