using System;

namespace LibraryEngine
{
    /// <summary>
    /// The interface that defines a contract for all users of the library system.
    /// </summary>
    public interface IUser
    {
        /// <summary>
        /// Gets the user's username.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// Gets the user's password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Gets the user's unique ID.
        /// </summary>
        Guid Guid { get; }
    }
}