using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of many books.
    /// </summary>
    public class MultiBookViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// A value indicating whether or not the view model is part of check out mode.
        /// </summary>
        private bool isInCheckOutMode;

        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's view source.
        /// </summary>
        private CollectionViewSource bookViewSource;

        /// <summary>
        /// The sort column's name.
        /// </summary>
        private string sortColumnName;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The view model's representation of the book's multiple copies.
        /// </summary>
        private MultiBookCopyViewModel bookCopyViewModel;

        /// <summary>
        /// The view model's collection of book view model.
        /// </summary>
        private ObservableCollection<BookViewModel> displayedBooks;

        /// <summary>
        /// The command to filter data through format and genre.
        /// </summary>
        private ICommand filterCommand;

        /// <summary>
        /// The command to display data by search text.
        /// </summary>
        private ICommand searchCommand;

        /// <summary>
        /// The command to clear search data.
        /// </summary>
        private ICommand clearCommand;

        /// <summary>
        /// The view model's format for filtering books.
        /// </summary>
        private Format format;

        /// <summary>
        /// The view model's genre for filtering books.
        /// </summary>
        private Genre genre;

        /// <summary>
        /// The view model's user-entered text for searching for books.
        /// </summary>
        private string userText;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's database.</param>
        /// <param name="isInCheckOutMode">A value indicating whether or not the view model is in check out mode.</param>
        public MultiBookViewModel(Repository repository, bool isInCheckOutMode)
            : base("All books")
        {
            this.repository = repository;
            this.isInCheckOutMode = isInCheckOutMode;

            this.DisplayedBooks = new ObservableCollection<BookViewModel>();
            this.bookViewSource = new CollectionViewSource();
            this.bookViewSource.Source = this.DisplayedBooks;

            this.SortCommand = new DelegateCommand(this.Sort);
            this.Commands.Clear();
            this.CreateCommands();

            this.CreateAllBooks();

            this.repository.BookAdded += this.OnBookAdded;
            this.repository.BookArchived += this.OnBookArchived;

            this.AllBooks.ToList().ForEach(b => b.BookSelected += this.OnBookSelected);

            this.Pager = new PagingViewModel(this.AllBooks.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets the list collection view of the sorted books.
        /// </summary>
        public ListCollectionView SortedBooks
        {
            get
            {
                return this.bookViewSource.View as ListCollectionView;
            }
        }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the clear command.
        /// </summary>
        public ICommand ClearCommand
        {
            get
            {
                if (this.clearCommand == null)
                {
                    this.clearCommand = new DelegateCommand(p => this.Refresh());
                }

                return this.clearCommand;
            }
        }

        /// <summary>
        /// Gets the command to filter data.
        /// </summary>
        public ICommand FilterCommand
        {
            get
            {
                if (this.filterCommand == null)
                {
                    this.filterCommand = new DelegateCommand(p => this.Filter());
                }

                return this.filterCommand;
            }
        }

        /// <summary>
        /// Gets the command to search data.
        /// </summary>
        public ICommand SearchCommand
        {
            get
            {
                if (this.searchCommand == null)
                {
                    this.searchCommand = new DelegateCommand(p => this.Search());
                }

                return this.searchCommand;
            }
        }

        /// <summary>
        /// Gets the collection of books.
        /// </summary>
        public ObservableCollection<BookViewModel> DisplayedBooks
        {
            get
            {
                return this.displayedBooks;
            }
            private set
            {
                this.displayedBooks = value;

                // Create and link to collection view source.
                this.bookViewSource = new CollectionViewSource();
                this.bookViewSource.Source = this.DisplayedBooks;
            }
        }

        /// <summary>
        /// Gets or sets the number of books selected.
        /// </summary>
        public int NumberOfBooksSelected { get; set; }

        /// <summary>
        /// Gets the view model's list of books.
        /// </summary>
        public ObservableCollection<BookViewModel> AllBooks { get; private set; }

        /// <summary>
        /// Gets a list of all formats (used to filter).
        /// </summary>
        public IEnumerable<Format> AllFormats
        {
            get
            {
                return this.repository.GetFormats().ToList().OrderBy(f => f.ToString());
            }
        }

        /// <summary>
        /// Gets a list of all genres (used to filter).
        /// </summary>
        public IEnumerable<Genre> AllGenres
        {
            get
            {
                return this.repository.GetGenres().ToList().OrderBy(g => g.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the book copy view model.
        /// </summary>
        public MultiBookCopyViewModel BookCopyViewModel
        {
            get
            {
                return this.bookCopyViewModel;
            }
            set
            {
                this.bookCopyViewModel = value;
                this.OnPropertyChanged("BookCopyViewModel");
            }
        }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        public Format Format
        {
            get
            {
                return this.format;
            }
            set
            {
                this.format = value;
                this.OnPropertyChanged("Format");
            }
        }

        /// <summary>
        /// Gets or sets the genre.
        /// </summary>
        public Genre Genre
        {
            get
            {
                return this.genre;
            }
            set
            {
                this.genre = value;
                this.OnPropertyChanged("Genre");
            }
        }

        /// <summary>
        /// Gets or sets the user entered text.
        /// </summary>
        public string UserText
        {
            get
            {
                return this.userText;
            }
            set
            {
                this.userText = value;
                this.OnPropertyChanged("UserText");
            }
        }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets a value indicating whether one or more book is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                bool result = false;
                int selectedCount = 0;

                foreach (BookViewModel bvm in this.AllBooks)
                {
                    if (bvm.IsSelected != false)
                    {
                        selectedCount += 1;

                        result = true;

                        if (selectedCount != 1)
                        {
                            result = false;

                            break;
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a book is selected.
        /// </summary>
        public bool IsSelectedToRomove
        {
            get
            {
                bool result = false;

                foreach (BookViewModel avm in this.AllBooks)
                {
                    if (avm.IsSelected != false)
                    {
                        result = true;

                        break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Filters the display by format and genre selection.
        /// </summary>
        public void Filter()
        {
            // get value from combobox
            IEnumerable<BookViewModel> books = null;

            if ((this.Format != null && this.Format.Type != "None") && (this.Genre != null && this.Genre.Name != "None"))
            {
                books =
                    from b in this.repository.GetBooks()
                    where !b.IsArchived && b.Copies.Any(c => c.Format == this.Format) && b.Genre == this.Genre
                    select new BookViewModel(b, this.repository);
            }
            else if ((this.Format != null && this.Format.Type != "None") && (this.Genre == null || this.Genre.Name == "None"))
            {
                books =
                    from b in this.repository.GetBooks()
                    where !b.IsArchived && b.Copies.Any(c => c.Format == this.Format)
                    select new BookViewModel(b, this.repository);
            }
            else if ((this.Format == null || this.Format.Type == "None") && (this.Genre != null && this.Genre.Name != "None"))
            {
                books =
                    from b in this.repository.GetBooks()
                    where !b.IsArchived && b.Genre == this.Genre
                    select new BookViewModel(b, this.repository);
            }

            if (books == null)
            {
                this.CreateAllBooks();
            }
            else
            {
                // Display all collection of books as a new collection.
                this.AllBooks = new ObservableCollection<BookViewModel>(books);
            }

            // Call rebuild page data to refresh page.
            this.RebuildPageData();
        }

        /// <summary>
        /// Searches through the book's title and authors to match user entered text.
        /// </summary>
        public void Search()
        {
            // If title contains text then query to display
            string userText = this.UserText;

            // Get value from combobox
            IEnumerable<BookViewModel> books = null;

            if (userText != null)
            {
                books =
                    from b in this.repository.GetBooks()
                    where !b.IsArchived && (b.Author.ToString().ToLower().Contains(userText.ToLower()) || b.Title.ToString().ToLower().Contains(userText.ToLower()))
                    select new BookViewModel(b, this.repository);
            }

            if (books == null)
            {
                this.CreateAllBooks();
            }
            else
            {
                // Display all collection of books as a new collection.
                this.AllBooks = new ObservableCollection<BookViewModel>(books);
            }

            // Call rebuild page data to refresh page.
            this.RebuildPageData();
        }

        /// <summary>
        /// Sort the displayed data ascending, or descending.
        /// </summary>
        /// <param name="parameter">The object to sort.</param>
        public void Sort(object parameter)
        {
            string columnName = parameter as string;

            // If clicking on the header of the currently-sorted column...
            if (this.sortColumnName == columnName)
            {
                // Toggle sorting direction.
                this.sortDirection = this.sortDirection == ListSortDirection.Ascending ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                // Set the sored column.
                this.sortColumnName = columnName;

                // Set sort direction to ascending.
                this.sortDirection = ListSortDirection.Ascending;
            }

            // Clear and reset the sort order of the list view.
            this.bookViewSource.SortDescriptions.Clear();
            this.bookViewSource.SortDescriptions.Add(new SortDescription(this.sortColumnName, this.sortDirection));
        }

        /// <summary>
        /// Refreshes the page.
        /// </summary>
        public void Refresh()
        {
            this.Format = null;
            this.Genre = null;
            this.UserText = string.Empty;
            this.CreateAllBooks();
            this.RebuildPageData();
        }

        /// <summary>
        /// Rebuild data page.
        /// </summary>
        public void RebuildPageData()
        {
            if (this.BookCopyViewModel != null)
            {
                this.BookCopyViewModel.DisplayedCopies.Clear();
            }

            this.DisplayedBooks.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<BookViewModel> displayedBooks = this.AllBooks.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.AllBooks.Count;

            foreach (BookViewModel vm in displayedBooks)
            {
                this.DisplayedBooks.Add(vm);
            }

            this.AllBooks.ToList().ForEach(b => b.BookSelected += this.OnBookSelected);
        }

        /// <summary>
        /// Creates a new book.
        /// </summary>
        public void CreateNewBook()
        {
            // Create view model for book
            BookViewModel viewModel = new BookViewModel(new Book(), this.repository);

            this.ShowBook(viewModel, "Create");
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
            if (!this.isInCheckOutMode)
            {
                this.Commands.Add(new CommandViewModel("New...", new DelegateCommand(p => this.CreateNewBook())));
                this.Commands.Add(new CommandViewModel("Edit...", new DelegateCommand(p => this.EditBook(), b => this.IsSelected)));
                this.Commands.Add(new CommandViewModel("Remove", new DelegateCommand(p => this.ArchiveBook(), b => this.IsSelectedToRomove)));
            }
        }

        /// <summary>
        /// Rebuild the page to show updates in data.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event that handles the page change.</param>
        private void OnPageChange(object sender, CurrentPageChangeEventArgs e)
        {
            this.RebuildPageData();
        }

        /// <summary>
        /// Creates a list of books as view models.
        /// </summary>
        private void CreateAllBooks()
        {
            // Get a list of view models for each book in the database that is not archived
            IEnumerable<BookViewModel> books =
                from book in this.repository.GetBooks()
                where !book.IsArchived
                select new BookViewModel(book, this.repository);

            // Create observable collection from list
            this.AllBooks = new ObservableCollection<BookViewModel>(books);
        }

        /// <summary>
        /// Deletes a book.
        /// </summary>
        private void ArchiveBook()
        {
            BookViewModel viewModel = this.AllBooks.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected book?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.repository.ArchiveBook(viewModel.Book);
                }
            }
            else
            {
                MessageBox.Show("Please select a single book.");
            }

            this.RebuildPageData();
        }

        /// <summary>
        /// Edit the book.
        /// </summary>
        private void EditBook()
        {
            BookViewModel viewModel = this.AllBooks.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                this.ShowBook(viewModel, "Edit");
            }
            else
            {
                MessageBox.Show("Please first select a book.");
            }
        }

        /// <summary>
        /// Show the book.
        /// </summary>
        /// <param name="viewModel">The parameter book view model.</param>
        /// <param name="windowFunction">The function that the window will serve (creating or editing).</param>
        private void ShowBook(BookViewModel viewModel, string windowFunction)
        {
            // Create and configure window.
            WorkspaceWindow window = new WorkspaceWindow();
            window.Title = windowFunction + " " + viewModel.DisplayName;

            // Create and configure view.
            BookView view = new BookView();
            view.DataContext = viewModel;
            viewModel.CloseAction = b => window.DialogResult = b;

            window.Content = view;

            window.ShowDialog();
        }

        /// <summary>
        /// Handles the event of a book being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnBookAdded(object sender, BookEventArgs e)
        {
            BookViewModel viewModel = new BookViewModel(e.Book, this.repository);
            this.AllBooks.Add(viewModel);
            this.RebuildPageData();
        }

        /// <summary>
        /// Remove the selected view model.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The author event argument.</param>
        private void OnBookArchived(object sender, BookEventArgs e)
        {
            BookViewModel viewModel = this.AllBooks.FirstOrDefault(vm => vm.Book == e.Book);

            if (viewModel != null)
            {
                this.AllBooks.Remove(viewModel);
                this.RebuildPageData();
            }
        }

        /// <summary>
        /// Handles the event of a book being selected.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private void OnBookSelected(object sender, BookEventArgs e)
        {
            this.BookCopyViewModel = new MultiBookCopyViewModel(this.repository, e.Book, true);
        }
    }
}