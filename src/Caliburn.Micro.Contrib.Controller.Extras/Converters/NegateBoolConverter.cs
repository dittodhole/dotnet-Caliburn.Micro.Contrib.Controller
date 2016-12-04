using System;
using System.Globalization;
using System.Windows.Data;

namespace Caliburn.Micro.Contrib.Controller.Extras.Converters
{
  public class NegateBoolConverter : IValueConverter
  {
    /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="targetType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="targetType" /> is no <see langword="bool" />.</exception>
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

    /// <exception cref="ArgumentNullException"><paramref name="value" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="targetType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="targetType" /> is no <see langword="bool" />.</exception>
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
