using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using LibraryEngine;

namespace LibraryDataAccess
{
    /// <summary>
    /// The class that represents the database context of the library system.
    /// </summary>
    public class LibraryContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public LibraryContext()
            : base("LibraryContext")
        {
            Database.Initialize(true);
        }

        /// <summary>
        /// Gets or sets the context's list of administrators.
        /// </summary>
        public DbSet<Administrator> Administrators { get; set; }

        /// <summary>
        /// Gets or sets the context's list of authors.
        /// </summary>
        public DbSet<Author> Authors { get; set; }

        /// <summary>
        /// Gets or sets the context's list of books.
        /// </summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>
        /// Gets or sets the context's list of copies of books.
        /// </summary>
        public DbSet<BookCopy> Copies { get; set; }

        /// <summary>
        /// Gets or sets the context's list of formats.
        /// </summary>
        public DbSet<Format> Formats { get; set; }

        /// <summary>
        /// Gets or sets the context's list of genres.
        /// </summary>
        public DbSet<Genre> Genres { get; set; }

        /// <summary>
        /// Gets or sets the context's list of members.
        /// </summary>
        public DbSet<Member> Members { get; set; }

        /// <summary>
        /// Gets or sets the context's list of publishers.
        /// </summary>
        public DbSet<Publisher> Publishers { get; set; }

        /// <summary>
        /// Gets or sets the context's list of transactions.
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; }

        /// <summary>
        /// Gets or sets the context's list of transaction details.
        /// </summary>
        public DbSet<TransactionDetail> TransactionDetails { get; set; }

        /// <summary>
        /// Removes the plurals from table names.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}