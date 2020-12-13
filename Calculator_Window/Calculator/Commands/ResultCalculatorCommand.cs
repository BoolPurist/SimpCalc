﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace Calculator_Window
{
  public class ResultCalculatorCommand : ICommand
  {
    public event EventHandler CanExecuteChanged;

    private readonly Action command;

    public ResultCalculatorCommand(Action _command)
    {
      this.command = _command;
    }

    public bool CanExecute(object parameter) => true;

    public void Execute(object parameter)
    {
      this.command.Invoke();
    }
  }
}
