namespace LibraryEngine
{
    /// <summary>
    /// The class which describes the event arguments of a genre of a book.
    /// </summary>
    public class GenreEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="genreName">The event arguments' genre of a book.</param>
        public GenreEventArgs(Genre genreName)
        {
            this.GenreName = genreName;
        }

        /// <summary>
        /// Gets the event arguments' genre of a book.
        /// </summary>
        public Genre GenreName { get; private set; }
    }
}