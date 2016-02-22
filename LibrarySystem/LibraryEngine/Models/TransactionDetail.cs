using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryEngine
{
    /// <summary>
    /// The class that represents the detail of a single transaction.
    /// </summary>
    public class TransactionDetail
    {
        /// <summary>
        /// Gets or sets the transaction detail's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the transaction detail's transaction's ID.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// Gets or sets the transaction detail's copy's ID.
        /// </summary>
        public int BookCopyId { get; set; }

        /// <summary>
        /// Gets or sets the transaction detail's check in date.
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? CheckInDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction detail's due date.
        /// </summary>
        [Column(TypeName = "date")]
        [Required(ErrorMessage = "Due date is a required field")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction detail's transaction.
        /// </summary>
        public virtual Transaction Transaction { get; set; }

        /// <summary>
        /// Gets or sets the transaction detail's copy.
        /// </summary>
        public virtual BookCopy Copy { get; set; }
    }
}