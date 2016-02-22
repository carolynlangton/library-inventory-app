using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model for all of a member's transactions.
    /// </summary>
    public class MultiMemberTransactionViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's member.
        /// </summary>
        private Member member;

        /// <summary>
        /// The view model's repository.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's representation of a transaction's multiple lines.
        /// </summary>
        private MultiTransactionDetailViewModel detailViewModel;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's repository.</param>
        /// <param name="member">The view model's member.</param>
        public MultiMemberTransactionViewModel(Repository repository, Member member)
            : base("All transactions")
        {
            this.member = member;
            this.repository = repository;

            this.CreateAllMemberTransactions();

            this.repository.TransactionAdded += this.OnTransactionAdded;
            this.AllTransactions.ToList().ForEach(t => t.TransactionSelected += this.OnTransactionSelected);
        }

        /// <summary>
        /// Gets or sets the number of transactions selected.
        /// </summary>
        public int NumberOfTransactionsSelected { get; set; }

        /// <summary>
        /// Gets the view model's list of transactions.
        /// </summary>
        public ObservableCollection<TransactionViewModel> AllTransactions { get; private set; }

        /// <summary>
        /// Gets the detail view model.
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
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
        }

        /// <summary>
        /// Creates a list of transactions as view models.
        /// </summary>
        private void CreateAllMemberTransactions()
        {
            IEnumerable<TransactionViewModel> transactions =
                from t in this.repository.GetTransactions()
                where t.Member.Id == this.member.Id
                select new TransactionViewModel(t, this.repository);

            this.AllTransactions = new ObservableCollection<TransactionViewModel>(transactions);
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
        }

        /// <summary>
        /// Handles the event of a transaction being selected.s
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private void OnTransactionSelected(object sender, TransactionEventArgs e)
        {
            this.DetailViewModel = new MultiTransactionDetailViewModel(e.Transaction, this.repository);
        }
    }
}