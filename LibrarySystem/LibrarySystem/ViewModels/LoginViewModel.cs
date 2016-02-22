using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model for logging in to the application.
    /// </summary>
    public class LoginViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The view model's repository.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The user to log in.
        /// </summary>
        private IUser user;

        /// <summary>
        /// The command to log in a member.
        /// </summary>
        private ICommand loginCommand;

        /// <summary>
        /// The command to create a new member.
        /// </summary>
        private ICommand newUserCommand;

        /// <summary>
        /// Log in view model's control view model.
        /// </summary>
        private ControlWindowViewModel controlViewModel;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="controlViewModel">The control window view model.</param>
        public LoginViewModel(ControlWindowViewModel controlViewModel)
            : base("Login")
        {
            this.repository = new Repository();
            this.controlViewModel = controlViewModel;
        }

        /// <summary>
        /// Gets or sets the username of the user attempting to log in.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets the command to log in a member.
        /// </summary>
        public ICommand LoginCommand
        {
            get
            {
                if (this.loginCommand == null)
                {
                    this.loginCommand = new DelegateCommand(p => this.LoginUser(p));
                }

                return this.loginCommand;
            }
        }

        /// <summary>
        /// Gets the command to create a new member.
        /// </summary>
        public ICommand NewUserCommand
        {
            get
            {
                if (this.newUserCommand == null)
                {
                    this.newUserCommand = new DelegateCommand(p => this.CreateNewUser());
                }

                return this.newUserCommand;
            }
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
        }

        /// <summary>
        /// Logs in a user and opens the appropriate user interface.
        /// </summary>
        /// <param name="args">The arguments of the command.</param>
        private void LoginUser(object args)
        {
            // Get the username and password from the user
            string password = (args as PasswordBox).Password;
            string username = this.Username;

            // Get a list of possible users that could be trying to log in
            List<IUser> users = new List<IUser>();
            users.AddRange(this.repository.GetAdministrators());
            users.AddRange(this.repository.GetMembers());

            // Find the user trying to log in based on the username
            this.user = users.FirstOrDefault(u => u.Username == username);

            if (this.user != null)
            {
                // Get the hashed version of the entered password
                string hashedPassword = Security.CreateHashedPassword(password + this.user.Guid);

                // If the user's password matches the hashed entered password
                if (this.user.Password == hashedPassword)
                {
                    // Log in the user as a member or as an administrator, based on the user's role
                    if (this.user is Member)
                    {
                        MemberMainView memberMainView = new MemberMainView();
                        memberMainView.DataContext = new MemberMainWindowViewModel(this.user as Member, this.repository, this.controlViewModel, this);
                        this.controlViewModel.ControlWindowContent = memberMainView;
                    }
                    else if (this.user is Administrator)
                    {
                        MainWindowView mainWindowView = new MainWindowView();
                        mainWindowView.DataContext = new MainWindowViewModel(this.repository, this.controlViewModel, this);
                        this.controlViewModel.ControlWindowContent = mainWindowView;
                    }
                }
                else
                {
                    MessageBox.Show("The password is incorrect.");
                }
            }
            else
            {
                MessageBox.Show("The username is incorrect.");
            }
        }

        /// <summary>
        /// Creates a new member.
        /// </summary>
        private void CreateNewUser()
        {
            // Create view model for member
            MemberViewModel viewModel = new MemberViewModel(new Member { Guid = Guid.NewGuid() }, this.repository);

            this.ShowMember(viewModel, "Create");
        }

        /// <summary>
        /// Show the member.
        /// </summary>
        /// <param name="viewModel">The parameter member view model.</param>
        /// <param name="windowFunction">The function that the window will serve (creating or editing).</param>
        private void ShowMember(MemberViewModel viewModel, string windowFunction)
        {
            // Create and configure window.
            WorkspaceWindow window = new WorkspaceWindow();
            window.Title = windowFunction + " " + viewModel.DisplayName;

            // Create and configure view.
            MemberView view = new MemberView();
            view.DataContext = viewModel;
            viewModel.CloseAction = b => window.DialogResult = b;

            window.Content = view;

            window.ShowDialog();
        }
    }
}