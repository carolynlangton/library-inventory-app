using System;
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
    /// The class that represents the view model for the window for the member user interface.
    /// </summary>
    public class MemberMainWindowViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The member who is currently logged in.
        /// </summary>
        private Member member;

        /// <summary>
        /// The view model's repository.
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
        /// <param name="member">The member who is logged in.</param>
        /// <param name="repository">The view model's repository.</param>
        /// <param name="controlViewModel">The control window view model.</param>
        /// <param name="loginViewModel">The login view model.</param>
        public MemberMainWindowViewModel(Member member, Repository repository, ControlWindowViewModel controlViewModel, LoginViewModel loginViewModel)
            : base("Library Rental System")
        {
            this.member = member;
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
        /// Gets the member.
        /// </summary>
        public Member Member
        {
            get
            {
                return this.member;
            }
        }

        /// <summary>
        /// Gets the displayed greeting to the currently logged in member.
        /// </summary>
        public string MemberGreeting
        {
            get
            {
                return string.Format("Hello, {0}!", this.Member.FirstName);
            }
        }

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
        /// Button commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("Current books", "Actions", new DelegateCommand(p => this.ViewCurrentCopies())));
            this.Commands.Add(new CommandViewModel("Check out books", "Actions", new DelegateCommand(p => this.CheckOutCopies())));
            this.Commands.Add(new CommandViewModel("View history", "Actions", new DelegateCommand(p => this.ViewMemberTransactions())));
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
        /// Shows the book copies that the logged in member currently has checked out.
        /// </summary>
        private void ViewCurrentCopies()
        {
            MultiMemberBookCopyViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiMemberBookCopyViewModel) as MultiMemberBookCopyViewModel;

            if (workspace == null)
            {
                workspace = new MultiMemberBookCopyViewModel(this.member, this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Allows the logged in member to check out book copies.
        /// </summary>
        private void CheckOutCopies()
        {
            CheckOutCopyViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is CheckOutCopyViewModel) as CheckOutCopyViewModel;

            if (workspace == null)
            {
                workspace = new CheckOutCopyViewModel(this.member, this.repository);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }

        /// <summary>
        /// Allows the logged in member to view all of their transactions.
        /// </summary>
        private void ViewMemberTransactions()
        {
            MultiMemberTransactionViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is MultiMemberTransactionViewModel) as MultiMemberTransactionViewModel;

            if (workspace == null)
            {
                workspace = new MultiMemberTransactionViewModel(this.repository, this.member);

                workspace.RequestClose += this.OnWorkspaceRequestClose;

                this.Workspaces.Add(workspace);
            }

            this.ActivateWorkspace(workspace);
        }
    }
}