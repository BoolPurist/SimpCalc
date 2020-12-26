using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

namespace Calculator_Window.Util.Validators
{
  public class StringToNumberValidator : ValidationRule
  {
    public int? Min { get; set; } = null;
    public int? Max { get; set; } = null;

    public override ValidationResult Validate(
      object value, 
      CultureInfo cultureInfo
      )
    {
      int numberValue;

      string textInput = value as string;

      if (textInput == null)
      {
        throw new ArgumentException("Value must be of type string !");
      }

      textInput = textInput.Trim();

      if (textInput == String.Empty)
      {
        return new ValidationResult(false, $"Field is empty !");
      }

      try
      {
        numberValue = Int32.Parse(textInput);
      }
      catch (FormatException)
      {
        return new ValidationResult(false, $"Please enter only a number");
      }
      catch (OverflowException)
      {
        return new ValidationResult(false, "Please enter a smaller number");
      }

      if (Min != null && Min > numberValue)
      {
        return new ValidationResult(
          false, $"Please enter a bigger number than {Min - 1}"
          );
      }
      else if (Max != null && Max < numberValue)
      {
        return new ValidationResult(
          false, $"Please enter a smaller number than {Max + 1}"
          );
      }


      return ValidationResult.ValidResult;

    }
  }
}
