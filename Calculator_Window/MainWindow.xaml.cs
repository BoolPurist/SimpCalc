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

    public RelayCommand OpenSettingCommand { get; private set; }

    #region properties

    private Visibility lastResultVisibility = Visibility.Visible;

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

    private Visibility historyVisibility = Visibility.Visible;

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

    private Visibility calculatorStateVisibility = Visibility.Visible;

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

      this.Loaded += (sender, e) => AdjustWindowSize();

      this.OpenSettingCommand = new RelayCommand(param => this.OpenSettings());
    }

    // Making sure that the user does not shrink the main window to a size
    // which breaks the whole layout.
    private void AdjustWindowSize()
    {
      this.MinHeight = this.ActualHeight;
      this.MinWidth = this.ActualWidth;
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
