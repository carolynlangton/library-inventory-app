using System;

namespace LibraryEngine
{
    /// <summary>
    /// The class that represents an administrator of the library system.
    /// </summary>
    public class Administrator : IUser
    {
        /// <summary>
        /// Gets or sets the administrator's ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the administrator's username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the administrator's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the administrator's unique ID.
        /// </summary>
        public Guid Guid { get; set; }
    }
}