using System.ComponentModel;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of a format.
    /// </summary>
    public class FormatViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        /// <summary>
        /// The view model of format.
        /// </summary>
        private Format format;

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
        /// <param name="format">The view model's format.</param>
        /// <param name="repository">The view model's database.</param>
        public FormatViewModel(Format format, Repository repository)
            : base("Format")
        {
            this.format = format;
            this.repository = repository;
        }

        /// <summary>
        /// Gets the selected format.
        /// </summary>
        public Format Format
        {
            get
            {
                return this.format;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the view model's format is archived.
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return this.format.IsArchived;
            }
            set
            {
                this.format.IsArchived = value;
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
        /// Gets or sets the format's ID.
        /// </summary>
        public int FormatId
        {
            get
            {
                return this.format.FormatId;
            }
            set
            {
                this.format.FormatId = value;
                this.OnPropertyChanged("FormatId");
            }
        }

        /// <summary>
        /// Gets or sets the format type.
        /// </summary>
        public string Type
        {
            get
            {
                return this.format.Type;
            }
            set
            {
                this.format.Type = value;
                this.OnPropertyChanged("Type");
            }
        }

        /// <summary>
        /// Gets the format's error.
        /// </summary>
        public string Error
        {
            get
            {
                return this.Format.Error;
            }
        }

        /// <summary>
        /// Gets the format with the property name.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>The format with the property name.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.Format[propertyName];
            }
        }

        /// <summary>
        /// Saves the view model's book to the database.
        /// </summary>
        /// <returns>A value indicating whether or not the save was successful.</returns>
        public bool Save()
        {
            bool result = true;

            if (this.Format.IsValid)
            {
                this.repository.AddFormat(this.format);

                // Push changes.
                this.repository.SaveToDatabase();
            }
            else
            {
                result = false;
                MessageBox.Show("One or more fields are invalid. Format could not be saved.");
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