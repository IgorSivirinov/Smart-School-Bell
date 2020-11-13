using System;
using System.Windows.Input;

namespace Smart_school_bell
{
    public class DelegateCommand : ICommand
        {
            Action<object> _execute;
            Func<object, bool> _canExecute;

            public bool CanExecute(object parameter)
            {
                if (_canExecute != null)
                    return _canExecute(parameter);
                return true;
            }

            public void Execute(object parameter)
            {
                _execute?.Invoke(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public DelegateCommand(Action<object> executeAction) : this(executeAction, null) { }

            public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecuteFunc)
            {
                _canExecute = canExecuteFunc;
                _execute = executeAction;
            }
        }
}   