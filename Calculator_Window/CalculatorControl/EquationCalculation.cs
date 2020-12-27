using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Calculator_Window.CalculatorControl
{
  public class EquationCalculation : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;

    private string result;
    public string Result
    {
      get => this.result;
      set
      {
        this.result = value;
        this.OnPropertyChanged(nameof(this.Result));
      }
    }

    private string equation;
    public string Equation
    {
      get => this.equation;
      set
      {
        this.equation = value;
        this.OnPropertyChanged(nameof(this.Equation));
      }
    }

    public EquationCalculation() : this("0", "0") { }

    public EquationCalculation(string _result, string _equation)
      => (Result, Equation) = (_result, _equation);

    protected void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
  }
}
