using AsterEats.Models;
using AsterEats.Services;
using System.Collections.ObjectModel;

namespace AsterEats.ViewModels;

public class DeliveriesViewModel : BaseViewModel
{
    private readonly MockDataService _dataService;

    public ObservableCollection<DeliveryOrder> Deliveries { get; } = new();

    public DeliveriesViewModel(MockDataService dataService)
    {
        _dataService = dataService;
    }

    public void LoadDeliveries()
    {
        var deliveries = _dataService.GetDeliveryHistory();

        Deliveries.Clear();

        foreach (var delivery in deliveries)
        {
            Deliveries.Add(delivery);
        }

        OnPropertyChanged(nameof(EmptyMessage));
    }

    public string EmptyMessage =>
        Deliveries.Count == 0
            ? "No completed deliveries yet."
            : string.Empty;
}