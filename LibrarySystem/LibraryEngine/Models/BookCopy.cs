using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryEngine
{
    /// <summary>
    /// The class that represents a copy of a book.
    /// </summary>
    public class BookCopy : IDataErrorInfo
    {
        /// <summary>
        /// The properties to validate.
        /// </summary>
        private static readonly string[] propertiesToValidate =
        {
            "NumberOfPages", "ShelfNumber", "CopyrightYear"
        };

        /// <summary>
        /// Gets or sets the copy's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the copy's book's ID.
        /// </summary>
        public int BookId { get; set; }

        /// <summary>
        /// Gets or sets the copy's number of pages.
        /// </summary>
        [Range(1, 3100, ErrorMessage = "Number of pages must be between 1 and 3100")]
        public int? NumberOfPages { get; set; }

        /// <summary>
        /// Gets or sets the copy's format's ID.
        /// </summary>
        public int FormatId { get; set; }

        /// <summary>
        /// Gets or sets the copy's publisher's ID.
        /// </summary>
        public int PublisherId { get; set; }

        /// <summary>
        /// Gets or sets the copy's shelf number (location).
        /// </summary>
        [MaxLength(4, ErrorMessage = "Max length of a shelf number is four characters.")]
        public string ShelfNumber { get; set; }

        /// <summary>
        /// Gets or sets the copy's copyright year.
        /// </summary>
        public int CopyrightYear { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the copy is available.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the copy is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the copy's book.
        /// </summary>
        public virtual Book Book { get; set; }

        /// <summary>
        /// Gets or sets the copy's format.
        /// </summary>
        public virtual Format Format { get; set; }

        /// <summary>
        /// Gets or sets the copy's publisher.
        /// </summary>
        public virtual Publisher Publisher { get; set; }

        /// <summary>
        /// Gets or sets the copy's collection of transaction details.
        /// </summary>
        public virtual ICollection<TransactionDetail> TransactionDetails { get; set; }

        /// <summary>
        /// Gets a value indicating whether book copy's property is valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool result = true;

                foreach (string p in BookCopy.propertiesToValidate)
                {
                    if (this.GetValidationError(p) != null)
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string Error
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the validation error for the property name.
        /// </summary>
        /// <param name="propertyName">The error for the property name.</param>
        /// <returns>The validation error for the property name.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.GetValidationError(propertyName);
            }
        }

        /// <summary>
        /// Gets the validation error.
        /// </summary>
        /// <param name="propertyName">The property name to validate.</param>
        /// <returns>The validation error.</returns>
        private string GetValidationError(string propertyName)
        {
            string result = null;

            switch (propertyName)
            {
                case "NumberOfPages":
                    result = this.ValidateNumberOfPages();
                    break;
                case "ShelfNumber":
                    result = this.ValidateShelfNumber();
                    break;
                case "CopyrightYear":
                    result = this.ValidateCopyrightYear();
                    break;
                default:
                    throw new Exception("Unexpected property found to validate.");
            }

            return result;
        }

        /// <summary>
        /// Data validation on book copy's copy right year.
        /// </summary>
        /// <returns>The validation error for book copy's copy right year.</returns>
        private string ValidateCopyrightYear()
        {
            string result = null;

            if (this.CopyrightYear > DateTime.Today.Year)
            {
                result = "The copyright year must be " + DateTime.Today.Year + " or earlier";
            }

            return result;
        }

        /// <summary>
        /// Data validation on book copy's shelf number.
        /// </summary>
        /// <returns>The validation error for the book copy's shelf number.</returns>
        private string ValidateShelfNumber()
        {
            string result = null;

            if (this.ShelfNumber != null)
            {
                if (this.ShelfNumber.Length > 4)
                {
                    result = "Shelf number cannot be more than 4 characters";
                }
            }

            return result;
        }

        /// <summary>
        /// Data validation on book copy's number of pages.
        /// </summary>
        /// <returns>The validation error on book copy's number of pages.</returns>
        private string ValidateNumberOfPages()
        {
            string result = null;

            if (this.NumberOfPages != null)
            {
                if (this.NumberOfPages < 1 || this.NumberOfPages > 3100)
                {
                    result = "The number of pages must be between 1 and 3100";
                }
            }

            return result;
        }
    }
}