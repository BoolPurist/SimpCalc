using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator_Window.CalculatorControl
{
  public enum SyntaxError
  {
    InvalidOperand,
    InvalidOperator,
    InvalidFloatingNumber,
    MissingClosingParanthese,
    MissingBaseForFunction,
    MissingParantheseParamForFunction,
    InvalidSorroundedParamter
  }

  public enum MathematicalError
  {
    RootBaseZero,
    RootParamZeroOrSmaller,
    LogBaseZeroOrSmaller,
    LogParamZeroOrSmaller,
    TanInvalidAngle,
    SinCosInvalidAngle,
  }
}
