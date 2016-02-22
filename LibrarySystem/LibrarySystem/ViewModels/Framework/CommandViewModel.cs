using System;
using System.Windows.Input;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a command view model.
    /// </summary>
    public class CommandViewModel : ViewModel
    {
        /// <summary>
        /// The name of the group to which the command belongs.
        /// </summary>
        private string groupName;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="displayName">The view model's display name.</param>
        /// <param name="command">The command to execute.</param>
        public CommandViewModel(string displayName, ICommand command)
            : this(displayName, string.Empty, command)
        {
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="displayName">The command's display name.</param>
        /// <param name="groupName">The name of the group to which the command belongs.</param>
        /// <param name="command">The command.</param>
        public CommandViewModel(string displayName, string groupName, ICommand command)
            : base(displayName)
        {
            if (command == null)
            {
                throw new Exception("Command was null.");
            }

            this.Command = command;
            this.groupName = groupName;
        }

        /// <summary>
        /// Gets the view model's command.
        /// </summary>
        public ICommand Command { get; private set; }

        /// <summary>
        /// Gets the name of the group to which the command belongs.
        /// </summary>
        public string GroupName
        {
            get
            {
                return this.groupName;
            }
        }
    }
}