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
        LoadDeliveries();
    }

    public void LoadDeliveries()
    {
        Deliveries.Clear();

        foreach (var delivery in _dataService.GetDeliveryHistory())
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