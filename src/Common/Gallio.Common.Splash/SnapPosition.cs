using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Provides the result of mapping a screen position to a character index.
    /// </summary>
    public struct Snap
    {
        private readonly SnapKind kind;
        private readonly int charIndex;

        /// <summary>
        /// Initializes a snap result.
        /// </summary>
        /// <param name="kind">The snap kind.</param>
        /// <param name="charIndex">The character index of the snap, or -1 if no snap.</param>
        public Snap(SnapKind kind, int charIndex)
        {
            this.kind = kind;
            this.charIndex = charIndex;
        }

        /// <summary>
        /// Gets the snap kind.
        /// </summary>
        public SnapKind Kind
        {
            get { return kind; }
        }

        /// <summary>
        /// Gets the character index of the snap, or -1 if no snap.
        /// </summary>
        public int CharIndex
        {
            get { return charIndex; }
        }
    }
}
