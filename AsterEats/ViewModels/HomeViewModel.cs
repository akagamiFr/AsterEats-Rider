using System.Windows.Input;
using AsterEats.Models;
using AsterEats.Services;

namespace AsterEats.ViewModels;

public class HomeViewModel : BaseViewModel
{
    private readonly MockDataService _dataService;

    public Rider Rider { get; }

    private int _todayDeliveryCount;
    public int TodayDeliveryCount
    {
        get => _todayDeliveryCount;
        set => SetProperty(ref _todayDeliveryCount, value);
    }

    private decimal _todayEarningsAmount;
    public decimal TodayEarningsAmount
    {
        get => _todayEarningsAmount;
        set => SetProperty(ref _todayEarningsAmount, value);
    }

    private DeliveryOrder? _activeDelivery;
    public DeliveryOrder? ActiveDelivery
    {
        get => _activeDelivery;
        set
        {
            if (SetProperty(ref _activeDelivery, value))
                OnPropertyChanged(nameof(HasActiveDelivery));
        }
    }

    public bool HasActiveDelivery => ActiveDelivery is not null;

    public string GreetingText
    {
        get
        {
            var hour = DateTime.Now.Hour;

            var timeOfDay = hour switch
            {
                < 12 => "Good Morning",
                < 17 => "Good Afternoon",
                _ => "Good Evening"
            };

            return $"{timeOfDay}, {Rider.Name}";
        }
    }

    public string GoOnlineButtonText =>
        Rider.IsOnline ? "GO OFFLINE" : "GO ONLINE";

    public ICommand ToggleOnlineCommand { get; }
    public ICommand RefreshCommand { get; }

    public HomeViewModel(MockDataService dataService)
    {
        _dataService = dataService;

        Rider = _dataService.GetCurrentRider();

        Rider.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Rider.IsOnline))
            {
                OnPropertyChanged(nameof(GoOnlineButtonText));
            }
        };

        ToggleOnlineCommand = new Command(async () => await ToggleOnline());
        RefreshCommand = new Command(LoadTodaysSummary);

        LoadTodaysSummary();
    }

    private async Task ToggleOnline()
    {
        Rider.IsOnline = !Rider.IsOnline;

        if (Rider.IsOnline)
        {
            // Create a delivery request
            // but stay on the Home page.
            ActiveDelivery = _dataService.CreateDeliveryRequest();
        }
        else
        {
            // Go offline and remove the current delivery request.
            ActiveDelivery = null;

            _dataService.SetCurrentDelivery(null);
        }
    }

    private void LoadTodaysSummary()
    {
        var todaysEarnings = _dataService.GetTodaysEarnings();

        TodayDeliveryCount = todaysEarnings.Count;
        TodayEarningsAmount = todaysEarnings.Sum(e => e.Amount);

        ActiveDelivery = _dataService.GetCurrentDelivery();
    }
}