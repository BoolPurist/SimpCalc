using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Calculator_Window.CalculatorControl;

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

    /// <summary> 
    /// Empties the list of all calculatedly valid equations in the view and
    /// the model
    /// </summary>
    /// <value> 
    /// Getter for invocation of command. 
    /// Command clears all elements from property HistoryData and 
    /// a internal property of the model as the base of HistoryData.
    /// Command can only executed 
    /// if the history of processed equations in not empty
    /// </value>
    public RelayCommand ClearHistoryCommand { get; private set; }
    /// <summary> 
    /// After command execution Mouse cursor and keyboard have focus 
    /// on the calculator display where equations are entered and results are shown.
    /// </summary>
    /// <value> 
    /// Getter for command execution. Can always be executed.
    /// </value>
    public RelayCommand FocusCalculatorDisplayCommand { get; private set; }

    /// <summary> 
    /// Inserts the last result at the end of equation in the input field in
    /// the calculator
    /// </summary>
    /// <value> 
    /// Getter to execution of a command. Can be executed if InputCommand can
    /// be executed. Appends the value of the property LastResult to 
    /// the property CalculationOutput.
    /// </value>
    public RelayCommand InsertLastResultCommand { get; private set; }

    #region properties

    public ObservableCollection<EquationCalculation> HistoryData
      => this.calculatorModel.Results;

    private double mainHeight;

    public double MainHeight
    {
      get => this.mainHeight;
      set
      {
        this.mainHeight = value;
        this.OnPropertyChanged(nameof(this.MainHeight));
      }
    }

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

    public bool UsesesRadians
    {
      get => this.calculatorModel.UsesRadians;
      set => this.calculatorModel.UsesRadians = value;      
    }

    public bool UsesPointAsDecimalSeperator
    {
      get => this.calculatorModel.UsesPointAsDecimalSeperator;
      set
      {
        this.calculatorModel.UsesPointAsDecimalSeperator = value;
        
        if (value)
        {
          this.CalculationOutput = this.CalculationOutput.Replace(",", ".");
          this.LastResult = this.LastResult.Replace(",", ".");          
        }
        else
        {
          this.CalculationOutput = this.CalculationOutput.Replace(".", ",");
          this.LastResult = this.LastResult.Replace(".", ",");
        }

        this.OnPropertyChanged(nameof(this.UsesPointAsDecimalSeperator));
      }
    }
    
    public int MaxNumberOfResult
    {
      get => this.calculatorModel.MaxNumberOfResult;
      set => this.calculatorModel.MaxNumberOfResult = value;
    }

    public int RoundingPrecision
    {
      get => this.calculatorModel.RoundingPrecision;
      set => this.calculatorModel.RoundingPrecision = value;
    }

    private readonly CalculatorModel calculatorModel = new CalculatorModel();   
    // Set to true if integer or fraction operation was performed
    // Set to false if new equation is entered.
    private bool modifiedResult = false;
    private TextBox calculatorDisplayBox = null;

    #endregion
    
    public Calculator()
    {
      InitializeComponent();
      DataContext = this;
      
      this.InputCommand = new RelayCommand(this.AddInputToCalc);
      this.IntegerCommand = new RelayCommand(
        param => this.IntegerFromResult(),
        param => this.CanExtractFractOrIntergerFromResult()
        );
      this.FractionCommand = new RelayCommand(
        param => this.FractionFromResult(),
        param => this.CanExtractFractOrIntergerFromResult()
        );
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
      this.ClearHistoryCommand = new RelayCommand(
        param => this.ClearHistory(),
        param => this.CanClearHistory()
        );

      this.FocusCalculatorDisplayCommand = 
        new RelayCommand(param => this.FocusCalculatorDisplay());

      this.InsertLastResultCommand =
        new RelayCommand(
          param => this.InputCommand.Execute(this.LastResult),
          param => this.InputCommand.CanExecute(null)
          );

      this.Loaded += (sender, e) => this.MainHeight = this.ActualHeight;      
    }

    #region methods for commands of calculator

    private void CalculateResult()
    {
      this.ErrorMessageVisible = Visibility.Collapsed;

      this.CalculationOutput = this.CalculationOutput.Trim();

      if (this.CalculationOutput == String.Empty)
      {
        this.CalculationOutput = "0";
        ProcessValidResult();
      }

      try
      {
        string equation = this.CalculationOutput;
        this.calculatorModel
          .CalculateFromText(this.CalculationOutput)
            .ToString();
        this.CalculationOutput = this.calculatorModel.LastResult;
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
      catch (CalculationParseSyntaxException e)
      {
        ShowParsingError(e);
      }
      catch (CalculationParseMathematicalException e)
      {
        ShowParsingError(e);
      }

      void ProcessValidResult()
      {
        this.LastResult = calculatorModel.LastResult;
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
      this.modifiedResult = false;

      if (inputSymbol is string symbol)
      {
        if (ShowsResult)
        {
          this.CalculationOutput = String.Empty;
          this.CalculationOutput = symbol;
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
        this.LastResult = calculatorModel.LastResult;
      }

      this.modifiedResult = true;
      this.ShowsResult = true;
    }

    private bool CanExtractFractOrIntergerFromResult()
     => !this.modifiedResult && this.calculationOutput != String.Empty;

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

    private void ClearHistory()
    {
      this.calculatorModel.ClearHistory();
      this.HistoryData.Clear();
    }

    private bool CanClearHistory()
      => this.calculatorModel.Results.Count != 0;

    private void FocusCalculatorDisplay()
    {
      if (this.calculatorDisplayBox != null)
      {
        FocusManager.SetFocusedElement(this, this.calculatorDisplayBox);
        Keyboard.Focus(this.calculatorDisplayBox);
      }
    }

    #endregion

    #region event handler of calculator
    private void ClearResult_TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      this.modifiedResult = false;

      if (ShowsResult)
      {
        ShowsResult = false;
        if (sender is TextBox textBox)
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

    #endregion

    private void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(
        this, new PropertyChangedEventArgs(paramName)
        );

    private void CalcDisplayTextBox_Loaded(object sender, RoutedEventArgs e)
      => this.calculatorDisplayBox = sender as TextBox;
    
  }
  
}
