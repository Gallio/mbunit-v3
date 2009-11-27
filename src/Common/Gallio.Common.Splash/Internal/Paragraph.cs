using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Internal
{
    /// <summary>
    /// Internal structure describing a paragraph in the document.
    /// Each paragraph consists of a number of characters subdivided into runs that have
    /// the same style or that represent embedded objects.
    /// </summary>
    /// <remarks>
    /// Packed into 16 bytes per Paragraph.  Keep it that way or make it smaller.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Paragraph
    {
        public int CharIndex;
        public int CharCount;
        public int RunIndex;
        public int RunCount;

        public void Initialize(int charIndex, int charCount, int runIndex, int runCount)
        {
            CharIndex = charIndex;
            CharCount = charCount;
            RunIndex = runIndex;
            RunCount = runCount;
        }
    }
}
