using AsterEats.Models;
using AsterEats.Services;
using System.Collections.ObjectModel;

namespace AsterEats.ViewModels;

public class EarningsViewModel : BaseViewModel
{
    private readonly MockDataService _dataService;

    public ObservableCollection<DeliveryOrder> Deliveries { get; } = new();

    public ObservableCollection<Earning> Earnings { get; } = new();

    public ObservableCollection<Earning> RecentEarnings { get; } = new();

    public ObservableCollection<WeeklyEarningPoint> WeeklyEarnings { get; } = new();

    private bool _hasRecentEarnings;

    public bool HasRecentEarnings
    {
        get => _hasRecentEarnings;
        set => SetProperty(ref _hasRecentEarnings, value);
    }

    public decimal TodayEarnings =>
        Earnings
            .Where(x =>
                x.DateUtc.Date == DateTime.UtcNow.Date)
            .Sum(x => x.Amount);

    public decimal TotalEarnings =>
        Earnings.Sum(x => x.Amount);

    public int TodayDeliveries =>
        Earnings.Count(x =>
            x.DateUtc.Date == DateTime.UtcNow.Date);

    public decimal WeeklyEarningsTotal =>
        Earnings
            .Where(x =>
                x.DateUtc.Date >= GetStartOfWeek(DateTime.UtcNow).Date &&
                x.DateUtc.Date <= DateTime.UtcNow.Date)
            .Sum(x => x.Amount);

    public decimal AveragePerDelivery =>
        Earnings.Count == 0
            ? 0
            : Earnings.Sum(x => x.Amount) / Earnings.Count;

    public EarningsViewModel(MockDataService dataService)
    {
        _dataService = dataService;
    }

    public void LoadEarnings()
    {
        // Main earnings
        Earnings.Clear();

        foreach (var earning in _dataService.GetEarnings())
        {
            Earnings.Add(earning);
        }

        // Delivery history
        Deliveries.Clear();

        foreach (var delivery in _dataService.GetDeliveryHistory())
        {
            Deliveries.Add(delivery);
        }

        // Recent earnings
        RecentEarnings.Clear();

        foreach (var earning in _dataService.GetRecentEarnings())
        {
            RecentEarnings.Add(earning);
        }

        HasRecentEarnings =
            RecentEarnings.Count > 0;

        // Weekly chart
        BuildWeeklyChart();

        OnPropertyChanged(nameof(TodayEarnings));
        OnPropertyChanged(nameof(TotalEarnings));
        OnPropertyChanged(nameof(TodayDeliveries));
        OnPropertyChanged(nameof(WeeklyEarningsTotal));
        OnPropertyChanged(nameof(AveragePerDelivery));
    }

    private void BuildWeeklyChart()
    {
        WeeklyEarnings.Clear();

        DateTime today =
            DateTime.UtcNow.Date;

        DateTime startOfWeek =
            GetStartOfWeek(today);

        var dailyTotals =
            new List<decimal>();

        for (int i = 0; i < 7; i++)
        {
            DateTime currentDay =
                startOfWeek.AddDays(i);

            decimal amount =
                Earnings
                    .Where(x =>
                        x.DateUtc.Date == currentDay.Date)
                    .Sum(x => x.Amount);

            dailyTotals.Add(amount);
        }

        decimal maxAmount =
            dailyTotals.Count == 0
                ? 0
                : dailyTotals.Max();

        for (int i = 0; i < 7; i++)
        {
            DateTime currentDay =
                startOfWeek.AddDays(i);

            decimal amount =
                dailyTotals[i];

            double barHeight;

            if (maxAmount <= 0)
            {
                barHeight = 8;
            }
            else if (amount <= 0)
            {
                barHeight = 8;
            }
            else
            {
                barHeight =
                    18 +
                    ((double)amount / (double)maxAmount) * 92;
            }

            WeeklyEarnings.Add(
                new WeeklyEarningPoint
                {
                    DayLabel =
                        currentDay.ToString("ddd"),

                    Amount =
                        amount,

                    BarHeight =
                        barHeight
                });
        }
    }

    private static DateTime GetStartOfWeek(DateTime date)
    {
        int difference =
            ((int)date.DayOfWeek + 6) % 7;

        return date.Date.AddDays(-difference);
    }
}