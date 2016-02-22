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
    /// The class which describes a view model of many book formats.
    /// </summary>
    public class MultiFormatViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's format view source.
        /// </summary>
        private CollectionViewSource formatViewSource;

        /// <summary>
        /// The sort column's name.
        /// </summary>
        private string sortColumnName;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The multi view model's collection of format.
        /// </summary>
        private ObservableCollection<FormatViewModel> displayedFormats;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's database.</param>
        public MultiFormatViewModel(Repository repository)
            : base("View all formats")
        {
            this.repository = repository;

            this.DisplayedFormats = new ObservableCollection<FormatViewModel>();
            this.formatViewSource = new CollectionViewSource();
            this.formatViewSource.Source = this.DisplayedFormats;

            this.SortCommand = new DelegateCommand(this.Sort);

            this.CreateAllFormats();

            this.repository.FormatAdded += this.OnFormatAdded;
            this.repository.FormatArchived += this.OnFormatArchived;

            this.Pager = new PagingViewModel(this.AllFormats.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets the list collection of sorted formats.
        /// </summary>
        public ListCollectionView SortedFormats
        {
            get
            {
                return this.formatViewSource.View as ListCollectionView;
            }
        }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the collection of formats.
        /// </summary>
        public ObservableCollection<FormatViewModel> DisplayedFormats
        {
            get
            {
                return this.displayedFormats;
            }
            private set
            {
                this.displayedFormats = value;

                // Create and link to collection view source.
                this.formatViewSource = new CollectionViewSource();
                this.formatViewSource.Source = this.DisplayedFormats;
            }
        }

        /// <summary>
        /// Gets or sets the number of formats selected.
        /// </summary>
        public int NumberOfFormatsSelected { get; set; }

        /// <summary>
        /// Gets the view model's list of book formats.
        /// </summary>
        public ObservableCollection<FormatViewModel> AllFormats { get; private set; }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets a value indicating whether one or more format is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                bool result = false;
                int selectedCount = 0;

                foreach (FormatViewModel avm in this.AllFormats)
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
        /// Gets a value indicating whether an format view model is selected.
        /// </summary>
        public bool IsSelectedToRomove
        {
            get
            {
                bool result = false;

                foreach (FormatViewModel avm in this.AllFormats)
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
            this.formatViewSource.SortDescriptions.Clear();
            this.formatViewSource.SortDescriptions.Add(new SortDescription(this.sortColumnName, this.sortDirection));
        }

        /// <summary>
        /// Rebuild data page.
        /// </summary>
        public void RebuildPageData()
        {
            this.DisplayedFormats.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<FormatViewModel> displayedFomats = this.AllFormats.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.AllFormats.Count;

            foreach (FormatViewModel vm in displayedFomats)
            {
                this.DisplayedFormats.Add(vm);
            }
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("New...", new DelegateCommand(f => this.CreateNewFormat())));
            this.Commands.Add(new CommandViewModel("Edit...", new DelegateCommand(f => this.EditFormat(), f => this.IsSelected)));
            this.Commands.Add(new CommandViewModel("Remove", new DelegateCommand(f => this.ArchiveFormat(), f => this.IsSelectedToRomove)));
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
        /// Creates a new format type for a book.
        /// </summary>
        private void CreateNewFormat()
        {
            // Create view model for format.
            FormatViewModel viewModel = new FormatViewModel(new Format(), this.repository);

            this.ShowFormat(viewModel, "Create");
        }

        /// <summary>
        /// Creates a list of formats as view models.
        /// </summary>
        private void CreateAllFormats()
        {
            // Get a list of view models for each book format in the database.
            IEnumerable<FormatViewModel> formats =
                from format in this.repository.GetFormats()
                where !format.IsArchived
                select new FormatViewModel(format, this.repository);

            // Create observable collection from list
            this.AllFormats = new ObservableCollection<FormatViewModel>(formats);
        }

        /// <summary>
        /// Deletes a format.
        /// </summary>
        private void ArchiveFormat()
        {
            FormatViewModel viewModel = this.AllFormats.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected format?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.repository.ArchiveFormat(viewModel.Format);
                }
            }
            else
            {
                MessageBox.Show("Please select a single format.");
            }

            this.RebuildPageData();
        }

        /// <summary>
        /// Edit the format.
        /// </summary>
        private void EditFormat()
        {
            FormatViewModel viewModel = this.AllFormats.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                this.ShowFormat(viewModel, "Edit");
            }
            else
            {
                MessageBox.Show("Please first select a format.");
            }
        }

        /// <summary>
        /// Show the format.
        /// </summary>
        /// <param name="viewModel">The parameter format view model.</param>
        /// <param name="windowFunction">The function that the window will serve (creating or editing).</param>
        private void ShowFormat(FormatViewModel viewModel, string windowFunction)
        {
            // Create and configure window.
            WorkspaceWindow window = new WorkspaceWindow();
            window.Title = windowFunction + " " + viewModel.DisplayName;

            // Create and configure view.
            FormatView view = new FormatView();
            view.DataContext = viewModel;
            viewModel.CloseAction = b => window.DialogResult = b;

            window.Content = view;

            window.ShowDialog();
        }

        /// <summary>
        /// Handles the event of a book format being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnFormatAdded(object sender, FormatEventArgs e)
        {
            FormatViewModel viewModel = new FormatViewModel(e.Format, this.repository);

            this.AllFormats.Add(viewModel);
            this.RebuildPageData();
        }

        /// <summary>
        /// Remove the selected view model.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The author event argument.</param>
        private void OnFormatArchived(object sender, FormatEventArgs e)
        {
            FormatViewModel viewModel = this.AllFormats.FirstOrDefault(vm => vm.Format == e.Format);

            if (viewModel != null)
            {
                this.AllFormats.Remove(viewModel);
                this.RebuildPageData();
            }
        }
    }
}