using AsterEats.Models;
using AsterEats.Services;
using System.Collections.ObjectModel;

namespace AsterEats.ViewModels;

public class EarningsViewModel : BaseViewModel
{
    private readonly MockDataService _dataService;

    public ObservableCollection<DeliveryOrder> Deliveries { get; } = new();

    public decimal TodayEarnings =>
        Deliveries
            .Where(x => x.CompletedAtUtc.HasValue &&
                        x.CompletedAtUtc.Value.Date == DateTime.UtcNow.Date)
            .Sum(x => x.DeliveryFee);

    public decimal TotalEarnings =>
        Deliveries.Sum(x => x.DeliveryFee);

    public int TodayDeliveries =>
        Deliveries.Count(x =>
            x.CompletedAtUtc.HasValue &&
            x.CompletedAtUtc.Value.Date == DateTime.UtcNow.Date);

    public EarningsViewModel(MockDataService dataService)
    {
        _dataService = dataService;
    }

    public void LoadEarnings()
    {
        Deliveries.Clear();

        foreach (var delivery in _dataService.GetDeliveryHistory())
        {
            Deliveries.Add(delivery);
        }

        OnPropertyChanged(nameof(TodayEarnings));
        OnPropertyChanged(nameof(TotalEarnings));
        OnPropertyChanged(nameof(TodayDeliveries));
    }
}