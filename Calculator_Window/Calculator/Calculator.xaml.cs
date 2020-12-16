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

    private TextBox calculationInput = null;

    private Label errorMessageLabel = null;

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

    public void CalculateResult()
    {
      this.errorMessageLabel.Visibility = Visibility.Collapsed;
      this.ShowsResult = true;

      try
      {
        this.calculationInput.Text =
          this.calculatorModel.CalculateFromText(this.calculationInput.Text)
            .ToString();
      }
      catch (ArgumentException e)
      {
        this.calculationInput.Text = "0";
      }
      catch (CalculationParseException e)
      {
        this.errorMessageLabel.Content = e.Message;
        this.errorMessageLabel.Visibility = Visibility.Visible;
      }
    }

    public void AddInputToCalc(object inputControl)
    {
      if (inputControl is Button inputBtn)
      {
        if (ShowsResult)
        {
          this.calculationInput.Text = String.Empty;
          ShowsResult = false;
        }

        string symbol = inputBtn.Content as string;
        bool noSpaceNeeded;

        if (
          Boolean.TryParse(inputBtn.Tag as string, out noSpaceNeeded) && noSpaceNeeded
          )
        {
          this.calculationInput.Text += $"{symbol}";
        }
        else
        {
          this.calculationInput.Text += $" {symbol} ";
        }
      }
    }

    public void ClearDisplay()
    {
      this.calculatorModel.Clear();
      this.calculationInput.Text = String.Empty;
    }

    private void GetMainGridWidth_Loaded(object sender, RoutedEventArgs e)
    {
      if (sender is Grid mainGrid)
      {
        this.MainGirdWidth = mainGrid.ActualWidth;
      }
    }

    private void GetCalculationFeedTextBox_Loaded(object sender, RoutedEventArgs e)
      => this.calculationInput = sender as TextBox;
         
    private void ErrorMsgLabel_Loaded(object sender, RoutedEventArgs e)
      => this.errorMessageLabel = sender as Label;

    private void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));


  }
  
}
