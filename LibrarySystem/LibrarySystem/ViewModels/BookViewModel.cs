using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of a book.
    /// </summary>
    public class BookViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        /// <summary>
        /// The view model's book.
        /// </summary>
        private Book book;

        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's is selected boolean.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// The filtered view model of the book's associated copies.
        /// </summary>
        private MultiBookCopyViewModel filteredBookCopyViewModel;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="book">The view model's book.</param>
        /// <param name="repository">The view model's database.</param>
        public BookViewModel(Book book, Repository repository)
            : base("Book")
        {
            this.book = book;
            this.repository = repository;
            this.filteredBookCopyViewModel = new MultiBookCopyViewModel(this.repository, this.book, false);
            this.filteredBookCopyViewModel.AllCopies = this.FilteredCopies;
        }

        /// <summary>
        /// Raises an event when a book is selected.
        /// </summary>
        public event EventHandler<BookEventArgs> BookSelected;

        /// <summary>
        /// Gets the selected book.
        /// </summary>
        public Book Book
        {
            get
            {
                return this.book;
            }
        }

        /// <summary>
        /// Gets the view model's number of copies.
        /// </summary>
        public int NumberOfCopies
        {
            get
            {
                return this.book.Copies.Where(c => !c.IsArchived).Count();
            }
        }

        /// <summary>
        /// Gets the copies owned by the view model's book.
        /// </summary>
        public ObservableCollection<BookCopyViewModel> FilteredCopies
        {
            get
            {
                List<BookCopyViewModel> copies = new List<BookCopyViewModel>();

                if (this.book.Copies != null)
                {
                    this.book.Copies.Where(c => !c.IsArchived).ToList().ForEach(c => copies.Add(new BookCopyViewModel(c, this.repository)));
                }

                return new ObservableCollection<BookCopyViewModel>(copies);
            }
        }

        /// <summary>
        /// Gets the view model's filtered multi copy view model.
        /// </summary>
        public MultiBookCopyViewModel FilteredBookCopyViewModel
        {
            get
            {
                return this.filteredBookCopyViewModel;
            }
        }

        /// <summary>
        /// Gets or sets the Author of the book copy.
        /// </summary>
        public Author Author
        {
            get
            {
                return this.book.Author;
            }
            set
            {
                this.book.Author = value;

                this.OnPropertyChanged("Author");
            }
        }

        /// <summary>
        /// Gets a list of Authors.
        /// </summary>
        public IEnumerable<Author> Authors
        {
            get
            {
                return this.repository.GetAuthors().Where(a => !a.IsArchived).ToList().OrderBy(a => a.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the Genre of the book copy.
        /// </summary>
        public Genre Genre
        {
            get
            {
                return this.book.Genre;
            }
            set
            {
                this.book.Genre = value;

                this.OnPropertyChanged("Genre");
            }
        }

        /// <summary>
        /// Gets a list of Genre.
        /// </summary>
        public IEnumerable<Genre> Genres
        {
            get
            {
                return this.repository.GetGenres().Where(g => !g.IsArchived).ToList().OrderBy(g => g.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the view model is selected.
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

                if (value)
                {
                    if (this.BookSelected != null)
                    {
                        this.BookSelected(this, new BookEventArgs(this.book));
                    }
                }

                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the view model's book is archived.
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return this.book.IsArchived;
            }
            set
            {
                this.book.IsArchived = value;
                this.OnPropertyChanged("IsArchived");
            }
        }

        /// <summary>
        /// Gets or sets the book's ID.
        /// </summary>
        public int Id
        {
            get
            {
                return this.book.Id;
            }
            set
            {
                this.book.Id = value;
                this.OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets the book's title.
        /// </summary>
        public string Title
        {
            get
            {
                return this.book.Title;
            }
            set
            {
                this.book.Title = value;
                this.OnPropertyChanged("Title");
            }
        }

        /// <summary>
        /// Gets or sets the book's ISBN.
        /// </summary>
        public string Isbn
        {
            get
            {
                return this.book.Isbn;
            }
            set
            {
                this.book.Isbn = value;
                this.OnPropertyChanged("Isbn");
            }
        }

        /// <summary>
        /// Gets the book's error.
        /// </summary>
        public string Error
        {
            get
            {
                return this.Book.Error;
            }
        }

        /// <summary>
        /// Gets the book with the property name.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>The book with the property name.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.Book[propertyName];
            }
        }

        /// <summary>
        /// Saves the view model's book to the database.
        /// </summary>
        /// <returns>The boolean result.</returns>
        public bool Save()
        {
            bool result = true;

            if (this.Book.IsValid)
            {
                this.repository.AddBook(this.book);

                // Push changes.
                this.repository.SaveToDatabase();

                this.OnPropertyChanged("NumberOfCopies");
            }
            else
            {
                result = false;
                MessageBox.Show("One or more fields are invalid. Book could not be saved.");
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