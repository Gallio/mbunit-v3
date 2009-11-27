using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Gallio.Common.Splash.Internal
{
    /// <summary>
    /// Internal structure that maps a visual line onto a particular range of runs
    /// within a paragraph and includes the character position of required line breaks.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct ScriptLine
    {
        /// <summary>
        /// The absolute vertical offset of the line in the document when rendered.
        /// </summary>
        public int Y;

        /// <summary>
        /// The absolute horizontal offset of the line in the document when rendered.
        /// </summary>
        public int X;

        /// <summary>
        /// The height of the line.
        /// </summary>
        public int Height;

        /// <summary>
        /// The descent height of the line (below text baseline).
        /// </summary>
        public int Descent;

        /// <summary>
        /// The ascent height of the line (above text baseline).
        /// </summary>
        public int Ascent
        {
            get { return Height - Descent; }
        }

        /// <summary>
        /// The index of the paragraph that appears on the line.
        /// </summary>
        public int ParagraphIndex;

        /// <summary>
        /// The index of the paragraph's first script run that appears on the line.
        /// </summary>
        public int ScriptRunIndex;

        /// <summary>
        /// The number of script runs that appear on the line.
        /// </summary>
        public int ScriptRunCount;

        /// <summary>
        /// The number of leading chars in the first script run to truncate due to word wrap.
        /// </summary>
        public int TruncatedLeadingCharsCount;

        /// <summary>
        /// The number of trailing chars in the last script run to truncate due to word wrap.
        /// </summary>
        public int TruncatedTrailingCharsCount;

        public void Initialize(int paragraphIndex, int scriptRunIndex, int y)
        {
            ParagraphIndex = paragraphIndex;
            ScriptRunIndex = scriptRunIndex;
            Y = y;
            X = 0;
            Height = 0;
            Descent = 0;
            ScriptRunCount = 0;
            TruncatedLeadingCharsCount = 0;
            TruncatedTrailingCharsCount = 0;
        }
    }
}
