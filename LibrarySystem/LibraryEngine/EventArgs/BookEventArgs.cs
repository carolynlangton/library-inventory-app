namespace LibraryEngine
{
    /// <summary>
    /// The class that represents the event arguments of a book.
    /// </summary>
    public class BookEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="book">The event arguments' book.</param>
        public BookEventArgs(Book book)
        {
            this.Book = book;
        }

        /// <summary>
        /// Gets the event arguments' book.
        /// </summary>
        public Book Book { get; private set; }
    }
}