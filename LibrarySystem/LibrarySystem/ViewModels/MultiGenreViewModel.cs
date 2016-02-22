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
    /// The class which describes a view model of many genres.
    /// </summary>
    public class MultiGenreViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The collection of genre view source.
        /// </summary>
        private CollectionViewSource genreViewSource;

        /// <summary>
        /// The sort column name.
        /// </summary>
        private string sortColumnName;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The multi view model's collection of genres.
        /// </summary>
        private ObservableCollection<GenreViewModel> displayedGenres;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's database.</param>
        public MultiGenreViewModel(Repository repository)
            : base("View all genres")
        {
            this.repository = repository;

            this.DisplayedGenres = new ObservableCollection<GenreViewModel>();
            this.genreViewSource = new CollectionViewSource();
            this.genreViewSource.Source = this.DisplayedGenres;

            this.SortCommand = new DelegateCommand(this.Sort);

            this.CreateAllGenres();

            this.repository.GenreAdded += this.OnGenreAdded;
            this.repository.GenreArchived += this.OnGenreArchived;

            this.Pager = new PagingViewModel(this.AllGenres.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets the list collection of sorted  genres.
        /// </summary>
        public ListCollectionView SortedGenres
        {
            get
            {
                return this.genreViewSource.View as ListCollectionView;
            }
        }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the collection of genres.
        /// </summary>
        public ObservableCollection<GenreViewModel> DisplayedGenres
        {
            get
            {
                return this.displayedGenres;
            }
            private set
            {
                this.displayedGenres = value;

                // Create and link to collection view source.
                this.genreViewSource = new CollectionViewSource();
                this.genreViewSource.Source = this.DisplayedGenres;
            }
        }

        /// <summary>
        /// Gets or sets the number of genres selected.
        /// </summary>
        public int NumberOfGenresSelected { get; set; }

        /// <summary>
        /// Gets the view model's list of genres.
        /// </summary>
        public ObservableCollection<GenreViewModel> AllGenres { get; private set; }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets a value indicating whether one or more genre is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                bool result = false;
                int selectedCount = 0;

                foreach (GenreViewModel avm in this.AllGenres)
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
        /// Gets a value indicating whether a genre is selected.
        /// </summary>
        public bool IsSelectedToRomove
        {
            get
            {
                bool result = false;

                foreach (GenreViewModel avm in this.AllGenres)
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
            this.genreViewSource.SortDescriptions.Clear();
            this.genreViewSource.SortDescriptions.Add(new SortDescription(this.sortColumnName, this.sortDirection));
        }

        /// <summary>
        /// Rebuild the data page.
        /// </summary>
        public void RebuildPageData()
        {
            this.DisplayedGenres.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<GenreViewModel> displayedGenres = this.AllGenres.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.AllGenres.Count;

            foreach (GenreViewModel vm in displayedGenres)
            {
                this.DisplayedGenres.Add(vm);
            }
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("New...", new DelegateCommand(g => this.CreateNewGenre())));
            this.Commands.Add(new CommandViewModel("Edit...", new DelegateCommand(g => this.EditGenre(), g => this.IsSelected)));
            this.Commands.Add(new CommandViewModel("Remove", new DelegateCommand(g => this.ArchiveGenre(), g => this.IsSelectedToRomove)));
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
        /// Creates a new genre for a book.
        /// </summary>
        private void CreateNewGenre()
        {
            // Create view model for genre.
            GenreViewModel viewModel = new GenreViewModel(new Genre(), this.repository);

            this.ShowGenre(viewModel, "Create");
        }

        /// <summary>
        /// Creates a list of genres as view models.
        /// </summary>
        private void CreateAllGenres()
        {
            // Get a list of view models for each genre in the database.
            IEnumerable<GenreViewModel> genres =
                from genre in this.repository.GetGenres()
                where !genre.IsArchived
                select new GenreViewModel(genre, this.repository);

            // Create observable collection from list
            this.AllGenres = new ObservableCollection<GenreViewModel>(genres);
        }

        /// <summary>
        /// Deletes a genre.
        /// </summary>
        private void ArchiveGenre()
        {
            GenreViewModel viewModel = this.AllGenres.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected genre?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.repository.ArchiveGenre(viewModel.Genre);
                }
            }
            else
            {
                MessageBox.Show("Please select a single genre.");
            }

            this.RebuildPageData();
        }

        /// <summary>
        /// Edit the genre.
        /// </summary>
        private void EditGenre()
        {
            GenreViewModel viewModel = this.AllGenres.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                this.ShowGenre(viewModel, "Edit");
            }
            else
            {
                MessageBox.Show("Please first select a genre.");
            }
        }

        /// <summary>
        /// Show the genre.
        /// </summary>
        /// <param name="viewModel">The parameter genre view model.</param>
        /// <param name="windowFunction">The function that the window will serve (creating or editing).</param>
        private void ShowGenre(GenreViewModel viewModel, string windowFunction)
        {
            // Create and configure window.
            WorkspaceWindow window = new WorkspaceWindow();
            window.Title = windowFunction + " " + viewModel.DisplayName;

            // Create and configure view.
            GenreView view = new GenreView();
            view.DataContext = viewModel;
            viewModel.CloseAction = b => window.DialogResult = b;

            window.Content = view;

            window.ShowDialog();
        }

        /// <summary>
        /// Handles the event of a genre being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnGenreAdded(object sender, GenreEventArgs e)
        {
            GenreViewModel viewModel = new GenreViewModel(e.GenreName, this.repository);

            this.AllGenres.Add(viewModel);
            this.RebuildPageData();
        }

        /// <summary>
        /// Remove the selected view model.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The author event argument.</param>
        private void OnGenreArchived(object sender, GenreEventArgs e)
        {
            GenreViewModel viewModel = this.AllGenres.FirstOrDefault(vm => vm.Genre == e.GenreName);

            if (viewModel != null)
            {
                this.AllGenres.Remove(viewModel);
                this.RebuildPageData();
            }
        }
    }
}