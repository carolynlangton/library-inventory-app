using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a view model of many members.
    /// </summary>
    public class MultiMemberViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The multi-member view model's database.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The collection of member view source.
        /// </summary>
        private CollectionViewSource memberViewSource;

        /// <summary>
        /// The sort column name.
        /// </summary>
        private string sortColumnName;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private ListSortDirection sortDirection;

        /// <summary>
        /// The multi view model's collection of members.
        /// </summary>
        private ObservableCollection<MemberViewModel> displayedMembers;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's database.</param>
        public MultiMemberViewModel(Repository repository)
            : base("View all members")
        {
            this.repository = repository;

            this.DisplayedMembers = new ObservableCollection<MemberViewModel>();
            this.memberViewSource = new CollectionViewSource();
            this.memberViewSource.Source = this.DisplayedMembers;

            this.SortCommand = new DelegateCommand(this.Sort);

            this.CreateAllMembers();

            this.repository.MemberAdded += this.OnMemberAdded;
            this.repository.MemberArchived += this.OnMemberArchived;

            this.Pager = new PagingViewModel(this.AllMembers.Count);
            this.Pager.CurrentPageChanged += this.OnPageChange;

            this.RebuildPageData();
        }

        /// <summary>
        /// Gets the list collection of sorted members.
        /// </summary>
        public ListCollectionView SortedMembers
        {
            get
            {
                return this.memberViewSource.View as ListCollectionView;
            }
        }

        /// <summary>
        /// Gets the sort command.
        /// </summary>
        public ICommand SortCommand { get; private set; }

        /// <summary>
        /// Gets the collection of members.
        /// </summary>
        public ObservableCollection<MemberViewModel> DisplayedMembers
        {
            get
            {
                return this.displayedMembers;
            }
            private set
            {
                this.displayedMembers = value;

                // Create and link to collection view source.
                this.memberViewSource = new CollectionViewSource();
                this.memberViewSource.Source = this.DisplayedMembers;
            }
        }

        /// <summary>
        /// Gets the view model's list of members.
        /// </summary>
        public ObservableCollection<MemberViewModel> AllMembers { get; private set; }

        /// <summary>
        /// Gets the view model's pager.
        /// </summary>
        public PagingViewModel Pager { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a view model is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                bool result = false;
                int selectedCount = 0;

                foreach (MemberViewModel avm in this.AllMembers)
                {
                    if (avm.IsSelected != false)
                    {
                        selectedCount += 1;

                        result = true;

                        if (selectedCount != 1)
                        {
                            result = false;

                            break;
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether an member is selected.
        /// </summary>
        public bool IsSelectedToRomove
        {
            get
            {
                bool result = false;

                foreach (MemberViewModel avm in this.AllMembers)
                {
                    if (avm.IsSelected != false)
                    {
                        result = true;

                        break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the number of members selected in the multi-member view.
        /// </summary>
        public int NumberOfMembersSelected { get; set; }

        /// <summary>
        /// Creates a new member.
        /// </summary>
        public void CreateNewMember()
        {
            // Create view model for member
            MemberViewModel viewModel = new MemberViewModel(new Member { Guid = Guid.NewGuid() }, this.repository);

            this.ShowMember(viewModel, "Create");
        }

        /// <summary>
        /// Sort the displayed data ascending, or descending.
        /// </summary>
        /// <param name="parameter">The object to sort.</param>
        public void Sort(object parameter)
        {
            string columnName = parameter as string;

            // If clicking on the header of the currently-sorted column...
            if (this.sortColumnName == columnName)
            {
                // Toggle sorting direction.
                this.sortDirection = this.sortDirection == ListSortDirection.Ascending ?
                    ListSortDirection.Descending : ListSortDirection.Ascending;
            }
            else
            {
                // Set the sored column.
                this.sortColumnName = columnName;

                // Set sort direction to ascending.
                this.sortDirection = ListSortDirection.Ascending;
            }

            // Clear and reset the sort order of the list view.
            this.memberViewSource.SortDescriptions.Clear();
            this.memberViewSource.SortDescriptions.Add(new SortDescription(this.sortColumnName, this.sortDirection));
        }

        /// <summary>
        /// Rebuild the data page.
        /// </summary>
        public void RebuildPageData()
        {
            this.DisplayedMembers.Clear();

            int startingIndex = this.Pager.PageSize * (this.Pager.CurrentPage - 1);

            List<MemberViewModel> displayedMembers = this.AllMembers.Skip(startingIndex).Take(this.Pager.PageSize).ToList();

            this.Pager.ItemCount = this.AllMembers.Count;

            foreach (MemberViewModel vm in displayedMembers)
            {
                this.DisplayedMembers.Add(vm);
            }
        }

        /// <summary>
        /// Creates the view model's commands.
        /// </summary>
        protected override void CreateCommands()
        {
            this.Commands.Add(new CommandViewModel("New...", new DelegateCommand(p => this.CreateNewMember())));
            this.Commands.Add(new CommandViewModel("Edit...", new DelegateCommand(p => this.EditMember(), m => this.IsSelected)));
            this.Commands.Add(new CommandViewModel("Remove", new DelegateCommand(p => this.ArchiveMember(), m => this.IsSelectedToRomove)));
        }

        /// <summary>
        /// Rebuild the page to show updates in data.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event that handles the page change.</param>
        private void OnPageChange(object sender, CurrentPageChangeEventArgs e)
        {
            this.RebuildPageData();
        }

        /// <summary>
        /// Creates a list of members as view models.
        /// </summary>
        private void CreateAllMembers()
        {
            // Get a list of view models for each member in the database that is not archived.
            IEnumerable<MemberViewModel> members =
                from member in this.repository.GetMembers()
                where !member.IsArchived
                select new MemberViewModel(member, this.repository);

            // Create observable collection from list
            this.AllMembers = new ObservableCollection<MemberViewModel>(members);
        }

        /// <summary>
        /// Deletes a member.
        /// </summary>
        private void ArchiveMember()
        {
            MemberViewModel viewModel = this.AllMembers.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                if (MessageBox.Show("Are you sure you want to delete the selected member?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.repository.ArchiveMember(viewModel.Member);
                }
            }
            else
            {
                MessageBox.Show("Please select a single member.");
            }

            this.RebuildPageData();
        }

        /// <summary>
        /// Edit the member.
        /// </summary>
        private void EditMember()
        {
            MemberViewModel viewModel = this.AllMembers.FirstOrDefault(vm => vm.IsSelected);

            if (viewModel != null)
            {
                this.ShowMember(viewModel, "Edit");
            }
            else
            {
                MessageBox.Show("Please first select a member.");
            }
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

        /// <summary>
        /// Handles the event of a member being added.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnMemberAdded(object sender, MemberEventArgs e)
        {
            MemberViewModel viewModel = new MemberViewModel(e.Member, this.repository);
            this.AllMembers.Add(viewModel);
            this.RebuildPageData();
        }

        /// <summary>
        /// Remove the selected view model.
        /// </summary>
        /// <param name="sender">The object that initiated the event.</param>
        /// <param name="e">The author event argument.</param>
        private void OnMemberArchived(object sender, MemberEventArgs e)
        {
            MemberViewModel viewModel = this.AllMembers.FirstOrDefault(vm => vm.Member == e.Member);

            if (viewModel != null)
            {
                this.AllMembers.Remove(viewModel);
                this.RebuildPageData();
            }
        }
    }
}