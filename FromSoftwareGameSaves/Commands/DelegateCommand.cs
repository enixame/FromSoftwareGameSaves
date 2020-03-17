using System;
using System.Windows.Input;

namespace FromSoftwareGameSaves.Commands
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> _canExecuteDelegate;
        private readonly Action<object> _executeDelegate;

        public DelegateCommand(Action<object> execute) 
            : this(execute, null)
        {
        }

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            _executeDelegate = execute;
            _canExecuteDelegate = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteDelegate == null || _canExecuteDelegate(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _executeDelegate?.Invoke(parameter);
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecuteDelegate;
        private readonly Action<T> _executeDelegate;

        public DelegateCommand(Action<T> execute)
            : this(execute, null)
        { 
        }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            _executeDelegate = execute;
            _canExecuteDelegate = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecuteDelegate == null || _canExecuteDelegate((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            _executeDelegate?.Invoke((T)parameter);
        }

        #endregion
    }

}
