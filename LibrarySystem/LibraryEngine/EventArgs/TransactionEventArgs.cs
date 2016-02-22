namespace LibraryEngine
{
    /// <summary>
    /// The class that represents the event arguments of a transaction.
    /// </summary>
    public class TransactionEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="t">The event arguments' transaction.</param>
        public TransactionEventArgs(Transaction t)
        {
            this.Transaction = t;
        }

        /// <summary>
        /// Gets the event arguments' transaction.
        /// </summary>
        public Transaction Transaction { get; private set; }
    }
}