using System.Windows.Controls;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a control window view model.
    /// </summary>
    public class ControlWindowViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The control window view model's control window content.
        /// </summary>
        private UserControl controlWindowContent;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public ControlWindowViewModel()
            : base("Library System")
        {
        }

        /// <summary>
        /// Gets or sets the control window content.
        /// </summary>
        public UserControl ControlWindowContent
        {
            get
            {
                return this.controlWindowContent;
            }
            set
            {
                this.controlWindowContent = value;
                this.OnPropertyChanged("ControlWindowContent");
            }
        }

        /// <summary>
        /// Create commands.
        /// </summary>
        protected override void CreateCommands()
        {
        }
    }
}