using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryEngine
{
    /// <summary>
    /// The class that represents a current page change event argument.
    /// </summary>
    public class CurrentPageChangeEventArgs
    {
        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="itemCount">The item count.</param>
        public CurrentPageChangeEventArgs(int startIndex, int itemCount)
        {
            this.StartIndex = startIndex;
            this.ItemCount = itemCount;
        }

        /// <summary>
        /// Gets the start index.
        /// </summary>
        public int StartIndex { get; private set; }

        /// <summary>
        /// Gets the item count.
        /// </summary>
        public int ItemCount { get; private set; }
    }
}