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
    public RelayCommand FractionCommand { get; private set; }
    public RelayCommand IntegerCommand { get; private set; }
    public RelayCommand RemoveCommand { get; private set; }

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
    public Visibility ErrorMessageVisible
    {
      get => this.errorMessageVisible;
      set
      {
        this.errorMessageVisible = value;
        this.OnPropertyChanged(nameof(ErrorMessageVisible));
      }
    }

    protected string calculationOutput = String.Empty;
    public string CalculationOutput
    {
      get => this.calculationOutput;
      set
      {
        this.calculationOutput = value;
        this.ShowsResult = false;
        this.OnPropertyChanged(nameof(CalculationOutput));
      }
    }

    private double mainGridWidth = 0.0;
    private string lastResultToken = "X";  
    public double MainGirdWidth 
    {
      get => this.mainGridWidth;
      set
      {
        this.mainGridWidth = value * 0.9;
        this.OnPropertyChanged(nameof(MainGirdWidth));

      }
    }

    protected bool showsResult = false;
    public bool ShowsResult
    {
      get => this.showsResult;
      set
      {
        this.showsResult = value;
        this.OnPropertyChanged(nameof(this.ShowsResult));
      }
    }

    private readonly CalculatorModel calculatorModel = new CalculatorModel();

    private string lastResult = "0";
    public string LastResult
    {
      get => this.lastResult;
      set
      {
        this.lastResult = value;
        this.OnPropertyChanged(nameof(this.LastResult));        
      }
    }

    public Calculator()
    {
      InitializeComponent();
      DataContext = this;
      
      this.InputCommand = new RelayCommand(this.AddInputToCalc);
      this.ClearCommand = new RelayCommand(param => this.ClearDisplay());
      this.ResultCommand = new RelayCommand(param => this.CalculateResult());
      this.IntegerCommand = new RelayCommand(param => this.IntegerFromResult());
      this.FractionCommand = new RelayCommand(param => this.FractionFromResult());
      this.RemoveCommand = new RelayCommand(param => this.RemoveOneChar());
    }

    public void CalculateResult()
    {
      this.ErrorMessageVisible = Visibility.Collapsed;

      this.CalculationOutput = this.CalculationOutput.Trim();

      if (this.CalculationOutput == String.Empty)
      {
        this.CalculationOutput = "0";
        ProcessValidResult();
      }

      // Insert the result from the last calculation. 
      this.CalculationOutput = 
        this.CalculationOutput.Replace(this.lastResultToken, lastResult);
   
      try
      {        
        this.CalculationOutput = this.calculatorModel
          .CalculateFromText(this.CalculationOutput)
            .ToString();
        ProcessValidResult();
      }      
      catch (CalculationParseException e)
      {
        this.ErrorMessage = e.Message;
        this.ErrorMessageVisible = Visibility.Visible;        
      }

      void ProcessValidResult()
      {
        this.LastResult = this.CalculationOutput;
        this.ShowsResult = true;
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

    public void IntegerFromResult()
    {
      if (!this.ShowsResult)
      {
        this.ResultCommand.Execute(null);
      }

      if (this.ShowsResult)
      {
        this.CalculationOutput = 
          this.calculatorModel.IntegerFromCurrentResult.ToString();
        this.LastResult = this.CalculationOutput;
      }      
    }

    public void FractionFromResult()
    {
      if (!this.ShowsResult)
      {
        this.ResultCommand.Execute(null);
      }

      if (this.ShowsResult)
      {
        this.CalculationOutput =
          this.calculatorModel.FractionFromCurrentResult.ToString();
        this.LastResult = this.CalculationOutput;
      }
    }

    private void RemoveOneChar()
    {
      this.CalculationOutput = this.CalculationOutput.Length > 0 ? 
        this.CalculationOutput[..^1] : String.Empty;
      Debug.WriteLine($"Output after removal {this.CalculationOutput}");
    }

    private void ClearResult_TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (ShowsResult)
      {
        ShowsResult = false;
        if ( sender is TextBox textBox )
        {
          textBox.Text = String.Empty;
        }
      }
    }

    private void GetMainGridWidth_Loaded(object sender, RoutedEventArgs e)
    {
      if (sender is Grid mainGrid)
      {
        this.MainGirdWidth = mainGrid.ActualWidth;
      }
      else
      {
        string faultyParamName = nameof(sender);
        throw new ArgumentException(
          $"Parameter {faultyParamName} must be of type Grid",
          faultyParamName
          );
      }

    }

    private void SetLastResultToken_Button_Loaded(object sender, RoutedEventArgs e)
    {
      string faultyParmaName = nameof(sender);
      if (sender is Button lastResultBtn)
      {
        if (lastResultBtn.Content is string btnContent)
        {
          if (btnContent != null)
          {
            this.lastResultToken = btnContent;
          }
          else
          {
            throw new ArgumentNullException(
              $"{faultyParmaName}.{nameof(lastResultBtn.Content)}",
              $"Property {nameof(lastResultBtn.Content)} of" +
              $"{faultyParmaName} must not be null"
              );
          }
        }
        else
        {
          throw new ArgumentException(
            $"Property {nameof(lastResultBtn.Content)} of" +
            $"{faultyParmaName} must be a string", 
            $"{faultyParmaName}.{nameof(lastResultBtn.Content)}"
            );
        }
      }
      else
      {
        
        throw new ArgumentException(
          $"Parameter {faultyParmaName} must be of type Button", faultyParmaName
          );
      }
    }
         
    private void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));

  }
  
}
