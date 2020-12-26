using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Calculator_Window
{
  /// <summary>
  /// Handles dynamic sizing of main window and provides property for 
  /// binding properties for visibility of certain controls in the main window 
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {    
    public event PropertyChangedEventHandler PropertyChanged;

    private Visibility lastResultVisibility = Visibility.Visible;

    /// <summary>  
    /// Determines if the label as last result is shown to the user.
    /// </summary>
    /// <value> 
    /// Getter/Setter of visibility of last result
    /// </value>
    public Visibility LastResultVisibility
    {
      get => this.lastResultVisibility;
      set
      {
        this.lastResultVisibility = value;        
        this.PropertyChanged?.Invoke(
          this,
          new PropertyChangedEventArgs(nameof(this.LastResultVisibility))
          );
      }
    }

    private Visibility historyVisibility = Visibility.Collapsed;

    /// <summary>  
    /// Determines if the label as last result is shown to the user.
    /// </summary>
    /// <value> 
    /// Getter/Setter of visibility of history list of all entered equations
    /// </value>
    public Visibility HistoryVisibility
    {
      get => this.historyVisibility;
      set
      {
        this.historyVisibility = value;
        this.PropertyChanged?.Invoke(
          this,
          new PropertyChangedEventArgs(nameof(this.HistoryVisibility))
          );
      }
    }

    public MainWindow()
    {
      InitializeComponent();
      this.DataContext = this;

      this.Loaded += (sender, e) => AdjustWindowSize();
    }

    // Making sure that the user does not shrink the main window to a size
    // which breaks the whole layout.
    private void AdjustWindowSize()
    {
      this.MinHeight = this.ActualHeight;
      this.MinWidth = this.ActualWidth;
    }

  }
}
