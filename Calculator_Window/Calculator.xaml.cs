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
    public Calculator()
    {
      InitializeComponent();
      

    }

    private void CalculationBtn_Click(object sender, RoutedEventArgs e)
    {
      Debug.WriteLine("Button was pressed !");
    }

    private void CalculationBtn_Loaded(object sender, RoutedEventArgs e)
    {
      Debug.WriteLine("Loaded");
    }
  }
}
