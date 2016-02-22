using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryEngine
{
    /// <summary>
    /// The class that represents a transaction.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Gets or sets the transaction's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the transaction's check out date.
        /// </summary>
        [Column(TypeName = "date")]
        [Required(ErrorMessage = "Check out date is a required field")]
        public DateTime CheckOutDate { get; set; }

        /// <summary>
        /// Gets or sets the transaction's member ID.
        /// </summary>
        public int MemberId { get; set; }

        /// <summary>
        /// Gets or sets the transaction's member.
        /// </summary>
        public virtual Member Member { get; set; }

        /// <summary>
        /// Gets or sets the transaction's collection of transaction details.
        /// </summary>
        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }
    }
}