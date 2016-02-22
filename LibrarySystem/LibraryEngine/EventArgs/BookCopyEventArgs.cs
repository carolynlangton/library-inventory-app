namespace LibraryEngine
{
    /// <summary>
    /// The class that represents the event arguments of a copy of a book.
    /// </summary>
    public class BookCopyEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="copy">The event arguments' copy of a book.</param>
        public BookCopyEventArgs(BookCopy copy)
        {
            this.Copy = copy;
        }

        /// <summary>
        /// Gets the event arguments' copy of a book.
        /// </summary>
        public BookCopy Copy { get; private set; }
    }
}