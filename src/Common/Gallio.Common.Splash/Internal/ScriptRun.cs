using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Splash.Native;

namespace Gallio.Common.Splash.Internal
{
    /// <summary>
    /// Internal structure with script information necessary for layout and display of a run.
    /// A single run may be subdivided into multiple script runs.
    /// </summary>
    /// <remarks>
    /// Since a script paragraph can contain many script runs and each one uses a variable amount
    /// of storage for buffers of various kinds, this structure contains indices into shared
    /// buffers managed by the containing script paragraph.  This reduces the number of required
    /// memory allocations per paragraph.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ScriptRun
    {
        private Run run;

        /// <summary>
        /// The script analysis for the run produced during itemization.
        /// </summary>
        public SCRIPT_ANALYSIS ScriptAnalysis;

        /// <summary>
        /// The index of the first character of the run relative to the first character of its containing paragraph.
        /// </summary>
        public int CharIndexInParagraph;

        /// <summary>
        /// The index of the first glyph of the run relative to the first glyph of its containing paragraph.
        /// </summary>
        public int GlyphIndexInParagraph;

        /// <summary>
        /// The number of glyphs in the run.
        /// </summary>
        public int GlyphCount;

        /// <summary>
        /// The height of the run excluding its margins.
        /// </summary>
        public int Height;

        /// <summary>
        /// The descent height of the run (below text baseline).
        /// </summary>
        public int Descent;

        /// <summary>
        /// The top margin of the run.
        /// </summary>
        public int TopMargin;

        /// <summary>
        /// The bottom margin of the run.
        /// </summary>
        public int BottomMargin;

        /// <summary>
        /// The advance width spacing of the run.
        /// </summary>
        public ABC ABC;

        public void Initialize(Run* run, SCRIPT_ANALYSIS* scriptAnalysis, int charIndexInParagraph, int charCount)
        {
            this.run = *run;
            if (this.run.RunKind == RunKind.Text)
                this.run.CharCount = charCount;

            CharIndexInParagraph = charIndexInParagraph;
            ScriptAnalysis = *scriptAnalysis;
        }

        public void InitializeTruncatedCopy(ScriptRun* scriptRun, int truncatedLeadingCharsCount, int truncatedTrailingCharsCount)
        {
            this.run = scriptRun->run;
            if (this.run.RunKind == RunKind.Text)
                this.run.CharCount = scriptRun->CharCount - truncatedLeadingCharsCount - truncatedTrailingCharsCount;

            ScriptAnalysis = scriptRun->ScriptAnalysis;
            CharIndexInParagraph = scriptRun->CharIndexInParagraph + truncatedLeadingCharsCount;
        }

        public char* Chars(ScriptParagraph* scriptParagraph, char* charZero)
        {
            return scriptParagraph->Chars(charZero) + CharIndexInParagraph;
        }

        public int CharCount
        {
            get { return run.CharCount; }
        }

        public int EndCharIndexInParagraph
        {
            get { return CharIndexInParagraph + CharCount; }
        }

        public int StyleIndex
        {
            get { return run.StyleIndex; }
        }

        public int ObjectIndex
        {
            get { return run.ObjectIndex; }
        }

        public RunKind RunKind
        {
            get { return run.RunKind; }
        }

        public bool RequiresTabExpansion
        {
            get { return run.RequiresTabExpansion; }
        }

        /// <summary>
        /// The ascent height of the run (above text baseline).
        /// </summary>
        public int Ascent
        {
            get { return Height - Descent; }
        }

        public ushort* CharLogicalClusters(ScriptParagraph* scriptParagraph)
        {
            return scriptParagraph->CharLogicalClusters + CharIndexInParagraph;
        }

        public SCRIPT_LOGATTR* CharLogicalAttributes(ScriptParagraph* scriptParagraph)
        {
            return scriptParagraph->CharLogicalAttributes + CharIndexInParagraph;
        }

        public ushort* Glyphs(ScriptParagraph* scriptParagraph)
        {
            return scriptParagraph->Glyphs + GlyphIndexInParagraph;
        }

        public SCRIPT_VISATTR* GlyphVisualAttributes(ScriptParagraph* scriptParagraph)
        {
            return scriptParagraph->GlyphVisualAttributes + GlyphIndexInParagraph;
        }

        public int* GlyphAdvanceWidths(ScriptParagraph* scriptParagraph)
        {
            return scriptParagraph->GlyphAdvanceWidths + GlyphIndexInParagraph;
        }

        public GOFFSET* GlyphOffsets(ScriptParagraph* scriptParagraph)
        {
            return scriptParagraph->GlyphOffsets + GlyphIndexInParagraph;
        }
    }
}
