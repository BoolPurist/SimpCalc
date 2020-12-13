using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Controls;

namespace Calculator_Window  
{
  public class InputCalculatorCommand : ICommand
  {    
    public event EventHandler CanExecuteChanged;

    private readonly Action<object> command;

    public InputCalculatorCommand(Action<Object> _command)
    {
      this.command = _command;
    }

    bool ICommand.CanExecute(object parameter) => true;

    void ICommand.Execute(object parameter)
    {
      this.command.Invoke(parameter);     
    }
  }
}
