using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator_Window
{
  class CalculatorModel
  {
    public double CurrentResult { get; private set; } = 0.0;

    public double CalculateFromString(string inputForCalc)
    {
      this.CurrentResult = -1.0;
      return this.CurrentResult;
    }

    public void Clear() => this.CurrentResult = 0.0;
  }
}
