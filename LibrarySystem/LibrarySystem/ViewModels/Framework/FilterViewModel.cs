using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryDataAccess;
using LibraryEngine;

namespace LibrarySystem
{
    /// <summary>
    /// The class that represents a filter view model.
    /// </summary>
    public class FilterViewModel : ViewModel
    {
        /// <summary>
        /// The view model's repository.
        /// </summary>
        private Repository repository;

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="repository">The view model's repository.</param>
        public FilterViewModel(Repository repository)
            : base(string.Empty)
        {
            this.repository = repository;
        }

        /// <summary>
        /// Gets the authors.
        /// </summary>
        public IEnumerable<Author> Authors
        {
            get
            {
                return this.repository.GetAuthors().ToList().OrderBy(a => a.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the apply filter command.
        /// </summary>
        public DelegateCommand ApplyFilterCommand { get; set; }
    }
}