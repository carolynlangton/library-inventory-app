using System.ComponentModel;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class which describes a view model of an author.
    /// </summary>
    public class AuthorViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        /// <summary>
        /// The view model's author.
        /// </summary>
        private Author author;

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
        /// <param name="author">The view model's author.</param>
        /// <param name="repository">The view model's database.</param>
        public AuthorViewModel(Author author, Repository repository)
            : base("Author")
        {
            this.author = author;
            this.repository = repository;
        }

        /// <summary>
        /// Gets the selected author.
        /// </summary>
        public Author Author
        {
            get
            {
                return this.author;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the view model's author is archived.
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return this.author.IsArchived;
            }
            set
            {
                this.author.IsArchived = value;
                this.OnPropertyChanged("IsArchived");
            }
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
        /// Gets or sets the author's ID.
        /// </summary>
        public int AuthorId
        {
            get
            {
                return this.author.AuthorId;
            }
            set
            {
                this.author.AuthorId = value;
                this.OnPropertyChanged("AuthorId");
            }
        }

        /// <summary>
        /// Gets or sets the author's first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return this.author.FirstName;
            }
            set
            {
                this.author.FirstName = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        /// <summary>
        /// Gets or sets the author's last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return this.author.LastName;
            }
            set
            {
                this.author.LastName = value;
                this.OnPropertyChanged("LastName");
            }
        }

        /// <summary>
        /// Gets the author's error.
        /// </summary>
        public string Error
        {
            get
            {
                return this.Author.Error;
            }
        }

        /// <summary>
        /// Gets the author with the property name.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>The author with the property name.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.Author[propertyName];
            }
        }

        /// <summary>
        /// Saves the view model's author to the database.
        /// </summary>
        /// <returns>A value indicating whether or not the save was successful.</returns>
        public bool Save()
        {
            bool result = true;

            if (this.Author.IsValid)
            {
                // Add to repository.
                this.repository.AddAuthor(this.author);

                // Push changes.
                this.repository.SaveToDatabase();
            }
            else
            {
                result = false;
                MessageBox.Show("One or more fields are invalid. Author could not be saved.");
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