using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Calculator_Window.Util;

namespace Calculator_Window
{
  /// <summary>
  /// Handles dynamic sizing of main window and provides property for 
  /// binding properties for visibility of certain controls in the main window 
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary> Opens up modular dialog for setting behavior of the calculator </summary>
    /// <value> 
    /// Getter of command. Command can always executed. Creates an instance
    /// CalculatorSettingDialog and starts it as modular to the user.
    /// </value>
    public RelayCommand OpenSettingCommand { get; private set; }

    /// <summary> Sets all settings possibilities to its initial state </summary>
    /// <value> 
    /// Getter for command. Command always executed 
    /// Currently makes all hide-able controls visible again
    /// and sets all set-able properties of the calculator and history of equation
    /// to a determined initial state. 
    /// </value>
    public RelayCommand ResetSettingCommand { get; private set; }

    #region properties

    private Visibility lastResultVisibility;

    /// <summary>  
    /// Determines if the label as last result is shown to the user.
    /// </summary>
    /// <value> 
    /// Getter/Setter of visibility of last result
    /// </value>
    public Visibility LastResultVisibility
    {
      get => this.lastResultVisibility;
      set
      {
        this.lastResultVisibility = value;

        this.OnPropertyChanged(nameof(this.LastResultVisibility));
      }
    }

    private Visibility historyVisibility;

    /// <summary>  
    /// Determines if the label as last result is shown to the user.
    /// </summary>
    /// <value> 
    /// Getter/Setter of visibility of history list of all entered equations
    /// </value>
    public Visibility HistoryVisibility
    {
      get => this.historyVisibility;
      set
      {
        this.historyVisibility = value;
        this.OnPropertyChanged(nameof(this.HistoryVisibility));          
      }
    }

    private Visibility calculatorStateVisibility;

    public Visibility CalculatorStateVisibility
    {
      get => this.calculatorStateVisibility;
      set
      {
        this.calculatorStateVisibility = value;
        this.OnPropertyChanged(nameof(this.CalculatorStateVisibility));
      }
    }

    #endregion

    public MainWindow()
    {
      InitializeComponent();
      this.DataContext = this;
      this.ResetSettings();

      this.Loaded += (sender, e) => AdjustWindowSize();

      this.OpenSettingCommand = new RelayCommand(param => this.OpenSettings());
      this.ResetSettingCommand = new RelayCommand(parma => this.ResetSettings());
    }

    // Making sure that the user does not shrink the main window to a size
    // which breaks the whole layout.
    private void AdjustWindowSize()
    {
      this.MinHeight = this.ActualHeight;
      this.MinWidth = this.ActualWidth;
    }

    private void ResetSettings()
    {
      const Visibility standardVisibility = Visibility.Visible;
      this.HistoryVisibility = standardVisibility;
      this.CalculatorStateVisibility = standardVisibility;
      this.LastResultVisibility = standardVisibility;
      this.Calc.LastResult = "0";
      this.Calc.UsesPointAsDecimalSeperator = true;
      this.Calc.RoundingPrecision = 15;
      this.Calc.MaxNumberOfResult = 10;
    }

    private void OpenSettings()
    {
      var settingDialog = new CalculatorSettingDialog(
        Calc.UsesRadians,
        Calc.UsesPointAsDecimalSeperator,
        Calc.RoundingPrecision,
        Calc.MaxNumberOfResult
        );

      settingDialog.ShowDialog();

      if (settingDialog.DialogResult == true)
      {
        Calc.UsesRadians = settingDialog.UsesRadians;
        Calc.UsesPointAsDecimalSeperator = settingDialog.UsesPoint;
        Calc.RoundingPrecision = settingDialog.RoundingPrecision;
        Calc.MaxNumberOfResult = settingDialog.MaxNbrOfStoredCalcs;
      }
    }


    private void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(
          this, new PropertyChangedEventArgs(paramName)
          );

  }
}
