using System.Globalization;

namespace AsterEats.Converters;

/// <summary>Flips a bool binding - e.g. showing a "No active delivery" placeholder when HasActiveDelivery is false.</summary>
public class InverseBoolConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is bool b && !b;

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> value is bool b && !b;
}
