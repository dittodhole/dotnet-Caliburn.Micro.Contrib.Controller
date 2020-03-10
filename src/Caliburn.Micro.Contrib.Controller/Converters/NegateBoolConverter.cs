using System;
using System.Globalization;
using System.Windows.Data;

namespace Caliburn.Micro.Contrib.Controller.Converters
{
  public sealed class NegateBoolConverter : IValueConverter
  {
    /// <inheritdoc/>
    public object Convert(object value,
                          Type targetType,
                          object parameter,
                          CultureInfo culture)
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }
      if (targetType == null)
      {
        throw new ArgumentNullException(nameof(targetType));
      }
      if (targetType != typeof(bool))
      {
        throw new ArgumentOutOfRangeException(nameof(targetType),
                                              $"{nameof(targetType)} must be a boolean");
      }

      return !(bool) value;
    }

    /// <inheritdoc/>
    public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
    {
      if (value == null)
      {
        throw new ArgumentNullException(nameof(value));
      }
      if (targetType == null)
      {
        throw new ArgumentNullException(nameof(targetType));
      }
      if (targetType != typeof(bool))
      {
        throw new ArgumentOutOfRangeException(nameof(targetType),
                                              $"{nameof(targetType)} must be a boolean");
      }

      return !(bool) value;
    }
  }
}
