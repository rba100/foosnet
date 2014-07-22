using System;
using System.Windows.Input;

namespace FoosNet.Controls
{
    public class SimpleCommand : ICommand
    {
        readonly Func<object, bool> m_CanExecute;
        readonly Action<object> m_ExecuteAction;

        public SimpleCommand(Action<object> executeAction)
        {
            m_ExecuteAction = executeAction;
        }

        public SimpleCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            m_ExecuteAction = executeAction;
            m_CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (m_CanExecute != null)
            {
                return m_CanExecute(parameter);
            }
            return false;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public void Execute(object parameter)
        {
            m_ExecuteAction(parameter);
        }
    }
}
