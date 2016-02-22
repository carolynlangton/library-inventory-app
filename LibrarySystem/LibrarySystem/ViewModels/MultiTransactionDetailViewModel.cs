using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model for multiple transaction details.
    /// </summary>
    public class MultiTransactionDetailViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's repository.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's transaction.
        /// </summary>
        private Transaction transaction;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="transaction">The view model's transaction.</param>
        /// <param name="repository">The view model's repository.</param>
        public MultiTransactionDetailViewModel(Transaction transaction, Repository repository)
            : base("Details")
        {
            this.transaction = transaction;
            this.repository = repository;
            this.CreateAllDetails();
        }

        /// <summary>
        /// Gets a collection of all details.
        /// </summary>
        public ObservableCollection<TransactionDetailViewModel> AllDetails { get; private set; }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
        }

        /// <summary>
        /// Creates view models for all details of the view model's transaction.
        /// </summary>
        private void CreateAllDetails()
        {
            IEnumerable<TransactionDetailViewModel> details =
                from detail in this.repository.GetTransactionDetails()
                where detail.Transaction == this.transaction
                select new TransactionDetailViewModel(detail, this.repository);

            this.AllDetails = new ObservableCollection<TransactionDetailViewModel>(details);
        }
    }
}