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
        private Action<object> _execute;
        private Func<object, bool> _canExecute;
        private Action _executeNoParam;
        private Func<bool> _canExecuteNoParam;
        // 构造函数，用于带有参数的命令
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        // 构造函数，用于不带参数的命令
        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _executeNoParam = execute;
            _canExecuteNoParam = canExecute;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                return _canExecute(parameter);
            }
            else if (_canExecuteNoParam != null)
            {
                // 如果是不带参数的CanExecute，忽略传入的参数
                return _canExecuteNoParam();
            }
            return true;
        }
        public void Execute(object parameter)
        {
            if (_execute != null)
            {
                _execute(parameter);
            }
            else if (_executeNoParam != null)
            {
                // 如果是不带参数的Execute，忽略传入的参数
                _executeNoParam();
            }
        }
    }
}
