namespace LibraryEngine
{
    /// <summary>
    /// The class which describes events related to the format of a book.
    /// </summary>
    public class FormatEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="format">The event arguments of the format of the book.</param>
        public FormatEventArgs(Format format)
        {
            this.Format = format;
        }

        /// <summary>
        /// Gets the event arguments of the book's format.
        /// </summary>
        public Format Format { get; private set; }
    }
}