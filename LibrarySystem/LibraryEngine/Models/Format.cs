using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LibraryEngine
{
    /// <summary>
    /// The class which describes a book's format.
    /// </summary>
    public class Format : IDataErrorInfo
    {
        /// <summary>
        /// The properties to validate.
        /// </summary>
        private static readonly string[] propertiesToValidate =
        {
            "Type"
        };

        /// <summary>
        /// Gets or sets the format Id.
        /// </summary>
        public int FormatId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the format is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the type of format.
        /// </summary>
        [MaxLength(100, ErrorMessage = "The max length of a format type is 100 characters")]
        [Required(ErrorMessage = "Type is a required field")]
        public string Type { get; set; }

        /// <summary>
        /// Gets a value indicating whether the format's result is valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool result = true;

                foreach (string p in Format.propertiesToValidate)
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
        /// Gets the format's error.
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
        /// Gets a string representation of the format.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Type;
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
                case "Type":
                    result = this.ValidateType();
                    break;
                default:
                    throw new Exception("Unexpected property found to validate.");
            }

            return result;
        }

        /// <summary>
        /// Validates the format's type.
        /// </summary>
        /// <returns>The validation error.</returns>
        private string ValidateType()
        {
            string result = null;

            if (string.IsNullOrWhiteSpace(this.Type) || this.Type.Length > 100)
            {
                result = "Please enter a type of less than 100 characters";
            }

            return result;
        }
    }
}