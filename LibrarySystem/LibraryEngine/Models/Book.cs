using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryEngine
{
    /// <summary>
    /// The class that represents a book.
    /// </summary>
    public class Book : IDataErrorInfo
    {
        /// <summary>
        /// The properties to validate.
        /// </summary>
        private static readonly string[] propertiesToValidate =
        {
            "Title", "Isbn"
        };

        /// <summary>
        /// Gets or sets the book's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the book's title.
        /// </summary>
        [Required(ErrorMessage = "Title is a required field")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the book's ISBN.
        /// </summary>
        [Required(ErrorMessage = "ISBN is a required field")]
        [MaxLength(13, ErrorMessage = "The max length of an ISBN is 13 characters")]
        public string Isbn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the book is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the book's author's ID.
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the book's genre's ID.
        /// </summary>
        public int GenreId { get; set; }

        /// <summary>
        /// Gets or sets the book's author.
        /// </summary>
        public virtual Author Author { get; set; }

        /// <summary>
        /// Gets or sets the book's collection of copies.
        /// </summary>
        public virtual ICollection<BookCopy> Copies { get; set; }

        /// <summary>
        /// Gets or sets the book's genre.
        /// </summary>
        public virtual Genre Genre { get; set; }

        /// <summary>
        /// Gets a value indicating whether the book's result is valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool result = true;

                foreach (string p in Book.propertiesToValidate)
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
        /// Gets the validation error of the property name.
        /// </summary>
        /// <param name="propertyName">The property name to get.</param>
        /// <returns>The validation error of the property name.</returns>
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
                case "Title":
                    result = this.ValidateTitle();
                    break;
                case "Isbn":
                    result = this.ValidateIsbn();
                    break;
                default:
                    throw new Exception("Unexpected property found to validate.");
            }

            return result;
        }

        /// <summary>
        /// Data validation for the book's i s b n.
        /// </summary>
        /// <returns>The validation error for i s b n.</returns>
        private string ValidateIsbn()
        {
            string result = null;

            if (string.IsNullOrWhiteSpace(this.Isbn) || this.Isbn.Length != 13)
            {
                result = "Please enter a valid 13-digit ISBN.\nAdd prefix 978 to 10-digit ISBNs";
            }

            return result;
        }

        /// <summary>
        /// Data validation for the book's title.
        /// </summary>
        /// <returns>The validation error for title.</returns>
        private string ValidateTitle()
        {
            string result = null;

            if (string.IsNullOrWhiteSpace(this.Title))
            {
                result = "Please enter a title";
            }

            return result;
        }
    }
}