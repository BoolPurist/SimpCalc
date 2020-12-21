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
    public RelayCommand SpaceCommand { get; private set; }

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
      this.IntegerCommand = new RelayCommand(param => this.IntegerFromResult());
      this.FractionCommand = new RelayCommand(param => this.FractionFromResult());
      this.ResultCommand = new RelayCommand(
        param => this.CalculateResult(),
        param => this.CanCalculateResult()
        );
      this.RemoveCommand = new RelayCommand(
        param => this.RemoveOneChar(),
        param => this.CanRemoveOneChar()
        );
      this.SpaceCommand = new RelayCommand(
        param => this.AddSpace(),
        param => this.CanAddSpace()
        );
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
      catch (OverflowException e)
      {
        ShowParsingError(e);
      }
      catch (DivideByZeroException e)
      {
        ShowParsingError(e);
      }
      catch (CalculationParseException e)
      {
        ShowParsingError(e);
      }

      void ProcessValidResult()
      {
        this.LastResult = this.CalculationOutput;
        this.ShowsResult = true;
      }

      void ShowParsingError(Exception e)
      {
        this.ErrorMessage = e.Message;
        this.ErrorMessageVisible = Visibility.Visible;
      }
    }

    private bool CanCalculateResult()
      => !this.ShowsResult;

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
          this.CalculationOutput += $"{symbol}";                                          
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
     => this.ExtractFractOrIntergerFromResult();

    public void FractionFromResult()
     => this.ExtractFractOrIntergerFromResult(true);

    private void ExtractFractOrIntergerFromResult(bool fractional = false)
    {
      if (!this.ShowsResult)
      {
        this.ResultCommand.Execute(null);
      }

      if (this.ShowsResult)
      {
        this.CalculationOutput = fractional ?
          this.calculatorModel.FractionFromCurrentResult.ToString() :
          this.calculatorModel.IntegerFromCurrentResult.ToString();
        this.LastResult = this.CalculationOutput;
      }

      this.ShowsResult = true;
    }

    private void RemoveOneChar()
    {
      this.CalculationOutput = this.CalculationOutput.Length > 0 ? 
        this.CalculationOutput[..^1] : String.Empty;      
    }

    private bool CanRemoveOneChar()
      => !this.ShowsResult;

    private void AddSpace()
     => this.CalculationOutput += " ";

    private bool CanAddSpace()
      => !this.ShowsResult;

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
