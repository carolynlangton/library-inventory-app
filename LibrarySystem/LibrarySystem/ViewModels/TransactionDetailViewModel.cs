using System;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model for a transaction line.
    /// </summary>
    public class TransactionDetailViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's transaction line.
        /// </summary>
        private TransactionDetail detail;

        /// <summary>
        /// The view model's repository.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// A value indicating whether or not a line is selected.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="detail">The view model's transaction line.</param>
        /// <param name="repository">The view model's repository.</param>
        public TransactionDetailViewModel(TransactionDetail detail, Repository repository)
            : base("Detail")
        {
            this.detail = detail;
            this.repository = repository;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a line is selected.
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
                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets or sets the check in date of the view model's transaction line.
        /// </summary>
        public DateTime? CheckInDate
        {
            get
            {
                return this.detail.CheckInDate;
            }
            set
            {
                this.detail.CheckInDate = value;
                this.OnPropertyChanged("CheckInDate");
            }
        }

        /// <summary>
        /// Gets the check in date of the transaction line in a string format.
        /// </summary>
        public string CheckInDateAsString
        {
            get
            {
                if (this.detail.CheckInDate == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ((DateTime)this.detail.CheckInDate).ToString("MMM. d");
                }
            }
        }

        /// <summary>
        /// Gets or sets the due date of the view model's transaction line.
        /// </summary>
        public DateTime DueDate
        {
            get
            {
                return this.detail.DueDate;
            }
            set
            {
                this.detail.DueDate = value;
                this.OnPropertyChanged("DueDate");
            }
        }

        /// <summary>
        /// Gets the due date of the transaction line in a string format.
        /// </summary>
        public string DueDateAsString
        {
            get
            {
                return this.detail.DueDate.ToString("MMM. d");
            }
        }

        /// <summary>
        /// Gets the title of the book copy associated with the view model's transaction line.
        /// </summary>
        public string Copy
        {
            get
            {
                return this.detail.Copy.Book.Title;
            }
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
        }
    }
}