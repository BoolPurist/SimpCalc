using System;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Calculator_Window.Util;

namespace Calculator_Window
{
  /// <summary>
  /// Interaction logic for Calculator.xaml
  /// </summary>
  public partial class Calculator : UserControl, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    public RelayCommand InputCommand { get; private set; }

    public RelayCommand ClearCommand { get; private set; }

    public RelayCommand ResultCommand { get; private set; }

    private readonly CalculatorModel calculatorModel = new CalculatorModel();

    private double mainGridWidth = 0.0;

    public double MainGirdWidth 
    {
      get => this.mainGridWidth;
      set
      {
        this.mainGridWidth = value * 0.9;
        this.OnPropertyChanged(nameof(MainGirdWidth));
      }
    }

    public bool ShowsResult { get; private set; } = false;

    public Calculator()
    {
      InitializeComponent();
      DataContext = this;
      this.InputCommand = new RelayCommand(this.AddInputToCalc);
      this.ClearCommand = new RelayCommand(param => this.ClearDisplay());
      this.ResultCommand = new RelayCommand(param => this.CalculateResult());
    }

    private void GetMainGridWidth_Loaded(object sender, RoutedEventArgs e)
    {
      if (sender is Grid mainGrid)
      {
        this.MainGirdWidth = mainGrid.ActualWidth;
      }
    }

    public void CalculateResult()
    {
      this.ShowsResult = true;
      
      this.CalculationInput.Text = 
        this.calculatorModel.CalculateFromText(this.CalculationInput.Text)
          .ToString();
    }

    public void AddInputToCalc(object inputControl)
    {
      if (inputControl is Button inputBtn)
      {
        if (ShowsResult)
        {
          this.CalculationInput.Text = String.Empty;
          ShowsResult = false;
        }

        string symbol = inputBtn.Content as string;
        bool noSpaceNeeded;

        if (
          Boolean.TryParse(inputBtn.Tag as string, out noSpaceNeeded) && noSpaceNeeded
          )
        {
          this.CalculationInput.Text += $"{symbol}";
        }
        else
        {
          this.CalculationInput.Text += $" {symbol} ";
        }
      }
    }

    public void ClearDisplay()
    {
      this.calculatorModel.Clear();
      this.CalculationInput.Text = String.Empty;
    }

    private void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
    
  }
  
}
