using System.Collections.Generic;
using System.Collections.ObjectModel;
using LibraryDataAccess;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a report view model.
    /// </summary>
    public class ReportViewModel : WorkspaceViewModel
    {
        /// <summary>
        /// The report view model's repository.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// The report view model's over due books list.
        /// </summary>
        private ObservableCollection<object> reportCollection;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's repository.</param>
        /// <param name="collection">The collection to show as a report.</param>
        /// <param name="reportType">The type of the report to show.</param>
        public ReportViewModel(Repository repository, IEnumerable<object> collection, string reportType)
            : base(reportType)
        {
            this.repository = repository;

            this.reportCollection = new ObservableCollection<object>(collection);
        }

        /// <summary>
        /// Gets or sets the over due list.
        /// </summary>
        public ObservableCollection<object> ReportCollection
        {
            get
            {
                return this.reportCollection;
            }
            set
            {
                this.reportCollection = value;
                this.OnPropertyChanged("MyOverDueBooks");
            }
        }

        /// <summary>
        /// The view model's create commands.
        /// </summary>
        protected override void CreateCommands()
        {
        }
    }
}