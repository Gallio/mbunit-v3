using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Arguments for the event raised when a paragraph is changed.
    /// </summary>
    public class ParagraphChangedEventArgs : EventArgs
    {
        private readonly int paragraphIndex;

        /// <summary>
        /// Creates event arguments for paragraph changed.
        /// </summary>
        /// <param name="paragraphIndex">The index of the paragraph that was changed.</param>
        public ParagraphChangedEventArgs(int paragraphIndex)
        {
            this.paragraphIndex = paragraphIndex;
        }

        /// <summary>
        /// Gets the index of the paragraph that was changed.
        /// </summary>
        public int ParagraphIndex
        {
            get { return paragraphIndex; }
        }
    }
}
