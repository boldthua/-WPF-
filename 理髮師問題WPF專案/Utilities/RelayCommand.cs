using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace 理髮師問題WPF專案.Utilities
{
    internal class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action action { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action.Invoke();
        }
        public RelayCommand(Action action)
        {
            this.action = action;
        }
    }

    internal class RelayCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        Action<T> action { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            action.Invoke((T)parameter);
        }
        public RelayCommand(Action<T> action)
        {
            this.action = action;
        }
    }
}
