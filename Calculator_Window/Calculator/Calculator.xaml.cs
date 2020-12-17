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

    protected string errorMessage = String.Empty;

    public string ErrorMessage
    {
      get => this.errorMessage;
      set
      {
        this.errorMessage = value;
        this.OnPropertyChanged(nameof(this.ErrorMessage));
      }
    }
    
    protected Visibility errorMessageVisible = Visibility.Collapsed;
    protected string calculationOutput = String.Empty;

    public Visibility ErrorMessageVisible
    {
      get => this.errorMessageVisible;
      set
      {
        
        this.errorMessageVisible = value;
        this.OnPropertyChanged(nameof(ErrorMessageVisible));
      }
    }

    public string CalculationOutput
    {
      get => this.calculationOutput;
      set
      {
        this.calculationOutput = value;
        this.OnPropertyChanged(nameof(CalculationOutput));
      }
    }

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

    private readonly CalculatorModel calculatorModel = new CalculatorModel();

    public Calculator()
    {
      InitializeComponent();
      DataContext = this;
      this.InputCommand = new RelayCommand(this.AddInputToCalc);
      this.ClearCommand = new RelayCommand(param => this.ClearDisplay());
      this.ResultCommand = new RelayCommand(param => this.CalculateResult());

#if DEBUG
      this.errorMessageVisible = Visibility.Visible;
      this.errorMessage = "Error message";
#endif
    }

    public void CalculateResult()
    {
      this.ErrorMessageVisible = Visibility.Collapsed;
      this.ShowsResult = true;

      try
      {
        this.CalculationOutput = this.calculatorModel
          .CalculateFromText(this.CalculationOutput)
            .ToString();
      }
      catch (ArgumentException)
      {
        this.CalculationOutput = "0";
      }
      catch (CalculationParseException e)
      {
        this.ErrorMessage = e.Message;
        this.ErrorMessageVisible = Visibility.Visible;
      }
    }

    public void AddInputToCalc(object inputControl)
    {
      this.ErrorMessageVisible = Visibility.Collapsed;

      if (inputControl is Button inputBtn)
      {
        if (ShowsResult)
        {
          this.CalculationOutput = String.Empty;
          ShowsResult = false;
        }

        if (inputBtn.Content is string symbol)
        {
          if (
          Boolean.TryParse(inputBtn.Tag as string, out bool noSpaceNeeded) && 
          noSpaceNeeded
          )
          {
            this.CalculationOutput += $"{symbol}";
          }
          else
          {
            this.CalculationOutput += $" {symbol} ";
          }
        }
        else
        {
          throw new ArgumentException(
            $"Content of {nameof(inputControl)} must be a string !", 
            nameof(inputBtn.Content)
            );
        }
                
      }
      else
      {
        throw new ArgumentException(
          "Control as parameter must be of type Button", nameof(inputControl)
          );
      }
    }

    public void ClearDisplay()
    {
      this.ErrorMessageVisible = Visibility.Collapsed;

      this.calculatorModel.Clear();
      this.CalculationOutput = String.Empty;
    }

    private void GetMainGridWidth_Loaded(object sender, RoutedEventArgs e)
    {
      if (sender is Grid mainGrid)
      {
        this.MainGirdWidth = mainGrid.ActualWidth;
      }
    }
         
    private void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));


  }
  
}
