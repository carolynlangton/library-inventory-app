namespace LibraryEngine
{
    /// <summary>
    /// The class that represents the event arguments of a publisher.
    /// </summary>
    public class PublisherEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="publisher">The event arguments' publisher.</param>
        public PublisherEventArgs(Publisher publisher)
        {
            this.Publisher = publisher;
        }

        /// <summary>
        /// Gets or sets the publisher.
        /// </summary>
        public Publisher Publisher { get; set; }
    }
}