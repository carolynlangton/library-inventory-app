using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryEngine
{
    /// <summary>
    /// The class which describes a book's Author.
    /// </summary>
    public class Author : IDataErrorInfo
    {
        /// <summary>
        /// The properties to validate.
        /// </summary>
        private static readonly string[] propertiesToValidate =
        {
            "FirstName", "LastName"
        };

        /// <summary>
        /// Gets or sets the author Id.
        /// </summary>
        public int AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the author's first name.
        /// </summary>
        [MaxLength(100, ErrorMessage = "The max length of a first name is 100 characters")]
        [Required(ErrorMessage = "First name is a required field")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the author is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the author's last name.
        /// </summary>
        [MaxLength(200, ErrorMessage = "The max length of a first name is 200 characters")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets a value indicating whether the author's result is valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool result = true;

                foreach (string p in Author.propertiesToValidate)
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
        /// Gets the author's error.
        /// </summary>
        public string Error
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the validation error associated with the specified property.
        /// </summary>
        /// <param name="propertyName">The property for which to get the error.</param>
        /// <returns>The validation error.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.GetValidationError(propertyName);
            }
        }

        /// <summary>
        /// Gets a string representation of an author.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.LastName + ", " + this.FirstName;
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
                case "FirstName":
                    result = this.ValidateFirstName();
                    break;
                case "LastName":
                    result = this.ValidateLastName();
                    break;
                default:
                    throw new Exception("Unexpected property found to validate.");
            }

            return result;
        }

        /// <summary>
        /// Validates the author's last name.
        /// </summary>
        /// <returns>The validation error.</returns>
        private string ValidateLastName()
        {
            string result = null;

            if (this.LastName != null && this.LastName.Length > 200)
            {
                result = "Last names cannot be more than 200 characters";
            }

            return result;
        }

        /// <summary>
        /// Validates the author's first name.
        /// </summary>
        /// <returns>The validation error.</returns>
        private string ValidateFirstName()
        {
            string result = null;

            if (string.IsNullOrWhiteSpace(this.FirstName) || this.FirstName.Length > 100)
            {
                result = "Please enter a first name of less than 100 characters";
            }

            return result;
        }
    }
}