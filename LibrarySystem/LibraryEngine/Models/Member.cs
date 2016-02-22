using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace LibraryEngine
{
    /// <summary>
    /// The class that represents a member.
    /// </summary>
    public class Member : IUser, IDataErrorInfo
    {
        /// <summary>
        /// The property names to validate.
        /// </summary>
        private static readonly string[] propertiesToValidate =
        {
            "FirstName",
            "LastName",
            "Username",
            "Password"
        };

        /// <summary>
        /// Gets or sets the member ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the member is archived.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the member's first name.
        /// </summary>
        [MaxLength(100, ErrorMessage = "The max length of a first name is 100 characters")]
        [Required(ErrorMessage = "First name is a required field")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the member's last name.
        /// </summary>
        [MaxLength(100, ErrorMessage = "The max length of a first name is 100 characters")]
        [Required(ErrorMessage = "Last name is a required field")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the member's username.
        /// </summary>
        [Required(ErrorMessage = "Username is a required field")]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the member's password.
        /// </summary>
        [Required(ErrorMessage = "Password is a required field")]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the member's unique ID.
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the member's list of transactions.
        /// </summary>
        public virtual ICollection<Transaction> Transactions { get; set; }

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

                foreach (string p in Member.propertiesToValidate)
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
        /// Gets a value indicating whether or not the member has overdue books.
        /// </summary>
        public bool HasOverdueBooks
        {
            get
            {
                bool result = false;

                List<Transaction> transactions = this.Transactions.ToList();

                List<TransactionDetail> details = new List<TransactionDetail>();

                foreach (Transaction t in transactions)
                {
                    details.AddRange(t.TransactionDetails);
                }

                result = details.Any(d => d.DueDate < DateTime.Today && d.CheckInDate == null);

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
        /// Gets a string representation of the member.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.FirstName + " " + this.LastName;
        }

        /// <summary>
        /// Data validation for member input properties.
        /// </summary>
        /// <param name="propertyName">The name of the property to test against.</param>
        /// <returns>The user's help error.</returns>
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
                case "Username":
                    result = this.ValidateUsername();
                    break;
                case "Password":
                    result = this.ValidatePassword();
                    break;
                default:
                    throw new Exception("Unexpected property was found to validate.");
            }

            return result;
        }

        /// <summary>
        /// Data validation for member's first name.
        /// </summary>
        /// <returns>The user's help error.</returns>
        private string ValidateFirstName()
        {
            string result = null;

            if (string.IsNullOrEmpty(this.FirstName))
            {
                result = "Please enter a first name.";
            }
            else if (!Regex.IsMatch(this.FirstName, @"^[\p{L}\p{M}' \.\-]+$"))
            {
                result = "Please enter a valid first name.";
            }

            return result;
        }

        /// <summary>
        /// Data validation for member's last name.
        /// </summary>
        /// <returns>The user's help error.</returns>
        private string ValidateLastName()
        {
            string result = null;

            if (string.IsNullOrEmpty(this.LastName))
            {
                result = "Please enter a last name.";
            }
            else if (!Regex.IsMatch(this.LastName, @"^[\p{L}\p{M}' \.\-]+$"))
            {
                result = "Please enter a valid last name.";
            }

            return result;
        }

        /// <summary>
        /// Validates the member's username.
        /// </summary>
        /// <returns>The validation error.</returns>
        private string ValidateUsername()
        {
            string result = null;

            if (string.IsNullOrEmpty(this.LastName))
            {
                result = "Please enter a username.";
            }

            return result;
        }

        /// <summary>
        /// Validates the member's password.
        /// </summary>
        /// <returns>The validation error.</returns>
        private string ValidatePassword()
        {
            string result = null;

            if (string.IsNullOrEmpty(this.LastName))
            {
                result = "Please enter a password.";
            }

            return result;
        }
    }
}