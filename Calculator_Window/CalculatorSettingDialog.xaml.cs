using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Calculator_Window
{
  /// <summary>
  /// Interaction logic for CalculatorSettingDialog.xaml
  /// </summary>
  public partial class CalculatorSettingDialog : Window, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private bool usesRadians;
    public bool UsesRadians
    {
      get => this.usesRadians;
      set
      {
        this.usesRadians = value;
        this.OnPropertyChanged(nameof(this.UsesRadians));
      }
    }

    private bool usesDegree;
    public bool UsesDegree
    {
      get => this.usesDegree;
      set
      {
        this.usesDegree = value;
        this.OnPropertyChanged(nameof(this.UsesDegree));
      }
    }

    private bool usesComma;
    public bool UsesComma
    {
      get => this.usesComma;
      set
      {
        this.usesComma = value;
        this.OnPropertyChanged(nameof(this.UsesComma));
      }
    }

    private bool usesPoint;
    public bool UsesPoint
    {
      get => this.usesPoint;
      set
      {
        this.usesPoint = value;
        this.OnPropertyChanged(nameof(this.UsesPoint));
      }
    }

    private int roundingPrecision;

    public int RoundingPrecision
    {
      get => this.roundingPrecision;
      set
      {
        this.roundingPrecision = value;
        this.OnPropertyChanged(nameof(this.RoundingPrecision));
      }
    }

    private int maxNbrOfStoredCalcs;

    public int MaxNbrOfStoredCalcs
    {
      get => this.maxNbrOfStoredCalcs;
      set
      {
        this.maxNbrOfStoredCalcs = value;
        this.OnPropertyChanged(nameof(this.MaxNbrOfStoredCalcs));
      }
    }

    public CalculatorSettingDialog() : this(false, true, 15, 10) { }

    public CalculatorSettingDialog(
      bool _usesRadians,
      bool _usesPoint,
      int _roundingPrecision,
      int _maxNbrOfStoredCalcs
      )
    {
      InitializeComponent();
      this.DataContext = this;
      this.UsesRadians = _usesRadians;
      this.UsesDegree = !_usesRadians;
      this.UsesPoint = _usesPoint;
      this.UsesComma = !_usesPoint;
      this.RoundingPrecision = _roundingPrecision;
      this.MaxNbrOfStoredCalcs = _maxNbrOfStoredCalcs;
    }

    
    protected void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(
        this, new PropertyChangedEventArgs(paramName)
        );

    private void ApplySettings_Button_Click(object sender, RoutedEventArgs e)
    {
      if (sender is Button clickedBtn)
      {
        if (
          !Validation.GetHasError(this.RoundingInput) && 
          !Validation.GetHasError(this.MaxHistoryInput)
          )
        {
          this.DialogResult = true;
          this.Close();
        }
      }
    }

    private void CancleSettings_Button_Click(object sender, RoutedEventArgs e)
    {
      if (sender is Button clickedBtn)
      {
        this.DialogResult = false;
        this.Close();
      }
    }
  }
}
