using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a return copy.
    /// </summary>
    public class ReturnCopyViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The return copy view model's repository.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The collection of return copies view source.
        /// </summary>
        private CollectionViewSource returnCopyViewSource;

        /// <summary>
        /// The sort column name.
        /// </summary>
        private string sortColumnName;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The return copy view model's filtered book copy view model.
        /// </summary>
        private MultiBookCopyViewModel filteredBookCopyViewModel;

        /// <summary>
        /// The multi view model's collection of copies.
        /// </summary>
        private ObservableCollection<BookCopyViewModel> displayedCopies;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The parameter repository.</param>
        public ReturnCopyViewModel(Repository repository)
            : base("Return books")
        {
            this.repository = repository;

            this.DisplayedCopies = new ObservableCollection<BookCopyViewModel>();
            this.returnCopyViewSource = new CollectionViewSource();
            this.returnCopyViewSource.Source = this.DisplayedCopies;

            this.SortCommand = new DelegateCommand(this.Sort);

            this.filteredBookCopyViewModel = new MultiBookCopyViewModel(this.repository, null, false);
            this.filteredBookCopyViewModel.AllCopies = this.CheckedOutCopies;

            this.Pager = new PagingViewModel(this.CheckedOutCopies.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets the list collection of sorted copies.
        /// </summary>
        public ListCollectionView SortedCopies
        {
            get
            {
                return this.returnCopyViewSource.View as ListCollectionView;
            }
        }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the collection of makes.
        /// </summary>
        public ObservableCollection<BookCopyViewModel> DisplayedCopies
        {
            get
            {
                return this.displayedCopies;
            }
            private set
            {
                this.displayedCopies = value;

                // Create and link to collection view source.
                this.returnCopyViewSource = new CollectionViewSource();
                this.returnCopyViewSource.Source = this.DisplayedCopies;
            }
        }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets the checked out copies.
        /// </summary>
        public ObservableCollection<BookCopyViewModel> CheckedOutCopies
        {
            get
            {
                IEnumerable<BookCopyViewModel> copies =
                from copy in this.repository.GetCopies().Where(c => !c.IsAvailable && !c.IsArchived)
                select new BookCopyViewModel(copy, this.repository);

                return new ObservableCollection<BookCopyViewModel>(copies);
            }
        }

        /// <summary>
        /// Gets the filtered book copy view model.
        /// </summary>
        public MultiBookCopyViewModel FilteredBookCopyViewModel
        {
            get
            {
                return this.filteredBookCopyViewModel;
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
            this.returnCopyViewSource.SortDescriptions.Clear();
            this.returnCopyViewSource.SortDescriptions.Add(new SortDescription(this.sortColumnName, this.sortDirection));
        }

        /// <summary>
        /// Rebuild data page.
        /// </summary>
        public void RebuildPageData()
        {
            this.DisplayedCopies.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<BookCopyViewModel> displayedCopies = this.CheckedOutCopies.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.CheckedOutCopies.Count;

            foreach (BookCopyViewModel vm in displayedCopies)
            {
                this.DisplayedCopies.Add(vm);
            }
        }

        /// <summary>
        /// Copy view model's create commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("Return", new DelegateCommand(p => this.ReturnCopies())));
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
        /// Return copies.
        /// </summary>
        private void ReturnCopies()
        {
            this.Save();
        }

        /// <summary>
        /// Return the copies.
        /// </summary>
        private void Save()
        {
            // Get all of the selected copies
            List<BookCopyViewModel> viewModels = this.DisplayedCopies.Where(vm => vm.IsSelected).ToList();

            // For each selected copy...
            viewModels.ForEach(vm =>
            {
                // Set its availability to true
                vm.IsAvailable = true;

                // Set its check in date to today
                vm.Copy.TransactionDetails.OrderBy(td => td.DueDate).Last().CheckInDate = DateTime.Today;

                // Remove it from the list of checked out copies
                this.FilteredBookCopyViewModel.AllCopies.Remove(vm);
            });

            this.RebuildPageData();

            this.repository.SaveToDatabase();
        }
    }
}