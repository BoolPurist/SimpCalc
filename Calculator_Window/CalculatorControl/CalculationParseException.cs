using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator_Window
{
  public class CalculationParseException : Exception
  {
    public CalculationParseException(string message) : base(message) { }    
  }
}
