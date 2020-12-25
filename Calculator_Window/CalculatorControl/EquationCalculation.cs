using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator_Window.CalculatorControl
{
  public struct EquationCalculation
  {
    public readonly double Result;
    public readonly string Equation;

    public EquationCalculation(double _result, string _equation)
      => (Result, Equation) = (_result, _equation);
  }
}
