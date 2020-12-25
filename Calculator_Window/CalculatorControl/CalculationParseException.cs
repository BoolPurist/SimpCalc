using System;
using System.Collections.Generic;
using System.Text;
using Calculator_Window.CalculatorControl;

namespace Calculator_Window
{
  public class CalculationParseSyntaxException : Exception
  {
    public SyntaxError SyntaxErrorType { get; private set; }
    public CalculationParseSyntaxException(
      string message, SyntaxError _syntaxErrorType
      ) : base(message)
    {
      this.SyntaxErrorType = _syntaxErrorType;
    }
  }

  public class CalculationParseMathematicalException : Exception
  {
    public MathematicalError MathematicalErrorType { get; private set; }
    public CalculationParseMathematicalException(
      string message, MathematicalError _MathematicalErrorType
      ) : base(message)
    {
      this.MathematicalErrorType = _MathematicalErrorType;
    }
  }

  
}
