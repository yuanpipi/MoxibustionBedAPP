using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoxibustionBedAPP.Models
{
    public class RelayCommand : ICommand
    {
        private Action _execute;

        private Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute=null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        //public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            //throw new NotImplementedException();
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            //throw new NotImplementedException();
            _execute();
        }
    }
}
