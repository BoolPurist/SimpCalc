﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace Calculator_Window
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public void App_Startup(object sender, StartupEventArgs e)
    {
      // Start settings for debugging
#if DEBUG
      bool startOnlyCalcSettingDialog = true;

      if (startOnlyCalcSettingDialog)
      {
        var dialogSettingWindow = new CalculatorSettingDialog();
        dialogSettingWindow.Show();
      }
      else
      {
        startProd();
      }

#else
      startProd();
#endif



      static void startProd()
      {
        var mainWindows = new MainWindow();
        mainWindows.Show();
      }


    }
  }
}
