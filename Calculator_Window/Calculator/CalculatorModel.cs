using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator_Window
{
  public class CalculatorModel
  {
    public double CurrentResult { get; private set; } = 0.0;

    public double CalculateFromText(string inputForCalc)
    {     
      this.CurrentResult = 4.0;
      return this.CurrentResult;
    }

    public void Clear() => this.CurrentResult = 0.0;
  }
}
