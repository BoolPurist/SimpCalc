using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Calculator_Window.Util.Converters
{
  [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
  public class BoolToSolidBrushOneWayConverter : IValueConverter
  {
    public SolidColorBrush FalseBrush { get; set; } = null;
    public SolidColorBrush TrueBrush { get; set; } = null;
   
    public object Convert(
      object value, Type targetType, object parameter, CultureInfo culture
      )
    {
      CheckForNotGivenBrushes();
      if (value is bool booleanValue)
      {
        return booleanValue ? TrueBrush : FalseBrush;
      }
      else
      {
        const string faultyName = nameof(value);
        throw new ArgumentException(
          $"{faultyName} must be of type bool, Value was {value}", faultyName
          );
      }
    }

    public object ConvertBack(
      object value, Type targetType, object parameter, CultureInfo culture
      )
    {
      throw new NotImplementedException(
        "Conversion back is not supported with this converter !"
        );
    }

    private void CheckForNotGivenBrushes()
    {
      if (FalseBrush == null)
      {
        const string faultyName = nameof(this.FalseBrush);
        throw new ArgumentNullException(
          faultyName, 
          $"{faultyName} must not be null, property was not given a value !"
          );
      }
      else if (TrueBrush == null)
      {
        const string faultyName = nameof(this.TrueBrush);
        throw new ArgumentNullException(
          faultyName,
          $"{faultyName} must not be null, property was not given a value !"
          );
      }

    }
  }
}
