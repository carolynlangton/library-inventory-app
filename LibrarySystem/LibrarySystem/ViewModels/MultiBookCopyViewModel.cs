using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of many copies of a book.
    /// </summary>
    public class MultiBookCopyViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The owner of the multiple copies.
        /// </summary>
        private Book owner;

        /// <summary>
        /// A value indicating whether is in preview mode or not.
        /// </summary>
        private bool isInPreviewMode;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's database.</param>
        /// <param name="book">The book owner of the multiple copies.</param>
        /// <param name="isInPreviewMode">Indicates if is in preview mode or not.</param>
        public MultiBookCopyViewModel(Repository repository, Book book, bool isInPreviewMode)
            : base("All copies")
        {
            this.repository = repository;
            this.owner = book;
            this.isInPreviewMode = isInPreviewMode;
            this.CreateCommands();

            this.DisplayedCopies = new ObservableCollection<BookCopyViewModel>();

            this.CreateAllCopies();

            this.repository.CopyAdded += this.OnCopyAdded;
            this.repository.CopyArchived += this.OnCopyArchived;

            this.Pager = new PagingViewModel(this.AllCopies.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets or sets the number of copies selected.
        /// </summary>
        public int NumberOfCopiesSelected { get; set; }

        /// <summary>
        /// Gets or sets the view model's list of copies.
        /// </summary>
        public ObservableCollection<BookCopyViewModel> AllCopies { get; set; }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets the collection of copies.
        /// </summary>
        public ObservableCollection<BookCopyViewModel> DisplayedCopies { get; private set; }

        /// <summary>
        /// Creates a new copy of a book.
        /// </summary>
        public void CreateNewCopy()
        {
            // Create view model for book
            BookCopyViewModel viewModel = new BookCopyViewModel(new BookCopy { BookId = this.owner.Id, Book = this.owner, IsAvailable = true }, this.repository);

            this.ShowBookCopy(viewModel, "Create");
        }

        /// <summary>
        /// Rebuild data page.
        /// </summary>
        public void RebuildPageData()
        {
            this.DisplayedCopies.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<BookCopyViewModel> displayedCopies = this.AllCopies.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.AllCopies.Count;

            foreach (BookCopyViewModel vm in displayedCopies)
            {
                this.DisplayedCopies.Add(vm);
            }
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
            if (this.owner != null && !this.isInPreviewMode)
            {
                this.Commands.Add(new CommandViewModel("New...", new DelegateCommand(c => this.CreateNewCopy())));
                this.Commands.Add(new CommandViewModel("Edit...", new DelegateCommand(c => this.EditBookCopy())));
                this.Commands.Add(new CommandViewModel("Remove", new DelegateCommand(c => this.ArchiveBookCopy())));
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
        /// Creates a list of book copies as view models.
        /// </summary>
        private void CreateAllCopies()
        {
            IEnumerable<BookCopyViewModel> copies = null;

            if (this.owner == null)
            {
                copies =
                    from copy in this.repository.GetCopies()
                    where !copy.IsArchived
                    select new BookCopyViewModel(copy, this.repository);
            }
            else
            {
                copies =
                    from copy in this.repository.GetCopies()
                    where copy.Book == this.owner && !copy.IsArchived
                    select new BookCopyViewModel(copy, this.repository);
            }

            this.AllCopies = new ObservableCollection<BookCopyViewModel>(copies);
        }

        /// <summary>
        /// Deletes a copy.
        /// </summary>
        private void ArchiveBookCopy()
        {
            BookCopyViewModel viewModel = this.AllCopies.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected copy?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.repository.ArchiveCopy(viewModel.Copy);
                }
            }
            else
            {
                MessageBox.Show("Please select a single copy.");
            }

            this.RebuildPageData();
        }

        /// <summary>
        /// Edit the book copy.
        /// </summary>
        private void EditBookCopy()
        {
            BookCopyViewModel viewModel = this.AllCopies.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                this.ShowBookCopy(viewModel, "Edit");
            }
            else
            {
                MessageBox.Show("Please first select a book copy.");
            }
        }

        /// <summary>
        /// Show the book copy.
        /// </summary>
        /// <param name="viewModel">The parameter book copy view model.</param>
        /// <param name="windowFunction">The function that the window will serve (creating or editing).</param>
        private void ShowBookCopy(BookCopyViewModel viewModel, string windowFunction)
        {
            // Create and configure window.
            WorkspaceWindow window = new WorkspaceWindow();
            window.Title = windowFunction + " " + viewModel.DisplayName;

            // Create and configure view.
            BookCopyView view = new BookCopyView();
            view.DataContext = viewModel;
            viewModel.CloseAction = b => window.DialogResult = b;

            window.Content = view;

            window.ShowDialog();
        }

        /// <summary>
        /// Handles the event of a copy being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCopyAdded(object sender, BookCopyEventArgs e)
        {
            BookCopyViewModel viewModel = new BookCopyViewModel(e.Copy, this.repository);
            this.AllCopies.Add(viewModel);
            this.RebuildPageData();
        }

        /// <summary>
        /// Event when a copy is removed.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments for the event.</param>
        private void OnCopyArchived(object sender, BookCopyEventArgs e)
        {
            BookCopyViewModel viewModel = this.AllCopies.FirstOrDefault(vm => vm.Copy == e.Copy);

            if (viewModel != null)
            {
                this.AllCopies.Remove(viewModel);
                this.RebuildPageData();
            }
        }
    }
}