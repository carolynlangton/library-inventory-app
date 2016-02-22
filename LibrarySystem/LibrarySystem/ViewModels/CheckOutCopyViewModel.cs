using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model for checking out book copies.
    /// </summary>
    public class CheckOutCopyViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The member who will be checking out the book copies.
        /// </summary>
        private Member member;

        /// <summary>
        /// The view model's repository.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's list of book view models.
        /// </summary>
        private MultiBookViewModel multiBookViewModel;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="member">The member who will be checking out books.</param>
        /// <param name="repository">The view model's repository.</param>
        public CheckOutCopyViewModel(Member member, Repository repository)
            : base("Check out books")
        {
            this.member = member;
            this.repository = repository;
            this.Commands.Clear();
            this.CreateCommands();

            this.multiBookViewModel = new MultiBookViewModel(this.repository, true);
            this.ShoppingCartBooks = new ObservableCollection<BookCopyViewModel>();
        }

        /// <summary>
        /// Gets or sets the number of copies selected.
        /// </summary>
        public int NumberOfCopiesSelected { get; set; }

        /// <summary>
        /// Gets the view model's list of book view models.
        /// </summary>
        public MultiBookViewModel MultiBookViewModel
        {
            get
            {
                return this.multiBookViewModel;
            }
        }

        /// <summary>
        /// Gets or sets the member who is currently checking out.
        /// </summary>
        public Member Member
        {
            get
            {
                return this.member;
            }
            set
            {
                this.member = value;
            }
        }

        /// <summary>
        /// Gets the view model's current list of book copies in the shopping cart.
        /// </summary>
        public ObservableCollection<BookCopyViewModel> ShoppingCartBooks { get; private set; }

        /// <summary>
        /// Gets the workspace view model's list of commands.
        /// </summary>
        public ObservableCollection<CommandViewModel> AddCommands { get; private set; }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.AddCommands = new ObservableCollection<CommandViewModel>();

            this.AddCommands.Add(new CommandViewModel("Add to cart", new DelegateCommand(p => this.AddCopiesToCart())));
            this.Commands.Add(new CommandViewModel("Check out", new DelegateCommand(p => this.CheckOutBooks())));
        }

        /// <summary>
        /// Adds the selected copies to the shopping cart.
        /// </summary>
        private void AddCopiesToCart()
        {
            IEnumerable<BookCopyViewModel> copies = this.MultiBookViewModel.BookCopyViewModel.AllCopies.Where(vm => vm.IsSelected);

            if (copies != null)
            {
                copies.ToList().ForEach(vm => this.ShoppingCartBooks.Add(vm));
            }
        }

        /// <summary>
        /// Checks out all of the copies in the shopping cart.
        /// </summary>
        private void CheckOutBooks()
        {
            if (this.member.HasOverdueBooks)
            {
                MessageBox.Show("You have overdue books. You cannot check out more materials until you return your overdue items.");
            }
            else if (this.ValidateTransaction())
            {
                Transaction transaction = new Transaction();

                // Set the transaction's check out date to today's date
                transaction.CheckOutDate = DateTime.Today;

                transaction.Member = this.member;

                // Add the transaction to the repository
                this.repository.AddTransaction(transaction);

                // For each copy chosen...
                foreach (BookCopyViewModel vm in this.ShoppingCartBooks)
                {
                    // Create a transaction detail
                    TransactionDetail detail = new TransactionDetail();

                    // Set the due date to 21 days out from today
                    detail.DueDate = DateTime.Today.AddDays(21);

                    // Set the detail's navigation properties
                    detail.Transaction = transaction;
                    detail.Copy = vm.Copy;

                    // Add the transaction detail to the repository
                    this.repository.AddTransactionDetail(detail);

                    // Make the copy unavailable
                    vm.IsAvailable = false;

                    // Push changes.
                    this.repository.SaveToDatabase();
                }

                this.ShoppingCartBooks.Clear();

                MessageBox.Show("Books checked out successfully.");
            }
            else
            {
                MessageBox.Show("The transaction is invalid. Please choose books that are available.");
            }
        }

        /// <summary>
        /// Validates the current transaction.
        /// </summary>
        /// <returns>A value indicating whether or not the transaction is valid.</returns>
        private bool ValidateTransaction()
        {
            bool result = true;

            if (this.ShoppingCartBooks.Any(vm => !vm.IsAvailable))
            {
                result = false;
            }

            return result;
        }
    }
}