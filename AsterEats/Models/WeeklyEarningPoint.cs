namespace AsterEats.Models;

public class WeeklyEarningPoint
{
    public string DayLabel { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public double BarHeight { get; set; }
}