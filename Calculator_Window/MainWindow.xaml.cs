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
using System.Xml.Serialization;
using System.IO;


using Calculator_Window.Util;

namespace Calculator_Window
{
  /// <summary>
  /// Handles dynamic sizing of main window and provides property for 
  /// binding properties for visibility of certain controls in the main window 
  /// </summary>
  public partial class MainWindow : Window, INotifyPropertyChanged
  {

    #region initial variables
    // Initial settings of this application if no settings
    // are not stored yet or loading of invalid setting file occurred
    const bool initShowsLastResult = true;
    const bool initShowsHistory = true;
    const bool initShowsCalculatorState = true;

    const bool initUsesRadians = true;
    const bool initUsesPointAsDecimalSeperator = true;
    const bool initUsesDarkTheme = false;
    const int initRoundingPrecision = 15;
    const int initMaxNumberOfResult = 10;

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary> Opens up modular dialog for setting behavior of the calculator </summary>
    /// <value> 
    /// Getter of command. Command can always executed. Creates an instance
    /// CalculatorSettingDialog and starts it as modular to the user.
    /// </value>
    public RelayCommand OpenSettingCommand { get; private set; }

    /// <summary> Sets all settings possibilities to its initial state </summary>
    /// <value> 
    /// Getter for command. Command always executed 
    /// Currently makes all hide-able controls visible again
    /// and sets all set-able properties of the calculator and history of equation
    /// to a determined initial state. 
    /// </value>
    public RelayCommand ResetSettingCommand { get; private set; }

    public RelayCommand SwitchThemeCommand { get; private set; }

    #region properties

    private bool showsLastResult;

    /// <summary>  
    /// Determines if the label as last result is shown to the user.
    /// </summary>
    /// <value> 
    /// Getter/Setter of visibility of last result
    /// </value>
    public bool ShowsLastResult
    {
      get => this.showsLastResult;
      set
      {
        this.showsLastResult = value;
        this.HandelToggeledControl(nameof(this.ShowsLastResult));
      }
    }

    private bool showsHistory;

    /// <summary>  
    /// Determines if the label as last result is shown to the user.
    /// </summary>
    /// <value> 
    /// Getter/Setter of visibility of history list of all entered equations
    /// </value>
    public bool ShowsHistory
    {
      get => this.showsHistory;
      set
      {
        this.showsHistory = value;
        this.HandelToggeledControl(nameof(this.ShowsHistory));
      }
    }

    private bool showsCalculatorState;

    public bool ShowsCalculatorState
    {
      get => this.showsCalculatorState;
      set
      {
        this.showsCalculatorState = value;
        this.HandelToggeledControl(nameof(this.ShowsCalculatorState));
      }
    }

    // Updates binding, adjust window size because of missing or appeared control and
    // saves that information to a file.
    private void HandelToggeledControl(string bindingTargetName)
    {
      this.OnPropertyChanged(bindingTargetName);
      this.AdjustWindowSize();
      this.SaveVisibility();
    }

    private bool usesesDarkTheme;
    /// <summary> States if dark theme or light theme is used </summary>
    /// <value> 
    /// Getter/Setter if true the dark theme is used otherwise light theme is used 
    /// </value>
    public bool UsesDarkTheme
    {
      get => this.usesesDarkTheme;
      set
      {
        this.usesesDarkTheme = value;
        this.OnPropertyChanged(nameof(this.UsesDarkTheme));
      }
    }

    #endregion
       
    private static readonly XmlSerializer xmlMainMenuSerialize =
      new XmlSerializer(typeof(MenuModel));

    // Name of the setting file and its path.
    private const string MENU_STATE_FILE_PATH = "MenuState.xml";

    // If true the loading of setting is done at the start of the app.
    // Used to prevent saving the setting in the start up.
    private readonly bool initLoadingDone = false;

    public MainWindow()
    {
      InitializeComponent();
      this.DataContext = this;

      this.Loaded +=  (sender, e) => { AdjustWindowSize(); };
      // Load settings at the start.
      this.LoadSettings();
      this.initLoadingDone = true;
            
      this.OpenSettingCommand = new RelayCommand(param => this.OpenSettings());
      this.ResetSettingCommand = new RelayCommand(parma => this.ResetSettings());
      this.SwitchThemeCommand = new RelayCommand(param => this.SwitchTheme());
    }

    // Making sure that the user does not shrink the main window to a size
    // which breaks the layout.
    private void AdjustWindowSize()
    {
      
      double currentWidht = this.ActualWidth;
      double currentHeight = this.ActualHeight;

      SizeToContent = SizeToContent.WidthAndHeight;
      this.MinHeight = this.ActualHeight;
      this.MinWidth = this.ActualWidth;
    }

    // Depending on the property UsesDarkTheme respective control get a different 
    // color to match either a light or dark themed GUI
    private void SwitchTheme()
    {
      this.UsesDarkTheme = !this.UsesDarkTheme;

      if (this.UsesDarkTheme)
      {
        Calc.BorderBrush = new SolidColorBrush(Colors.White);
      }
      else
      {
        Calc.BorderBrush = new SolidColorBrush(Colors.Black);
      }

      this.SaveSettings();
    }

    // Resets the state of the main menu and because 
    // there are no custom settings stored yet or 
    // the setting file is not valid.
    private void ResetSettings()
    {
      this.ShowsLastResult = initShowsLastResult;
      this.ShowsHistory = initShowsHistory;
      this.ShowsCalculatorState = initShowsCalculatorState;
      this.Calc.UsesRadians = initUsesRadians;
      this.Calc.UsesPointAsDecimalSeperator = initUsesPointAsDecimalSeperator;
      this.UsesDarkTheme = initUsesDarkTheme;
      this.Calc.RoundingPrecision = initRoundingPrecision;
      this.Calc.MaxNumberOfResult = initMaxNumberOfResult;

      this.SaveSettings();
    }

    // Opens dialog for the user to adjust the behavior of the calculator
    private void OpenSettings()
    {
      var settingDialog = new CalculatorSettingDialog(
        Calc.UsesRadians,
        Calc.UsesPointAsDecimalSeperator,
        Calc.RoundingPrecision,
        Calc.MaxNumberOfResult
        );

      settingDialog.ShowDialog();

      if (settingDialog.DialogResult == true)
      {
        Calc.UsesRadians = settingDialog.UsesRadians;
        Calc.UsesPointAsDecimalSeperator = settingDialog.UsesPoint;
        Calc.RoundingPrecision = settingDialog.RoundingPrecision;
        Calc.MaxNumberOfResult = settingDialog.MaxNbrOfStoredCalcs;
      }

      this.SaveSettings();
    }

    // Saves visibility of all toggable controls and settings of 
    // the calculator in xml file as a setting file.
    private void SaveSettings()
    {
      var menuState = new MenuModel()
      {
        ViewState = new View()
        {
          ShowLastResult = this.ShowsLastResult,
          ShowHistory = this.ShowsHistory,
          ShowCalculatorSetting = this.ShowsCalculatorState,
          UsesDarkTheme = this.UsesDarkTheme
        },
        OptionState = new Option()
        {
          SettingOfCalculator = new CalculatorSettings()
          {
            UsesRadians = this.Calc.UsesRadians,
            UsesPointAsDecimalSeparator = this.Calc.UsesPointAsDecimalSeperator,
            RoundingPrecision = this.Calc.RoundingPrecision,
            MaxNumberOfResult = this.Calc.MaxNumberOfResult
          }
        }
      };

      using var writer = new StreamWriter(MENU_STATE_FILE_PATH);
        xmlMainMenuSerialize.Serialize(writer, menuState);

    }

    // Invoked to save visibility of all toggable controls.
    private void SaveVisibility()
    {
      if (this.initLoadingDone)
      {
        this.SaveSettings();
      }
    }

    // Tries to read from setting file as .xml file
    private void LoadSettings()
    {     
      if (File.Exists(MENU_STATE_FILE_PATH))
      {
        MenuModel menuState = null;

        try
        {                    
          using var reader = new StreamReader(MENU_STATE_FILE_PATH);          
          menuState = xmlMainMenuSerialize.Deserialize(reader) as MenuModel;                  
        }
        catch (InvalidOperationException)
        {
          this.ResetSettings();
          return;
        }
        // Checks if casting to MenuModel was successful
        if (menuState != null)
        {
          ApplySettingsLoad(menuState);          
        }
        else
        {
          this.ResetSettings();          
        }

        
      }
      else
      {
        this.ResetSettings();
      }

      // Sets the certain properties of the application 
      // to apply saved settings
      // If setting file is not valid the initial settings are
      // applied instead.
      void ApplySettingsLoad(MenuModel menuState)
      {
        try
        {
          View viewState = menuState.ViewState;
          Option optionState = menuState.OptionState;

          CalculatorSettings calculatorSettingsState =
            optionState.SettingOfCalculator;

          this.ShowsLastResult = viewState.ShowLastResult;
          this.ShowsHistory = viewState.ShowHistory;
          this.ShowsCalculatorState = viewState.ShowCalculatorSetting;
          this.UsesDarkTheme = viewState.UsesDarkTheme;

          Calculator calculator = this.Calc;

          calculator.UsesRadians = calculatorSettingsState.UsesRadians;
          calculator.UsesPointAsDecimalSeperator =
            calculatorSettingsState.UsesPointAsDecimalSeparator;

          try
          {
            calculator.RoundingPrecision = calculatorSettingsState.RoundingPrecision;
            calculator.MaxNumberOfResult = calculatorSettingsState.MaxNumberOfResult;
          }
          catch (ArgumentOutOfRangeException)
          {
            this.ResetSettings();
          }
        }
        catch (ArgumentNullException)
        {
          this.ResetSettings();
        }        
      }
    }

    private void OnPropertyChanged(string paramName)
      => this.PropertyChanged?.Invoke(
          this, new PropertyChangedEventArgs(paramName)
          );

  }
}
