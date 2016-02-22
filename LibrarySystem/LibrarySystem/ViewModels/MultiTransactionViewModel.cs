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
    /// The class that represents a view model of many transactions.
    /// </summary>
    public class MultiTransactionViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The collection of transaction view source.
        /// </summary>
        private CollectionViewSource transactionViewSource;

        /// <summary>
        /// The sort column name.
        /// </summary>
        private string sortColumnName;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The view model's representation of the transaction's multiple lines.
        /// </summary>
        private MultiTransactionDetailViewModel detailViewModel;

        /// <summary>
        /// The multi view model's collection of transactions.
        /// </summary>
        private ObservableCollection<TransactionViewModel> displayedTransactions;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's database.</param>
        public MultiTransactionViewModel(Repository repository)
            : base("All transactions")
        {
            this.repository = repository;

            this.DisplayedTransactions = new ObservableCollection<TransactionViewModel>();
            this.transactionViewSource = new CollectionViewSource();
            this.transactionViewSource.Source = this.DisplayedTransactions;

            this.SortCommand = new DelegateCommand(this.Sort);

            this.CreateAllTransactions();

            this.repository.TransactionAdded += this.OnTransactionAdded;
            this.AllTransactions.ToList().ForEach(t => t.TransactionSelected += this.OnTransactionSelected);

            this.Pager = new PagingViewModel(this.AllTransactions.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets the list collection of sorted transactions.
        /// </summary>
        public ListCollectionView SortedTransactions
        {
            get
            {
                return this.transactionViewSource.View as ListCollectionView;
            }
        }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the collection of makes.
        /// </summary>
        public ObservableCollection<TransactionViewModel> DisplayedTransactions
        {
            get
            {
                return this.displayedTransactions;
            }
            private set
            {
                this.displayedTransactions = value;

                // Create and link to collection view source.
                this.transactionViewSource = new CollectionViewSource();
                this.transactionViewSource.Source = this.DisplayedTransactions;
            }
        }

        /// <summary>
        /// Gets or sets the number of transactions selected.
        /// </summary>
        public int NumberOfTransactionsSelected { get; set; }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets the view model's list of transactions.
        /// </summary>
        public ObservableCollection<TransactionViewModel> AllTransactions { get; private set; }

        /// <summary>
        /// Gets the view model's representation of a transaction's multiple details.
        /// </summary>
        public MultiTransactionDetailViewModel DetailViewModel
        {
            get
            {
                return this.detailViewModel;
            }
            private set
            {
                this.detailViewModel = value;
                this.OnPropertyChanged("DetailViewModel");
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
            this.transactionViewSource.SortDescriptions.Clear();
            this.transactionViewSource.SortDescriptions.Add(new SortDescription(this.sortColumnName, this.sortDirection));
        }

        /// <summary>
        /// Rebuild the data page.
        /// </summary>
        public void RebuildPageData()
        {
            this.DisplayedTransactions.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<TransactionViewModel> displayedTransactions = this.AllTransactions.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.AllTransactions.Count;

            foreach (TransactionViewModel vm in displayedTransactions)
            {
                this.DisplayedTransactions.Add(vm);
            }
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("New", new DelegateCommand(t => this.CreateNewTransaction())));
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
        /// Creates a new transaction.
        /// </summary>
        private void CreateNewTransaction()
        {
            // Create view model for book
            TransactionViewModel viewModel = new TransactionViewModel(new Transaction(), this.repository);

            this.ShowTransaction(viewModel);
        }

        /// <summary>
        /// Creates a list of transactions as view models.
        /// </summary>
        private void CreateAllTransactions()
        {
            IEnumerable<TransactionViewModel> transactions =
                from t in this.repository.GetTransactions()
                select new TransactionViewModel(t, this.repository);

            this.AllTransactions = new ObservableCollection<TransactionViewModel>(transactions);
        }

        /// <summary>
        /// Edit the transaction.
        /// </summary>
        private void EditTransaction()
        {
            TransactionViewModel viewModel = this.AllTransactions.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                this.ShowTransaction(viewModel);
            }
            else
            {
                MessageBox.Show("Please first select a transaction.");
            }
        }

        /// <summary>
        /// Show the transaction.
        /// </summary>
        /// <param name="viewModel">The parameter transaction view model.</param>
        private void ShowTransaction(TransactionViewModel viewModel)
        {
            // Create and configure window.
            WorkspaceWindow window = new WorkspaceWindow();
            window.Title = viewModel.DisplayName;

            // Create and configure view.
            TransactionView view = new TransactionView();
            view.DataContext = viewModel;
            viewModel.CloseAction = b => window.DialogResult = b;

            window.Content = view;

            window.ShowDialog();
        }

        /// <summary>
        /// Handles the event of a transaction being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTransactionAdded(object sender, TransactionEventArgs e)
        {
            TransactionViewModel viewModel = new TransactionViewModel(e.Transaction, this.repository);
            viewModel.TransactionSelected += this.OnTransactionSelected;
            this.AllTransactions.Add(viewModel);
            this.RebuildPageData();
        }

        /// <summary>
        /// Handles the event of a transaction being selected.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private void OnTransactionSelected(object sender, TransactionEventArgs e)
        {
            this.DetailViewModel = new MultiTransactionDetailViewModel(e.Transaction, this.repository);
        }
    }
}