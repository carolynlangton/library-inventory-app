using System;
using System.Security.Cryptography;
using System.Text;

namespace LibraryDataAccess
{
    /// <summary>
    /// A helper class for storing passwords securely.
    /// </summary>
    public static class Security
    {
        /// <summary>
        /// Creates a hashed version of a string value.
        /// </summary>
        /// <param name="value">The value to hash.</param>
        /// <returns>The hashed value.</returns>
        public static string CreateHashedPassword(string value)
        {
            HashAlgorithm hash = new SHA256CryptoServiceProvider();
            var valueAsBytes = Encoding.ASCII.GetBytes(value);
            var hashedValue = Convert.ToBase64String(hash.ComputeHash(valueAsBytes));

            return hashedValue;
        }
    }
}