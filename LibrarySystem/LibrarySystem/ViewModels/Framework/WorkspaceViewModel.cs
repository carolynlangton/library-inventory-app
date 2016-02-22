using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of a workspace.
    /// </summary>
    public abstract class WorkspaceViewModel : ViewModel
    {
        /// <summary>
        /// The workspace view model's close command.
        /// </summary>
        private DelegateCommand closeCommand;

        /// <summary>
        /// The workspace view model's list of commands.
        /// </summary>
        private ObservableCollection<CommandViewModel> commands = new ObservableCollection<CommandViewModel>();

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="description">The description of the view model.</param>
        public WorkspaceViewModel(string description)
            : base(description)
        {
            this.CreateCommands();
        }

        /// <summary>
        /// The workspace view model's event handler for close requests.
        /// </summary>
        public event EventHandler RequestClose;

        /// <summary>
        /// Gets or sets the action that closes a window.
        /// </summary>
        public Action<bool> CloseAction { get; set; }

        /// <summary>
        /// Gets the command that closes.
        /// </summary>
        public ICommand CloseCommand
        {
            get
            {
                // Use lazy instantiation
                if (this.closeCommand == null)
                {
                    this.closeCommand = new DelegateCommand(p => this.OnRequestClose());
                }

                return this.closeCommand;
            }
        }

        /// <summary>
        /// Gets the workspace view model's list of commands.
        /// </summary>
        public ObservableCollection<CommandViewModel> Commands
        {
            get
            {
                return this.commands;
            }
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        protected abstract void CreateCommands();

        /// <summary>
        /// Occurs when close is requested.
        /// </summary>
        private void OnRequestClose()
        {
            // If "close" event handler is assigned...
            if (this.RequestClose != null)
            {
                // Call the event handler, passing in self
                this.RequestClose(this, EventArgs.Empty);
            }
        }
    }
}