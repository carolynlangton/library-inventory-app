using System;
using System.Windows.Input;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a delegate command.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// The delegate's command.
        /// </summary>
        private Action<object> command;

        /// <summary>
        /// An indicator of whether or not the command can be executed.
        /// </summary>
        private Predicate<object> canExecute;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command">The delegate's command.</param>
        public DelegateCommand(Action<object> command)
            : this(command, null)
        {
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="command">The delegate's command.</param>
        /// <param name="canExecute">An indicator of whether or not the command can be executed.</param>
        public DelegateCommand(Action<object> command, Predicate<object> canExecute)
        {
            if (command == null)
            {
                throw new Exception("Command was null.");
            }

            this.command = command;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Handles the event of the can execute being changed.
        /// </summary>
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

        /// <summary>
        /// Determines if the command can be executed.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>A value indicating whether or not the command can be executed.</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute(parameter);
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public void Execute(object parameter)
        {
            this.command(parameter);
        }
    }
}