using System.Globalization;

namespace ToneSync.App.Converters
{
  /// <summary>
  /// Converts a boolean value to its inverse.
  /// </summary>
  public sealed class InvertedBoolConverter : IValueConverter
  {
    public object Convert(
      object? value,
      Type targetType,
      object? parameter,
      CultureInfo culture
    )
    {
      if (value is bool boolValue)
        return !boolValue;

      return false;
    }

    public object ConvertBack(
      object? value,
      Type targetType,
      object? parameter,
      CultureInfo culture
    )
    {
      if (value is bool boolValue)
        return !boolValue;

      return false;
    }
  }
}
