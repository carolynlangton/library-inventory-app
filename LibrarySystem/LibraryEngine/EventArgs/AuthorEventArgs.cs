namespace LibraryEngine
{
    /// <summary>
    /// The class which describes the event arguments of an author.
    /// </summary>
    public class AuthorEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="author">The event arguments' copy of a book.</param>
        public AuthorEventArgs(Author author)
        {
            this.Author = author;
        }

        /// <summary>
        /// Gets the event arguments' of an author.
        /// </summary>
        public Author Author { get; private set; }
    }
}