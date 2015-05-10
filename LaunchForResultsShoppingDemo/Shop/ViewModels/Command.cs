
namespace Shop.ViewModels
{
    using System;
    using System.Windows.Input;

    public class Command : ICommand
    {
        private Func<object, bool> canExecuteParameterized;

        private Func<bool> canExecute;

        private Action<object> executeParameterizedAction;

        private Action executeAction;

        public Command(Action executeAction) : this(executeAction, (Func<bool>)null)
        {
        }

        public Command(Action<object> executeParameterizedAction) : this(executeParameterizedAction, null)
        {
        }

        public Command(Action executeAction, Func<bool> canExecute)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            this.executeAction = executeAction;
            this.canExecute = canExecute;
        }

        public Command(Action executeAction, Func<object, bool> canExecute)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            this.executeAction = executeAction;
            this.canExecuteParameterized = canExecute;
        }


        public Command(Action<object> executeAction, Func<object, bool> canExecute)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException("executeAction");
            }

            this.executeParameterizedAction = executeAction;
            this.canExecuteParameterized = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            bool result = true;

            if (this.canExecute != null)
            {
                result = this.canExecute();
            }

            if (this.canExecuteParameterized != null && parameter != null)
            {
                result = this.canExecuteParameterized(parameter);
            }

            return result;
        }

        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public void Execute(object parameter)
        {
            if (this.executeAction != null)
            {
                this.executeAction();
            }

            if (this.executeParameterizedAction != null)
            {
                this.executeParameterizedAction(parameter);
            }
        }
    }
}
