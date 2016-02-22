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
    /// The class that represents a view model of many publisher.
    /// </summary>
    public class MultiPublisherViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The publisher view model database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The collection of publisher view source.
        /// </summary>
        private CollectionViewSource publisherViewSource;

        /// <summary>
        /// The sort column name.
        /// </summary>
        private string sortColumnName;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The multi view model's collection of publishers.
        /// </summary>
        private ObservableCollection<PublisherViewModel> displayedPublishers;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's database.</param>
        public MultiPublisherViewModel(Repository repository)
            : base("View all publishers")
        {
            this.repository = repository;

            this.DisplayedPublishers = new ObservableCollection<PublisherViewModel>();
            this.publisherViewSource = new CollectionViewSource();
            this.publisherViewSource.Source = this.DisplayedPublishers;

            this.SortCommand = new DelegateCommand(this.Sort);

            this.CreateAllPublishers();

            this.repository.PublisherAdded += this.OnPublisherAdded;
            this.repository.PublisherArchived += this.OnPublisherArchived;

            this.Pager = new PagingViewModel(this.AllPublishers.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets the list collection of sorted publishers.
        /// </summary>
        public ListCollectionView SortedPublishers
        {
            get
            {
                return this.publisherViewSource.View as ListCollectionView;
            }
        }

        /// <summary>
        /// Gets the sorted command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the collection of publishers.
        /// </summary>
        public ObservableCollection<PublisherViewModel> DisplayedPublishers
        {
            get
            {
                return this.displayedPublishers;
            }
            private set
            {
                this.displayedPublishers = value;

                // Create and link to collection view source.
                this.publisherViewSource = new CollectionViewSource();
                this.publisherViewSource.Source = this.DisplayedPublishers;
            }
        }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets a value indicating whether one or more publisher is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                bool result = false;
                int selectedCount = 0;

                foreach (PublisherViewModel avm in this.AllPublishers)
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
        /// Gets a value indicating whether a publisher is selected.
        /// </summary>
        public bool IsSelectedToRomove
        {
            get
            {
                bool result = false;

                foreach (PublisherViewModel avm in this.AllPublishers)
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
        /// Gets or sets the number of publisher selected.
        /// </summary>
        public int NumberOfPublishersSelected { get; set; }

        /// <summary>
        /// Gets the view model's list of publishers.
        /// </summary>
        public ObservableCollection<PublisherViewModel> AllPublishers { get; private set; }

        /// <summary>
        /// Creates a new publisher.
        /// </summary>
        public void CreateNewPublisher()
        {
            // Create view model for publisher
            PublisherViewModel viewModel = new PublisherViewModel(new Publisher(), this.repository);

            this.ShowPublisher(viewModel, "Create");
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
            this.publisherViewSource.SortDescriptions.Clear();
            this.publisherViewSource.SortDescriptions.Add(new SortDescription(this.sortColumnName, this.sortDirection));
        }

        /// <summary>
        /// Rebuild the data page.
        /// </summary>
        public void RebuildPageData()
        {
            this.DisplayedPublishers.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<PublisherViewModel> displayedPublishers = this.AllPublishers.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.AllPublishers.Count;

            foreach (PublisherViewModel vm in displayedPublishers)
            {
                this.DisplayedPublishers.Add(vm);
            }
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("New...", new DelegateCommand(p => this.CreateNewPublisher())));
            this.Commands.Add(new CommandViewModel("Edit...", new DelegateCommand(p => this.EditPublisher(), p => this.IsSelected)));
            this.Commands.Add(new CommandViewModel("Remove", new DelegateCommand(p => this.ArchivePublisher(), p => this.IsSelectedToRomove)));
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
        /// Creates a list of publishers as view models.
        /// </summary>
        private void CreateAllPublishers()
        {
            // Get a list of view models for each publisher in the database
            IEnumerable<PublisherViewModel> publishers =
                from publisher in this.repository.GetPublishers()
                where publisher.IsArchived != true
                select new PublisherViewModel(publisher, this.repository);

            // Create observable collection from list
            this.AllPublishers = new ObservableCollection<PublisherViewModel>(publishers);
        }

        /// <summary>
        /// Deletes a customer.
        /// </summary>
        private void ArchivePublisher()
        {
            PublisherViewModel viewModel = this.AllPublishers.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected publisher?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.repository.ArchivePublisher(viewModel.Publisher);
                }
            }
            else
            {
                MessageBox.Show("Please select a single publisher.");
            }

            this.RebuildPageData();
        }

        /// <summary>
        /// Edit the publisher.
        /// </summary>
        private void EditPublisher()
        {
            PublisherViewModel viewModel = this.AllPublishers.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                this.ShowPublisher(viewModel, "Edit");
            }
            else
            {
                MessageBox.Show("Please first select a Publisher.");
            }
        }

        /// <summary>
        /// Show the publisher.
        /// </summary>
        /// <param name="viewModel">The parameter publisher view model.</param>
        /// <param name="windowFunction">The function that the window will serve (creating or editing).</param>
        private void ShowPublisher(PublisherViewModel viewModel, string windowFunction)
        {
            // Create and configure window.
            WorkspaceWindow window = new WorkspaceWindow();
            window.Title = windowFunction + " " + viewModel.DisplayName;

            // Create and configure view.
            PublisherView view = new PublisherView();
            view.DataContext = viewModel;
            viewModel.CloseAction = b => window.DialogResult = b;

            window.Content = view;

            window.ShowDialog();
        }

        /// <summary>
        /// Handles the event of a publisher being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPublisherAdded(object sender, PublisherEventArgs e)
        {
            PublisherViewModel viewModel = new PublisherViewModel(e.Publisher, this.repository);
            this.AllPublishers.Add(viewModel);
            this.RebuildPageData();
        }

        /// <summary>
        /// Remove the selected view model.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The author event argument.</param>
        private void OnPublisherArchived(object sender, PublisherEventArgs e)
        {
            PublisherViewModel viewModel = this.AllPublishers.FirstOrDefault(vm => vm.Publisher == e.Publisher);

            if (viewModel != null)
            {
                this.AllPublishers.Remove(viewModel);
                this.RebuildPageData();
            }
        }
    }
}