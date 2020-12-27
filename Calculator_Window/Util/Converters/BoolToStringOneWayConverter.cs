using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Calculator_Window.Util.Converters
{
  [ValueConversion(typeof(bool), typeof(string))]
  /// <summary> Converts a boolean value to a respective string value </summary>
  public class BoolToStringOneWayConverter : IValueConverter
  {
    /// <summary> 
    /// String value to converted to if binding source is true 
    /// </summary>
    /// <value> 
    /// Getter/Setter property. Default value is null. 
    /// If no value != null is provided, a ArgumentNullException is thrown
    /// </value>
    public string TextForTrue { get; set; } = null;
    /// <summary> 
    /// String value to converted to if binding source is false 
    /// </summary>
    /// <value> 
    /// Getter/Setter property. Default value is null. 
    /// If no value != null is provided, a ArgumentNullException is thrown
    /// </value>
    public string TextForFalse { get; set; } = null;

    /// <summary> 
    /// Converts a boolean value provided by the binding source into a respective
    /// string value
    /// </summary>
    /// <param name="value"> 
    /// Boolean value from the binding source to be converted 
    /// </param>
    /// <returns> 
    /// Respective string value of the conversion the parameter value as boolean value
    /// </returns>
    /// <exception cref="ArgumentNullException">  
    /// Thrown if property TextForTrue or TextForFalse is null
    /// aka did not get a null value assigned.
    /// </exception>
    public object Convert(
      object value, Type targetType, object parameter, CultureInfo culture
      )
    {
      if (this.TextForTrue == null)
      {
        const string faultyParamName = nameof(this.TextForTrue);
        throw new ArgumentNullException(
          faultyParamName,
          $"{faultyParamName} must not be set to null, Provide a not null value " +
          $"Property {nameof(this.TextForTrue)}"
          );
      }
      else if (this.TextForFalse == null)
      {
        const string faultyParamName = nameof(this.TextForFalse);
        throw new ArgumentNullException(
          faultyParamName,
          $"{faultyParamName} must not be set to null, Provide a not null value " +
          $"Property {nameof(this.TextForFalse)}"
          );
      }

      if (value is bool boolValue)
      {
        return boolValue ? this.TextForTrue : this.TextForFalse;
      }
      else
      {
        const string faultyName = nameof(value);
        throw new ArgumentException(
          $"{faultyName} be must be of type bool", faultyName
          );
      }
    }

    /// <summary> Not implemented. If invoked an exception is thrown </summary>
    /// <exception cref="NotImplementedException"> Always thrown </exception>
    public object ConvertBack(
      object value, Type targetType, object parameter, CultureInfo culture
      )
    {
      throw new NotImplementedException(
        "Conversion is only one way. Conversion back is not supported."
        );
    }
  }
}
