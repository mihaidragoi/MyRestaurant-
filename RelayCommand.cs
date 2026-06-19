using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RestaurantIncercareaDoua.ViewModel
{
    internal class RelayCommand : ICommand
    {
        public Action<object> workToDo;
        public Predicate<object> canExecute;

        public RelayCommand(Action<object> workToDo, Predicate<object> canExecute)
        {
            this.workToDo = workToDo;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            workToDo(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
