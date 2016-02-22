using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryEngine
{
    /// <summary>
    /// The class which describes a genre of a book.
    /// </summary>
    public class Genre : IDataErrorInfo
    {
        /// <summary>
        /// The properties to validate.
        /// </summary>
        private static readonly string[] propertiesToValidate =
        {
            "Name"
        };

        /// <summary>
        /// Gets or sets the genre ID associated with the genre.
        /// </summary>
        public int GenreId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the genre is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets name of the genre.
        /// </summary>
        [MaxLength(150, ErrorMessage = "The max length of a genre name is 150 characters")]
        [Required(ErrorMessage = "Name is a required field")]
        public string Name { get; set; }

        /// <summary>
        /// Gets a value indicating whether the genre's result is valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool result = true;

                foreach (string p in Genre.propertiesToValidate)
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
        /// Gets the genre's error.
        /// </summary>
        public string Error
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the validation error associated with the specified property name.
        /// </summary>
        /// <param name="propertyName">The property name for which to get the error.</param>
        /// <returns>The validation error.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.GetValidationError(propertyName);
            }
        }

        /// <summary>
        /// Gets a string representation of the genre.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Name;
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
                case "Name":
                    result = this.ValidateName();
                    break;
                default:
                    throw new Exception("Unexpected property found to validate.");
            }

            return result;
        }

        /// <summary>
        /// Validates the genre's name.
        /// </summary>
        /// <returns>The validation error.</returns>
        private string ValidateName()
        {
            string result = null;

            if (string.IsNullOrWhiteSpace(this.Name) || this.Name.Length > 150)
            {
                result = "Please enter a name of less than 150 characters";
            }

            return result;
        }
    }
}