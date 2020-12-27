using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Calculator_Window.Util.Converters
{
  [ValueConversion(typeof(Visibility), typeof(bool))]
  public class BoolToVisibilityConverter : IValueConverter
  {
    /// <summary> 
    /// Used for mapping bool value false to Visibility value hidden or collapsed 
    /// </summary>
    /// <value> 
    /// Get/Setter, if false, boolean value is converted to Visibility.Hidden 
    /// if true, boolean value is converted to Visibility.Collapses 
    /// </value>
    public bool CollapsesIt { get; set; } = false;
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {      
      if (value is Visibility visibilityStatus)
      {
        return visibilityStatus == Visibility.Visible;
      }
      else
      {
        throw new ArgumentException(
          $"Value must be of type Visibility", nameof(value)
          );
      }

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {      
      if (value is bool showIt)
      {        
        return showIt ? Visibility.Visible : 
          (this.CollapsesIt ? Visibility.Collapsed : Visibility.Hidden);        
      }
      else
      {
        throw new ArgumentException(
          $"Value must be of type bool", nameof(value)
          );
      }
    }
  }
}
