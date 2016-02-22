using System.ComponentModel;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model.
    /// </summary>
    public class ViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="displayName">The view model's display name.</param>
        public ViewModel(string displayName)
        {
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Handles the event of a property changing.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the view model's display name.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Creates a string representation of the view model.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.DisplayName;
        }

        /// <summary>
        /// Handles the event of a property changing.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}