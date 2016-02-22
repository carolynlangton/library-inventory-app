using System.ComponentModel;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class which describes a view model of a genre.
    /// </summary>
    public class GenreViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        /// <summary>
        /// The view model of genre.
        /// </summary>
        private Genre genre;

        /// <summary>
        /// The view model database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's is selected boolean.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="genreName">The view model's genre.</param>
        /// <param name="repository">The view model's database.</param>
        public GenreViewModel(Genre genreName, Repository repository)
            : base("Genre")
        {
            this.genre = genreName;
            this.repository = repository;
        }

        /// <summary>
        /// Gets the selected genre.
        /// </summary>
        public Genre Genre
        {
            get
            {
                return this.genre;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the view model's genre is archived.
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return this.genre.IsArchived;
            }
            set
            {
                this.genre.IsArchived = value;
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
        /// Gets or sets the genre's ID.
        /// </summary>
        public int GenreId
        {
            get
            {
                return this.genre.GenreId;
            }
            set
            {
                this.genre.GenreId = value;
                this.OnPropertyChanged("GenreId");
            }
        }

        /// <summary>
        /// Gets or sets the genre name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.genre.Name;
            }
            set
            {
                this.genre.Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets the genre's error.
        /// </summary>
        public string Error
        {
            get
            {
                return this.Genre.Error;
            }
        }

        /// <summary>
        /// Gets the genre with the property name.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>The genre with the property name.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.Genre[propertyName];
            }
        }

        /// <summary>
        /// Saves the view model's book to the database.
        /// </summary>
        /// <returns>A value indicating whether or not the save was successful.</returns>
        public bool Save()
        {
            bool result = true;

            if (this.Genre.IsValid)
            {
                this.repository.AddGenre(this.genre);

                // Push changes.
                this.repository.SaveToDatabase();
            }
            else
            {
                result = false;
                MessageBox.Show("One or more fields are invalid. Genre could not be saved.");
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