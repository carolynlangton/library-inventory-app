using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LibraryEngine
{
    /// <summary>
    /// The class that represents a publisher.
    /// </summary>
    public class Publisher : IDataErrorInfo
    {
        /// <summary>
        /// The property names to validate.
        /// </summary>
        private static readonly string[] propertiesToValidate =
        {
            "Name",
            "Location"
        };

        /// <summary>
        /// Gets or sets the publisher's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the publisher's is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the publisher's name.
        /// </summary>
        [MaxLength(200, ErrorMessage = "The max length of a publisher name is 200 characters")]
        [Required(ErrorMessage = "Name is a required field")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the publisher's location.
        /// </summary>
        [MaxLength(200, ErrorMessage = "The max length of a publisher location is 200 characters")]
        public string Location { get; set; }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object.
        /// </summary>
        public string Error
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether input data is valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool result = true;

                foreach (string p in Publisher.propertiesToValidate)
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
        /// Gets the error message for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The name of the property whose error message to get.</param>
        /// <returns>The error message for the property.</returns>
        public string this[string propertyName]
        {
            get
            {
                return this.GetValidationError(propertyName);
            }
        }

        /// <summary>
        /// Gets a string representation of a publisher.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Data validation for publisher properties.
        /// </summary>
        /// <param name="propertyName">The name of the property to test against.</param>
        /// <returns>The user's help error.</returns>
        private string GetValidationError(string propertyName)
        {
            string result = null;

            switch (propertyName)
            {
                case "Name":
                    result = this.ValidateName();
                    break;
                case "Location":
                    result = this.ValidateLocation();
                    break;
                default:
                    throw new Exception("Unexpected property was found to validate.");
            }

            return result;
        }

        /// <summary>
        /// Data validation for publisher's name.
        /// </summary>
        /// <returns>The user's help error.</returns>
        private string ValidateName()
        {
            string result = null;

            if (string.IsNullOrEmpty(this.Name))
            {
                result = "Please enter a publisher name.";
            }
            else if (!Regex.IsMatch(this.Name, @"^[\p{L}\p{M}' \.\-]+$"))
            {
                result = "Please enter a valid name.";
            }

            return result;
        }

        /// <summary>
        /// Data validation for publisher's location.
        /// </summary>
        /// <returns>The user's help error.</returns>
        private string ValidateLocation()
        {
            string result = null;

            if (string.IsNullOrEmpty(this.Location))
            {
                result = "Please enter a location.";
            }
            else if (!Regex.IsMatch(this.Location, @"^[\p{L}\p{M}' \.\-]+$"))
            {
                result = "Please enter a valid location.";
            }

            return result;
        }
    }
}