using System;
using System.Diagnostics.Contracts;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a paging view model.
    /// </summary>
    public class PagingViewModel : ViewModel
    {
        /// <summary>
        /// The view model's item count.
        /// </summary>
        private int itemCount;

        /// <summary>
        /// The view model's page size.
        /// </summary>
        private int pageSize;

        /// <summary>
        /// The view model's current page.
        /// </summary>
        private int currentPage;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="itemCount">The item count.</param>
        public PagingViewModel(int itemCount)
            : base(string.Empty)
        {
            Contract.Requires(itemCount >= 0);
            Contract.Requires(this.pageSize > 0);

            this.itemCount = itemCount;
            this.pageSize = 10;
            this.currentPage = this.itemCount == 0 ? 0 : 1;

            this.GoToFirstPageCommand = new DelegateCommand(p => this.CurrentPage = 1, p => this.ItemCount > 0 && this.CurrentPage > 1);
            this.GoToPreviousPageCommand = new DelegateCommand(p => this.CurrentPage--, p => this.ItemCount > 0 && this.CurrentPage > 1);
            this.GoToNextPageCommand = new DelegateCommand(p => this.CurrentPage++, p => this.ItemCount > 0 && this.CurrentPage < this.PageCount);
            this.GoToLastPageCommand = new DelegateCommand(p => this.CurrentPage = this.PageCount, p => this.ItemCount > 0 && this.CurrentPage < this.PageCount);
        }

        /// <summary>
        /// Handles the event of the current page being changed.
        /// </summary>
        public event EventHandler<CurrentPageChangeEventArgs> CurrentPageChanged;

        /// <summary>
        /// Gets or sets the item count.
        /// </summary>
        public int ItemCount
        {
            get
            {
                return this.itemCount;
            }
            set
            {
                this.itemCount = value;
                this.OnPropertyChanged("PageSize");
                this.OnPropertyChanged("ItemCount");
                this.OnPropertyChanged("PageCount");
            }
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return this.currentPage;
            }
            set
            {
                this.currentPage = value;

                this.OnPropertyChanged("CurrentPage");

                EventHandler<CurrentPageChangeEventArgs> handler = this.CurrentPageChanged;

                if (handler != null)
                {
                    handler(this, new CurrentPageChangeEventArgs(this.CurrentPageStartIndex, this.PageSize));
                }
            }
        }

        /// <summary>
        /// Gets the current page start index.
        /// </summary>
        public int CurrentPageStartIndex
        {
            get
            {
                return this.PageCount == 0 ? -1 : (this.CurrentPage - 1) * this.PageSize;
            }
        }

        /// <summary>
        /// Gets the page count.
        /// </summary>
        public int PageCount
        {
            get
            {
                return (int)Math.Ceiling((double)this.itemCount / this.pageSize);
            }
        }

        /// <summary>
        /// Gets or sets the page size.
        /// </summary>
        public int PageSize
        {
            get
            {
                return this.pageSize;
            }
            set
            {
                this.pageSize = value;
                this.OnPropertyChanged("PageSize");
                this.OnPropertyChanged("ItemCount");
                this.OnPropertyChanged("PageCount");
            }
        }

        /// <summary>
        /// Gets or sets the event listener GoToFirstPageCommand.
        /// </summary>
        public DelegateCommand GoToFirstPageCommand { get; set; }

        /// <summary>
        /// Gets or sets the event listener GoToPreviousPageCommand.
        /// </summary>
        public DelegateCommand GoToPreviousPageCommand { get; set; }

        /// <summary>
        /// Gets or sets the event listener GoToNextPageCommand.
        /// </summary>
        public DelegateCommand GoToNextPageCommand { get; set; }

        /// <summary>
        /// Gets or sets the event listener GoToLastPageCommand.
        /// </summary>
        public DelegateCommand GoToLastPageCommand { get; set; }
    }
}