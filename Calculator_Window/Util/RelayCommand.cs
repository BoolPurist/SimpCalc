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
      if (_execute == null)
      {
        throw new ArgumentNullException();
      }

      this.execute = _execute;
      this.canExecute = _canExecute;
    }

    private Action<object> execute;
    private Predicate<object> canExecute;

    public bool CanExecute(object parameter)
      => this.canExecute == null ? 
      true : this.canExecute.Invoke(parameter);

    public void Execute(object parameter)
      => this.execute.Invoke(parameter);
  }
}
