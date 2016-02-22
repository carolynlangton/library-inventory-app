using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of a copy of a book.
    /// </summary>
    public class BookCopyViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        /// <summary>
        /// The view model's copy of a book.
        /// </summary>
        private BookCopy copy;

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
        /// <param name="copy">The view model's copy of a book.</param>
        /// <param name="repository">The view model's database.</param>
        public BookCopyViewModel(BookCopy copy, Repository repository)
            : base("Copy")
        {
            this.copy = copy;
            this.repository = repository;
        }

        /// <summary>
        /// Gets the view model's book copy.
        /// </summary>
        public BookCopy Copy
        {
            get
            {
                return this.copy;
            }
        }

        /// <summary>
        /// Gets the author of the book copy.
        /// </summary>
        public Author Author
        {
            get
            {
                return this.copy.Book.Author;
            }
        }

        /// <summary>
        /// Gets or sets the format of the book copy.
        /// </summary>
        public Format Format
        {
            get
            {
                return this.copy.Format;
            }
            set
            {
                this.copy.Format = value;

                this.OnPropertyChanged("Format");
            }
        }

        /// <summary>
        /// Gets a list of Formats.
        /// </summary>
        public IEnumerable<Format> Formats
        {
            get
            {
                return this.repository.GetFormats().Where(f => !f.IsArchived).ToList().OrderBy(f => f.ToString());
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
        /// Gets or sets a value indicating whether or not the view model's copy is archived.
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return this.copy.IsArchived;
            }
            set
            {
                this.copy.IsArchived = value;
                this.OnPropertyChanged("IsArchived");
            }
        }

        /// <summary>
        /// Gets the title of book associated with the view model's copy.
        /// </summary>
        public string Title
        {
            get
            {
                return this.copy.Book.Title;
            }
        }

        /// <summary>
        /// Gets or sets the view model's ID.
        /// </summary>
        public int Id
        {
            get
            {
                return this.copy.Id;
            }
            set
            {
                this.copy.Id = value;
                this.OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets the view model's number of pages.
        /// </summary>
        public int? NumberOfPages
        {
            get
            {
                return this.copy.NumberOfPages;
            }
            set
            {
                this.copy.NumberOfPages = value;
                this.OnPropertyChanged("NumberOfPages");
            }
        }

        /// <summary>
        /// Gets or sets the publisher of the book copy.
        /// </summary>
        public Publisher Publisher
        {
            get
            {
                return this.copy.Publisher;
            }
            set
            {
                this.copy.Publisher = value;

                this.OnPropertyChanged("Publisher");
            }
        }

        /// <summary>
        /// Gets a list of publishers.
        /// </summary>
        public IEnumerable<Publisher> Publishers
        {
            get
            {
                return this.repository.GetPublishers().Where(p => !p.IsArchived).ToList().OrderBy(p => p.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the view model's shelf number (location).
        /// </summary>
        public string ShelfNumber
        {
            get
            {
                return this.copy.ShelfNumber;
            }
            set
            {
                this.copy.ShelfNumber = value;
                this.OnPropertyChanged("ShelfNumber");
            }
        }

        /// <summary>
        /// Gets or sets the view model's copyright year.
        /// </summary>
        public int CopyrightYear
        {
            get
            {
                return this.copy.CopyrightYear;
            }
            set
            {
                this.copy.CopyrightYear = value;
                this.OnPropertyChanged("CopyrightYear");
            }
        }

        /// <summary>
        /// Gets the copies' due date.
        /// </summary>
        public string DueDate
        {
            get
            {
                return this.copy.TransactionDetails.OrderBy(d => d.DueDate).Last().DueDate.ToString("MMM. d");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the view model's copy is available.
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return this.copy.IsAvailable;
            }
            set
            {
                this.copy.IsAvailable = value;
                this.OnPropertyChanged("IsAvailable");
                this.OnPropertyChanged("Availability");
            }
        }

        /// <summary>
        /// Gets a string representing the availability of the copy.
        /// </summary>
        public string Availability
        {
            get
            {
                string result = string.Empty;

                if (this.Format.Type == "E-book" || this.Format.Type == "Audiobook MP3")
                {
                    result = "Online download";
                }
                else
                {
                    if (this.IsAvailable)
                    {
                        result = "Available";
                    }
                    else
                    {
                        result = this.copy.TransactionDetails.OrderBy(td => td.DueDate).Last().DueDate.ToString("MMM. d");
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the member who currently has the view model's copy checked out (returns null if copy is available).
        /// </summary>
        public Member CurrentMember
        {
            get
            {
                if (this.Copy.IsAvailable)
                {
                    return null;
                }
                else
                {
                    return this.Copy.TransactionDetails.OrderBy(td => td.DueDate).Last().Transaction.Member;
                }
            }
        }

        /// <summary>
        /// Gets the copy's error.
        /// </summary>
        public string Error
        {
            get
            {
                return this.Copy.Error;
            }
        }

        /// <summary>
        /// Gets the copy with the property name.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>The book with the property name.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.Copy[propertyName];
            }
        }

        /// <summary>
        /// Saves the view model's copy in the database.
        /// </summary>
        /// <returns>The boolean result</returns>
        public bool Save()
        {
            bool result = true;

            if (this.Copy.IsValid)
            {
                this.repository.AddBook(this.Copy.Book);

                this.repository.AddCopy(this.copy);

                // Push changes.
                this.repository.SaveToDatabase();
            }
            else
            {
                result = false;
                MessageBox.Show("One or more fields are invalid. Copy could not be saved.");
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