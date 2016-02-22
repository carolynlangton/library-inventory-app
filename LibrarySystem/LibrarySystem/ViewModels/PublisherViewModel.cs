using System.ComponentModel;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of a publisher.
    /// </summary>
    public class PublisherViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        /// <summary>
        /// The view model's publisher.
        /// </summary>
        private Publisher publisher;

        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's is selected boolean.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="publisher">The parameter publisher.</param>
        /// <param name="repository">The parameter database.</param>
        public PublisherViewModel(Publisher publisher, Repository repository)
            : base("Publisher")
        {
            this.publisher = publisher;
            this.repository = repository;
        }

        /// <summary>
        /// Gets or sets a value indicating whether is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;

                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets or sets the publisher's ID.
        /// </summary>
        public int Id
        {
            get
            {
                return this.publisher.Id;
            }
            set
            {
                this.publisher.Id = value;
                this.OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the view model's publisher is archived.
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return this.publisher.IsArchived;
            }
            set
            {
                this.publisher.IsArchived = value;
                this.OnPropertyChanged("IsArchived");
            }
        }

        /// <summary>
        /// Gets or sets the publisher's location.
        /// </summary>
        public string Location
        {
            get
            {
                return this.publisher.Location;
            }
            set
            {
                this.publisher.Location = value;
                this.OnPropertyChanged("Location");
            }
        }

        /// <summary>
        /// Gets or sets the publisher's name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.publisher.Name;
            }
            set
            {
                this.publisher.Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets the selected publisher.
        /// </summary>
        public Publisher Publisher
        {
            get
            {
                return this.publisher;
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get
            {
                return this.Publisher.Error;
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The name of the property whose error message to get.</param>
        /// <returns> The error message for the property.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.Publisher[propertyName];
            }
        }

        /// <summary>
        /// Saves the view model's publisher to the database.
        /// </summary>
        /// <returns>The true or false result.</returns>
        public bool Save()
        {
            bool result = true;

            if (this.Publisher.IsValid)
            {
                this.repository.AddPublisher(this.publisher);

                // Push changes.
                this.repository.SaveToDatabase();
            }
            else
            {
                result = false;

                MessageBox.Show("One or more fields are invalid.  Publisher cannot be saved.");
            }

            return result;
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("OK", new DelegateCommand(p => this.AcceptChanges())));
            this.Commands.Add(new CommandViewModel("Cancel", new DelegateCommand(p => this.CancelChanges())));
        }

        /// <summary>
        /// Accept the changes made and close.
        /// </summary>
        private void AcceptChanges()
        {
            if (this.Save())
            {
                if (this.CloseAction != null)
                {
                    this.CloseAction(true);
                }
            }
        }

        /// <summary>
        /// Do not accept the changes made and close.
        /// </summary>
        private void CancelChanges()
        {
            if (this.CloseAction != null)
            {
                this.CloseAction(false);
            }
        }
    }
}