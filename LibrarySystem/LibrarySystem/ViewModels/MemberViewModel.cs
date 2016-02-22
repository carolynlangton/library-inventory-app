using System.ComponentModel;
using System.Windows;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of a member.
    /// </summary>
    public class MemberViewModel : WorkspaceViewModel, IDataErrorInfo
    {
        /// <summary>
        /// The view model's member.
        /// </summary>
        private Member member;

        /// <summary>
        /// The view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The view model's is selected boolean.
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="member">The passed in member.</param>
        /// <param name="repository">The passed in database.</param>
        public MemberViewModel(Member member, Repository repository)
            : base("Member")
        {
            this.member = member;
            this.repository = repository;
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

                this.OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the member is archived.
        /// </summary>
        public bool IsArchived
        {
            get
            {
                return this.member.IsArchived;
            }
            set
            {
                this.member.IsArchived = value;
                this.OnPropertyChanged("IsArchived");
            }
        }

        /// <summary>
        /// Gets or sets the member's first name.
        /// </summary>
        public string FirstName
        {
            get
            {
                return this.member.FirstName;
            }
            set
            {
                this.member.FirstName = value;
                this.OnPropertyChanged("FirstName");
            }
        }

        /// <summary>
        /// Gets or sets the member's ID.
        /// </summary>
        public int Id
        {
            get
            {
                return this.member.Id;
            }
            set
            {
                this.member.Id = value;
                this.OnPropertyChanged("Id");
            }
        }

        /// <summary>
        /// Gets or sets the member's last name.
        /// </summary>
        public string LastName
        {
            get
            {
                return this.member.LastName;
            }
            set
            {
                this.member.LastName = value;
                this.OnPropertyChanged("LastName");
            }
        }

        /// <summary>
        /// Gets or sets the member's username.
        /// </summary>
        public string Username
        {
            get
            {
                return this.member.Username;
            }
            set
            {
                this.member.Username = value;
                this.OnPropertyChanged("Username");
            }
        }

        /// <summary>
        /// Gets or sets the member's password.
        /// </summary>
        public string Password
        {
            get
            {
                return this.member.Password;
            }
            set
            {
                this.member.Password = value;
                this.OnPropertyChanged("Password");
            }
        }

        /// <summary>
        /// Gets the selected member.
        /// </summary>
        public Member Member
        {
            get
            {
                return this.member;
            }
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get
            {
                return this.Member.Error;
            }
        }

        /// <summary>
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The name of the property whose error message to get.</param>
        /// <returns> The error message for the property.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.Member[propertyName];
            }
        }

        /// <summary>
        /// Saves the view model's member to the database.
        /// </summary>
        /// <returns>True or false result.</returns>
        public bool Save()
        {
            bool result = true;

            if (this.Member.IsValid)
            {
                this.repository.AddMember(this.member);

                // Push changes.
                this.repository.SaveToDatabase();
            }
            else
            {
                result = false;

                MessageBox.Show("One or more fields are invalid.  Member cannot be saved.");
            }

            return result;
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("OK", new DelegateCommand(p => this.AcceptChanges())));
            this.Commands.Add(new CommandViewModel("Cancel", new DelegateCommand(p => this.CancelChanges())));
        }

        /// <summary>
        /// Accept the changes made and close.
        /// </summary>
        private void AcceptChanges()
        {
            if (this.Save())
            {
                if (this.CloseAction != null)
                {
                    this.CloseAction(true);
                }
            }
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