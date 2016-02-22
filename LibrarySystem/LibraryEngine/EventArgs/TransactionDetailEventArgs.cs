namespace LibraryEngine
{
    /// <summary>
    /// The class that represents the event arguments of a transaction.
    /// </summary>
    public class TransactionDetailEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="t">The event arguments' transaction detail.</param>
        public TransactionDetailEventArgs(TransactionDetail t)
        {
            this.TransactionDetail = t;
        }

        /// <summary>
        /// Gets the event arguments' transaction.
        /// </summary>
        public TransactionDetail TransactionDetail { get; private set; }
    }
}