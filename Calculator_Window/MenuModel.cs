using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator_Window
{
  public class MenuModel
  {
    public View ViewState { get; set; }
    public Option OptionState { get; set; }

   
  }

  public class View
  {
    public bool ShowLastResult { get; set; }
    public bool ShowHistory { get; set; }
    public bool ShowCalculatorSetting { get; set; }
  }

  public class Option
  {
    public CalculatorSettings SettingOfCalculator { get; set; }

  }

  public class CalculatorSettings
  {
    public bool UsesRadians { get; set; }
    public bool UsesPointAsDecimalSeparator { get; set; }
    public int RoundingPrecision { get; set; }
    public int MaxNumberOfResult { get; set; }

  }
}
