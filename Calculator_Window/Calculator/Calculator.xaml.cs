using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator_Window
{
  /// <summary>
  /// Interaction logic for Calculator.xaml
  /// </summary>
  public partial class Calculator : UserControl
  {
    public InputCalculatorCommand InputCommand { get; set; }

    public ClearCalculatorCommand ClearCommand { get; set; }

    public ResultCalculatorCommand ResultCommand { get; set; }

    public bool ShowsResult { get; private set; } = false;

    public Calculator()
    {
      InitializeComponent();
      DataContext = this;
      this.InputCommand = new InputCalculatorCommand(this.AddInputToCalc);
      this.ClearCommand = new ClearCalculatorCommand(this.ClearDisplay);
      this.ResultCommand = new ResultCalculatorCommand(this.CalculateResult);
    }

    public void CalculateResult()
    {
      this.ShowsResult = true;
      this.CalculationInput.Text = "0";
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
      this.CalculationInput.Text = String.Empty;
    }
  }
  
}
