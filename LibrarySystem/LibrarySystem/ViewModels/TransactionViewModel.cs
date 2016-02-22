using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of a transaction.
    /// </summary>
    public class TransactionViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's transaction.
        /// </summary>
        private Transaction transaction;

        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's is selected boolean.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// The view model's representation of multiple book copies.
        /// </summary>
        private CheckOutCopyViewModel checkOutCopyViewModel;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="transaction">The view model's transaction.</param>
        /// <param name="repository">The view model's database.</param>
        public TransactionViewModel(Transaction transaction, Repository repository)
            : base("Create Transaction")
        {
            this.transaction = transaction;
            this.repository = repository;
            this.checkOutCopyViewModel = new CheckOutCopyViewModel(null, this.repository);
        }

        /// <summary>
        /// Raises an event when a transaction is selected.
        /// </summary>
        public event EventHandler<TransactionEventArgs> TransactionSelected;

        /// <summary>
        /// Gets or sets the Member of the book copy.
        /// </summary>
        public Member Member
        {
            get
            {
                return this.transaction.Member;
            }
            set
            {
                if (value.HasOverdueBooks)
                {
                    MessageBox.Show("This member has overdue books and\ncannot check out any more materials.");
                }
                else
                {
                    this.transaction.Member = value;
                    this.CheckOutCopyViewModel.Member = value;

                    this.OnPropertyChanged("Member");
                }
            }
        }

        /// <summary>
        /// Gets a list of Members.
        /// </summary>
        public IEnumerable<Member> Members
        {
            get
            {
                return this.repository.GetMembers().OrderBy(m => m.ToString());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                this.isSelected = value;

                if (value)
                {
                    if (this.TransactionSelected != null)
                    {
                        this.TransactionSelected(this, new TransactionEventArgs(this.transaction));
                    }
                }

                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets the number of items in the view model's transaction.
        /// </summary>
        public int NumberOfCopies
        {
            get
            {
                return this.transaction.TransactionDetails.Count;
            }
        }

        /// <summary>
        /// Gets or sets the transaction's ID.
        /// </summary>
        public int Id
        {
            get
            {
                return this.transaction.Id;
            }
            set
            {
                this.transaction.Id = value;
                this.OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets the transaction's check out date.
        /// </summary>
        public string CheckOutDate
        {
            get
            {
                return this.transaction.CheckOutDate.ToString("MMM. d");
            }
            set
            {
                this.transaction.CheckOutDate = DateTime.Parse(value);
                this.OnPropertyChanged("CheckOutDate");
            }
        }

        /// <summary>
        /// Gets the due date of the items that are part of the view model's transaction.
        /// </summary>
        public string DueDate
        {
            get
            {
                return this.transaction.TransactionDetails.First().DueDate.ToString("MMM. d");
            }
        }

        /// <summary>
        /// Gets the view model's representation of multiple book copies.
        /// </summary>
        public CheckOutCopyViewModel CheckOutCopyViewModel
        {
            get
            {
                return this.checkOutCopyViewModel;
            }
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("Cancel", new DelegateCommand(p => this.CancelChanges())));
        }

        /// <summary>
        /// Do not accept the changes made and close.
        /// </summary>
        private void CancelChanges()
        {
            if (this.CloseAction != null)
            {
                this.CloseAction(false);
            }
        }
    }
}