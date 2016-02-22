namespace LibraryEngine
{
    /// <summary>
    /// The class that represents the event arguments of a member.
    /// </summary>
    public class MemberEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="member">The event arguments' member.</param>
        public MemberEventArgs(Member member)
        {
            this.Member = member;
        }

        /// <summary>
        /// Gets or sets the member.
        /// </summary>
        public Member Member { get; set; }
    }
}