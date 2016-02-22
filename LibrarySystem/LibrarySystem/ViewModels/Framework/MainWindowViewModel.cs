using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using LibraryDataAccess;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of a main window.
    /// </summary>
    public class MainWindowViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's collection of workspaces.
        /// </summary>
        private ObservableCollection<WorkspaceViewModel> workspaces;

        /// <summary>
        /// Main window view model's control view model.
        /// </summary>
        private ControlWindowViewModel controlViewModel;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The database.</param>
        /// <param name="controlViewModel">The control window view model.</param>
        /// <param name="loginViewModel">The login view model.</param>
        public MainWindowViewModel(Repository repository, ControlWindowViewModel controlViewModel, LoginViewModel loginViewModel)
            : base("Library Inventory System")
        {
            this.repository = repository;

            this.controlViewModel = controlViewModel;

            LoginView loginView = new LoginView();
            loginView.DataContext = loginViewModel;

            this.LogoutCommand = new DelegateCommand(p => this.controlViewModel.ControlWindowContent = loginView);
        }

        /// <summary>
        /// Gets or sets the log out command.
        /// </summary>
        public ICommand LogoutCommand { get; set; }

        /// <summary>
        /// Gets or sets the view model's collection of workspaces.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                // Use lazy instantiation pattern to create upon first request
                if (this.workspaces == null)
                {
                    this.workspaces = new ObservableCollection<WorkspaceViewModel>();
                }

                return this.workspaces;
            }
            set
            {
                this.workspaces = value;
            }
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("View books", "Inventory", new DelegateCommand(p => this.ShowAllBooks())));
            this.Commands.Add(new CommandViewModel("View members", "Actions", new DelegateCommand(p => this.ShowAllMembers())));
            this.Commands.Add(new CommandViewModel("View transactions", "Actions", new DelegateCommand(p => this.ShowAllTransactions())));
            this.Commands.Add(new CommandViewModel("Return books", "Actions", new DelegateCommand(p => this.ReturnBooks())));
            this.Commands.Add(new CommandViewModel("View authors", "Book Lookups", new DelegateCommand(p => this.ShowAllAuthors())));
            this.Commands.Add(new CommandViewModel("View formats", "Book Lookups", new DelegateCommand(p => this.ShowAllFormats())));
            this.Commands.Add(new CommandViewModel("View genres", "Book Lookups", new DelegateCommand(p => this.ShowAllGenres())));
            this.Commands.Add(new CommandViewModel("View publishers", "Book Lookups", new DelegateCommand(p => this.ShowAllPublishers())));
            this.Commands.Add(new CommandViewModel("Overdue books", "Reports", new DelegateCommand(p => this.ShowReport("Overdue books"))));
            this.Commands.Add(new CommandViewModel("Popular authors", "Reports", new DelegateCommand(p => this.ShowReport("Popular authors"))));
            this.Commands.Add(new CommandViewModel("Popular formats", "Reports", new DelegateCommand(p => this.ShowReport("Popular formats"))));
        }

        /// <summary>
        /// Shifts the workspace in focus to the current workspace.
        /// </summary>
        /// <param name="workspace">The current workspace on which to focus.</param>
        private void ActivateWorkspace(WorkspaceViewModel workspace)
        {
            // Get the view associated with the view model
            ICollectionView view = CollectionViewSource.GetDefaultView(this.Workspaces);

            if (view != null)
            {
                // Make view current
                view.MoveCurrentTo(workspace);
            }
        }

        /// <summary>
        /// Handles the event that the user wants to close a workspace.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            this.workspaces.Remove(sender as WorkspaceViewModel);
        }

        /// <summary>
        /// Shows all authors.
        /// </summary>
        private void ShowAllAuthors()
        {
            // Try to find existing opened tab
            MultiAuthorViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiAuthorViewModel) as MultiAuthorViewModel;

            if (workspace == null)
            {
                workspace = new MultiAuthorViewModel(this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows all books.
        /// </summary>
        private void ShowAllBooks()
        {
            // Try to find existing opened tab
            MultiBookViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiBookViewModel) as MultiBookViewModel;

            if (workspace == null)
            {
                workspace = new MultiBookViewModel(this.repository, false);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows all members.
        /// </summary>
        private void ShowAllMembers()
        {
            // Try to find existing opened tab
            MultiMemberViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiMemberViewModel) as MultiMemberViewModel;

            if (workspace == null)
            {
                workspace = new MultiMemberViewModel(this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows all publishers.
        /// </summary>
        private void ShowAllPublishers()
        {
            // Try to find existing opened tab
            MultiPublisherViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiPublisherViewModel) as MultiPublisherViewModel;

            if (workspace == null)
            {
                workspace = new MultiPublisherViewModel(this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows all copies of books.
        /// </summary>
        private void ShowAllCopies()
        {
            // Try to find existing opened tab
            MultiBookCopyViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiBookCopyViewModel) as MultiBookCopyViewModel;

            if (workspace == null)
            {
                workspace = new MultiBookCopyViewModel(this.repository, null, false);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows all book formats.
        /// </summary>
        private void ShowAllFormats()
        {
            // Try to find existing opened tab
            MultiFormatViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiFormatViewModel) as MultiFormatViewModel;

            if (workspace == null)
            {
                workspace = new MultiFormatViewModel(this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows all genres of books.
        /// </summary>
        private void ShowAllGenres()
        {
            // Try to find existing opened tab.
            MultiGenreViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiGenreViewModel) as MultiGenreViewModel;

            if (workspace == null)
            {
                workspace = new MultiGenreViewModel(this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows all transactions.
        /// </summary>
        private void ShowAllTransactions()
        {
            // Try to find existing opened tab
            MultiTransactionViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiTransactionViewModel) as MultiTransactionViewModel;

            if (workspace == null)
            {
                workspace = new MultiTransactionViewModel(this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows all returnable books.
        /// </summary>
        private void ReturnBooks()
        {
            // Try to find existing opened tab
            ReturnCopyViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is ReturnCopyViewModel) as ReturnCopyViewModel;

            if (workspace == null)
            {
                workspace = new ReturnCopyViewModel(this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Shows a report.
        /// </summary>
        /// <param name="reportType">The type of the report to show.</param>
        private void ShowReport(string reportType)
        {
            // Try to find existing opened tab
            ReportViewModel report = this.Workspaces.FirstOrDefault(vm => vm is ReportViewModel && vm.DisplayName == reportType) as ReportViewModel;

            IEnumerable<object> collection = null;

            if (reportType == "Overdue books")
            {
                collection =
                from copy in this.repository.GetCopies()
                where copy.TransactionDetails.Count > 0 &&
                copy.TransactionDetails.Last().CheckInDate == null
                && copy.TransactionDetails.Last().DueDate < DateTime.Today
                select new { copy.Book.Title, copy.TransactionDetails.OrderBy(td => td.DueDate).Last().DueDate, copy.TransactionDetails.OrderBy(td => td.DueDate).Last().Transaction.Member };
            }
            else if (reportType == "Popular authors")
            {
                collection =
                    from td in this.repository.GetTransactionDetails()
                    group td by td.Copy.Book.Author.ToString() into author
                    orderby author.Count() descending
                    select new { Author = author.Key, NumberOfBooksCheckedOut = author.Count() };
            }
            else if (reportType == "Popular formats")
            {
                collection =
                    from td in this.repository.GetTransactionDetails()
                    group td by td.Copy.Format.Type into format
                    orderby format.Count() descending
                    select new { Format = format.Key, NumberOfBooksCheckedOut = format.Count() };
            }

            if (report == null)
            {
                report = new ReportViewModel(this.repository, collection, reportType);

                report.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(report);
            }

            this.ActivateWorkspace(report);
        }
    }
}