using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Text;

namespace Calculator_Window.Util
{
  public class RelayCommand : ICommand
  {
    public event EventHandler CanExecuteChanged
    {
      add { CommandManager.RequerySuggested += value; }
      remove { CommandManager.RequerySuggested -= value; }
    }

    public RelayCommand(
      Action<object> _execute, Predicate<object> _canExecute = null
      )
    {
      this.execute = _execute 
        ?? throw new ArgumentNullException(nameof(_execute), errorMsgForNoExecute); ;
      this.canExecute = _canExecute;
    }

    private const string errorMsgForNoExecute = 
      "Instance of command has no method to be executed !";

    private readonly Action<object> execute;
    private readonly Predicate<object> canExecute;

    public bool CanExecute(object parameter)
      => this.canExecute == null || this.canExecute.Invoke(parameter);

    public void Execute(object parameter)
    {
      if (this.execute == null)
      {
        throw new ArgumentNullException(errorMsgForNoExecute);
      }

      this.execute.Invoke(parameter);
    }
  }
}
