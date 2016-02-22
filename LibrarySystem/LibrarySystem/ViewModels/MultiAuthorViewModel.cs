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
    /// The class which describes a view model of many authors.
    /// </summary>
    public class MultiAuthorViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The multi-author view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's view source.
        /// </summary>
        private CollectionViewSource authorViewSource;

        /// <summary>
        /// The name of the sort column.
        /// </summary>
        private string sortColumnName;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The collection of author view model.
        /// </summary>
        private ObservableCollection<AuthorViewModel> displayedAuthors;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's database.</param>
        public MultiAuthorViewModel(Repository repository)
            : base("View all authors")
        {
            this.repository = repository;

            this.DisplayedAuthors = new ObservableCollection<AuthorViewModel>();
            this.authorViewSource = new CollectionViewSource();
            this.authorViewSource.Source = this.DisplayedAuthors;

            this.SortCommand = new DelegateCommand(this.Sort);

            this.CreateAllAuthors();

            this.repository.AuthorAdded += this.OnAuthorAdded;
            this.repository.AuthorArchived += this.OnAuthorArchived;

            this.Pager = new PagingViewModel(this.AllAuthors.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets or sets the number of authors selected.
        /// </summary>
        public int NumberOfAuthorsSelected { get; set; }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets the view model's list of authors.
        /// </summary>
        public ObservableCollection<AuthorViewModel> AllAuthors { get; private set; }

        /// <summary>
        /// Gets the collection of authors.
        /// </summary>
        public ObservableCollection<AuthorViewModel> DisplayedAuthors
        {
            get
            {
                return this.displayedAuthors;
            }
            private set
            {
                this.displayedAuthors = value;

                // Create and link to collection view source.
                this.authorViewSource = new CollectionViewSource();
                this.authorViewSource.Source = this.DisplayedAuthors;
            }
        }

        /// <summary>
        /// Gets the list collection view of authors.
        /// </summary>
        public ListCollectionView SortedAuthors
        {
            get
            {
                return this.authorViewSource.View as ListCollectionView;
            }
        }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets a value indicating whether one or more author is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                bool result = false;
                int selectedCount = 0;

                foreach (AuthorViewModel avm in this.AllAuthors)
                {
                    if (avm.IsSelected != false)
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
        /// Gets a value indicating whether an author view model is selected.
        /// </summary>
        public bool IsSelectedToRomove
        {
            get
            {
                bool result = false;

                foreach (AuthorViewModel avm in this.AllAuthors)
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
            this.authorViewSource.SortDescriptions.Clear();
            this.authorViewSource.SortDescriptions.Add(new SortDescription(this.sortColumnName, this.sortDirection));
        }

        /// <summary>
        /// Rebuild data page.
        /// </summary>
        public void RebuildPageData()
        {
            this.DisplayedAuthors.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<AuthorViewModel> displayedMakes = this.AllAuthors.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.AllAuthors.Count;

            foreach (AuthorViewModel avm in displayedMakes)
            {
                this.DisplayedAuthors.Add(avm);
            }
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("New...", new DelegateCommand(a => this.CreateNewAuthor())));
            this.Commands.Add(new CommandViewModel("Edit...", new DelegateCommand(a => this.EditAuthor(), a => this.IsSelected)));
            this.Commands.Add(new CommandViewModel("Remove", new DelegateCommand(a => this.ArchiveAuthor(), a => this.IsSelectedToRomove)));
        }

        /// <summary>
        /// Creates a new author.
        /// </summary>
        private void CreateNewAuthor()
        {
            // Create view model for author.
            AuthorViewModel viewModel = new AuthorViewModel(new Author(), this.repository);

            this.ShowAuthor(viewModel, "Create");
        }

        /// <summary>
        /// Rebuild page data on page change.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event current page change event argument.</param>
        private void OnPageChange(object sender, CurrentPageChangeEventArgs e)
        {
            this.RebuildPageData();
        }

        /// <summary>
        /// Creates a list of view models for each author in the repository.
        /// </summary>
        private void CreateAllAuthors()
        {
            // Get a list of view models for each author in the database.
            IEnumerable<AuthorViewModel> authors =
                from author in this.repository.GetAuthors()
                where !author.IsArchived
                select new AuthorViewModel(author, this.repository);

            // Create observable collection from list
            this.AllAuthors = new ObservableCollection<AuthorViewModel>(authors);
        }

        /// <summary>
        /// Deletes an author.
        /// </summary>
        private void ArchiveAuthor()
        {
            AuthorViewModel viewModel = this.AllAuthors.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected author", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.repository.ArchiveAuthor(viewModel.Author);
                }
                else
                {
                    MessageBox.Show("Please select a single author.");
                }
            }

            this.RebuildPageData();
        }

        /// <summary>
        /// Edit the author.
        /// </summary>
        private void EditAuthor()
        {
            AuthorViewModel viewModel = this.AllAuthors.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                this.ShowAuthor(viewModel, "Edit");
            }
            else
            {
                MessageBox.Show("Please first select an author.");
            }
        }

        /// <summary>
        /// Show the author.
        /// </summary>
        /// <param name="viewModel">The parameter author view model.</param>
        /// <param name="windowFunction">The function that the window will serve (creating or editing).</param>
        private void ShowAuthor(AuthorViewModel viewModel, string windowFunction)
        {
            //// Create and configure window
            WorkspaceWindow window = new WorkspaceWindow();
            window.Title = windowFunction + " " + viewModel.DisplayName;

            //// Create and configure view
            AuthorView view = new AuthorView();
            view.DataContext = viewModel;
            viewModel.CloseAction = b => window.DialogResult = b;

            window.Content = view;

            window.ShowDialog();
        }

        /// <summary>
        /// Handles the event of an author being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnAuthorAdded(object sender, AuthorEventArgs e)
        {
            AuthorViewModel viewModel = new AuthorViewModel(e.Author, this.repository);

            this.AllAuthors.Add(viewModel);
            this.RebuildPageData();
        }

        /// <summary>
        /// Remove the selected view model.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The author event argument.</param>
        private void OnAuthorArchived(object sender, AuthorEventArgs e)
        {
            AuthorViewModel viewModel = this.AllAuthors.FirstOrDefault(vm => vm.Author == e.Author);

            if (viewModel != null)
            {
                this.AllAuthors.Remove(viewModel);
                this.RebuildPageData();
            }
        }
    }
}