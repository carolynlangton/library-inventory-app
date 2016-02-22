using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model for book copies that a member has checked out.
    /// </summary>
    public class MultiMemberBookCopyViewModel : WorkspaceViewModel
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
        /// Initializes a new instance.
        /// </summary>
        /// <param name="member">The view model's member.</param>
        /// <param name="repository">The view model's repository.</param>
        public MultiMemberBookCopyViewModel(Member member, Repository repository)
            : base("All copies currently out")
        {
            this.member = member;
            this.repository = repository;

            this.repository.TransactionDetailAdded += this.OnTransactionDetailAdded;

            this.CreateAllCurrentlyOutCopies();
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
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
        }

        /// <summary>
        /// Creates a list of book copies as view models.
        /// </summary>
        private void CreateAllCurrentlyOutCopies()
        {
            List<TransactionDetail> details = new List<TransactionDetail>();

            foreach (Transaction t in this.member.Transactions)
            {
                foreach (TransactionDetail td in t.TransactionDetails)
                {
                    if (td.CheckInDate == null)
                    {
                        details.Add(td);
                    }
                }
            }

            List<BookCopy> checkedOutCopies = new List<BookCopy>();
            details.ForEach(d => checkedOutCopies.Add(d.Copy));

            IEnumerable<BookCopyViewModel> copies =
                from copy in checkedOutCopies
                where !copy.IsArchived
                select new BookCopyViewModel(copy, this.repository);

            this.AllCopies = new ObservableCollection<BookCopyViewModel>(copies);
        }

        /// <summary>
        /// Handles the event of a transaction detail being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments of the event.</param>
        private void OnTransactionDetailAdded(object sender, TransactionDetailEventArgs e)
        {
            BookCopyViewModel viewModel = new BookCopyViewModel(e.TransactionDetail.Copy, this.repository);
            this.AllCopies.Add(viewModel);
        }
    }
}