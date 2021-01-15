using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator_Window
{
  /// <summary> 
  /// Represents the saved settings of the application 
  /// for serialization/deserialization in xml
  /// </summary>
  public class MenuModel
  {
    /// <summary> 
    /// view settings of this application
    /// </summary>
    public View ViewState { get; set; }

    public Option OptionState { get; set; }
  }

  /// <summary> 
  /// Object for the information of which tog-gable widget are visible
  /// Used for serialization/deserialization in xml
  /// </summary>
  public class View
  {
    /// <summary> Showing last result in a label to the user </summary>
    public bool ShowLastResult { get; set; }
    /// <summary> Showing all processed equations with their results </summary>
    public bool ShowHistory { get; set; }
    /// <summary> Showing the settings value of the calculator to the user </summary>
    public bool ShowCalculatorSetting { get; set; }
    /// <summary> Showing respective colors of a theme </summary>
    /// <value> Getter/Setter. If true the dark theme is shown. </value>
    public bool UsesDarkTheme { get; set; }
  }

  /// <summary> 
  /// Object for all settings of a certain controls 
  /// Used for serialization/deserialization in xml
  /// </summary>
  public class Option
  {
    /// <summary> Object for settings to the calculator </summary>
    public CalculatorSettings SettingOfCalculator { get; set; }
  }

  /// <summary> 
  /// Object for the settings applied to the calculator 
  /// Used for serialization/deserialization in xml
  /// </summary>
  public class CalculatorSettings
  {
    /// <summary> If true radian is used as angel otherwise degreee </summary>
    public bool UsesRadians { get; set; }
    /// <summary> 
    /// If true point as a decimal separator is used. Otherwise the comma is 
    /// used for that.
    /// </summary>
    public bool UsesPointAsDecimalSeparator { get; set; }
    /// <summary> Digits to the a result is rounded up. </summary>
    /// <value> Getter/Setter. Valid value is between 0 to 15 </value>
    public int RoundingPrecision { get; set; }
    /// <summary> 
    /// Number of how many results with their respective equations are shown 
    /// to the user
    /// </summary>
    public int MaxNumberOfResult { get; set; }
  }
}
