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
    /// <summary> 
    /// Used to append content as text from a calculation button 
    /// to the binding source for the content of the calculation display 
    /// for the current Result or entered equation.
    /// </summary>
    /// <value> 
    /// Getter for a command to append provided CommandCommand as a string 
    /// to property CalculationOutput.

    /// </value>
    public RelayCommand InputCommand { get; private set; }
    /// <summary> 
    /// Clears the display for showing entered equation or current result  
    /// </summary>
    /// <value> 
    /// Execution of command sets property CalculationOutput to an empty string
    /// </value>
    public RelayCommand ClearCommand { get; private set; }
    /// <summary> 
    /// Tries to convert entered equation to an numeric value and shows 
    /// it to the user if no parsing error happens. If parsing fails user enters 
    /// an invalid equation. If parsing fails, 
    /// an error message is shown to user instead
    /// </summary>
    /// <value> 
    /// Execution of command converts value of the property CalculationOutput 
    /// to a result of the entered result. If entered equation is invalid the
    /// property ErrorMessage is set to the error message of an exception and
    /// the property ErrorMessageVisibility is set to Visibility.Visible
    /// Only executes if property ShowsResult is true.
    /// </value>
    public RelayCommand ResultCommand { get; private set; }
    /// <summary> 
    /// Execution of command displays the whole number part of the current result. 
    /// if entered equation is not calculated yet then the calculation is 
    /// converted to whole number part of this result and shows it to the user.
    /// </summary>
    public RelayCommand FractionCommand { get; private set; }
    /// <summary> 
    /// Execution of command displays the fractional part of the current result. 
    /// if entered equation is not calculated yet then the calculation is 
    /// converted to fractional part of this result and shows it to the user.
    /// </summary>
    public RelayCommand IntegerCommand { get; private set; }
    /// <summary> 
    /// Command to remove last char of text presenting currently entered equation 
    /// </summary>
    /// <value> 
    /// Execution of command removes last char of the property CalculationOutput
    /// Only executes if property ShowsResult is true.
    /// </value>
    public RelayCommand RemoveCommand { get; private set; }
    /// <summary>
    /// Inserts a white space at the end of the calculator display for
    /// the current result in the calculator
    /// </summary>
    /// <value> 
    /// Execution of command appends " " to the property CalculationOutput
    /// Only executes if property ShowsResult is true. 
    /// </value>
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

    private readonly CalculatorModel calculatorModel = new CalculatorModel();
    private string lastResultToken = "X";

    public Calculator()
    {
      InitializeComponent();
      DataContext = this;
      
      this.InputCommand = new RelayCommand(this.AddInputToCalc);
      this.IntegerCommand = new RelayCommand(param => this.IntegerFromResult());
      this.FractionCommand = new RelayCommand(param => this.FractionFromResult());
      this.ClearCommand = new RelayCommand(
        param => this.ClearDisplay(),
        param => this.CanClearDisplay()
        );
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

    private void CalculateResult()
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
      => NotShowingResultAndEmpty();

    private void AddInputToCalc(object inputSymbol)
    {      
      this.ErrorMessageVisible = Visibility.Collapsed;

      if (inputSymbol is string symbol)
      {
        if (ShowsResult)
        {
          this.CalculationOutput = String.Empty;
        }
        else
        {
          this.CalculationOutput += $"{symbol}";
        }                      
      }
      else
      {
        string paramName = nameof(inputSymbol);
        throw new ArgumentException(
          $"Parameter {paramName} must be of type String", paramName
          );
      }
    }

    private void ClearDisplay()
    {
      this.ErrorMessageVisible = Visibility.Collapsed;      
      this.CalculationOutput = String.Empty;
    }

    private bool CanClearDisplay()
     => this.CalculationOutput != String.Empty;

    private void IntegerFromResult()
     => this.ExtractFractOrIntergerFromResult();

    private void FractionFromResult()
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
      => this.CalculationOutput = this.CalculationOutput[..^1];      
    

    private bool CanRemoveOneChar()
      => NotShowingResultAndEmpty();

    private void AddSpace()
     => this.CalculationOutput += " ";

    private bool CanAddSpace()
      => NotShowingResultAndEmpty();

    private bool NotShowingResultAndEmpty()
      => !this.ShowsResult && this.CalculationOutput != String.Empty;

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
      => this.PropertyChanged?.Invoke(
        this, new PropertyChangedEventArgs(paramName)
        );

  }
  
}
