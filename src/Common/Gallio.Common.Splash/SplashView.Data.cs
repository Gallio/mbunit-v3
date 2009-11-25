using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Splash.Native;

namespace Gallio.Common.Splash
{
    public unsafe partial class SplashView
    {
        private static void* AllocMemory(int bytes)
        {
            return Marshal.AllocCoTaskMem(bytes).ToPointer();
        }

        private static void* ReAllocMemory(void* memory, int bytes)
        {
            return Marshal.ReAllocCoTaskMem(new IntPtr(memory), bytes).ToPointer();
        }

        private static void FreeMemory(void* memory)
        {
            Marshal.FreeCoTaskMem(new IntPtr(memory));
        }

        /// <summary>
        /// Internal structure describing a paragraph in the document.
        /// Each paragraph consists of a number of characters subdivided into runs that have
        /// the same style or that represent embedded objects.
        /// </summary>
        /// <remarks>
        /// Packed into 16 bytes per Paragraph.  Keep it that way or make it smaller.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private struct Paragraph
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

        private enum RunKind
        {
            Text = 0,
            Object = 1
        }

        /// <summary>
        /// Internal structure describing a run of text or an embedded object in the document.
        /// </summary>
        /// <remarks>
        /// Packed into 4 bytes per Run.  Keep it that way.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private struct Run
        {
            private const int RunKindMask = 0x01;
            private const int RequiresTabExpansionMask = 0x02;

            private byte styleIndex;
            private byte bitFields;
            private ushort miscShort;

            public RunKind RunKind
            {
                get { return (RunKind)(bitFields & RunKindMask); }
            }

            public bool RequiresTabExpansion
            {
                get { return (bitFields & RequiresTabExpansionMask) != 0; }
            }

            public void SetRequiresTabExpansion()
            {
                bitFields |= RequiresTabExpansionMask;
            }

            public int StyleIndex
            {
                get { return styleIndex; }
            }

            public int CharCount
            {
                get
                {
                    switch (RunKind)
                    {
                        case RunKind.Text:
                            return miscShort;
                        case RunKind.Object:
                            return 1;
                        default:
                            throw new NotSupportedException();
                    }
                }
                set
                {
                    switch (RunKind)
                    {
                        case RunKind.Text:
                            miscShort = (ushort)value;
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            public int ObjectIndex
            {
                get
                {
                    switch (RunKind)
                    {
                        case RunKind.Object:
                            return miscShort;
                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            public void InitializeTextRun(int styleIndex)
            {
                this.styleIndex = (byte)styleIndex;
                bitFields = (byte)RunKind.Text;
                miscShort = 0;
            }

            public void InitializeObjectRun(int styleIndex, int objectIndex)
            {
                this.styleIndex = (byte)styleIndex;
                bitFields = (byte)RunKind.Object;
                miscShort = (ushort)objectIndex;
            }
        }

        /// <summary>
        /// Internal structure with script information necessary for layout and display of a paragraph.
        /// Includes script information for each run in the paragraph.
        /// </summary>
        /// <remarks>
        /// We pool allocated script paragraph structures and cache them to improve performance
        /// and reduce the number of memory allocations.
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private struct ScriptParagraph
        {
            public int CharIndex;
            public int CharCount;
            public int CharCapacity;
            public ushort* CharLogicalClusters;           // 1 element per character
            public SCRIPT_LOGATTR* CharLogicalAttributes; // 1 element per character

            public int ScriptRunCount;
            public int ScriptRunCapacity;
            public ScriptRun* ScriptRuns;                 // 1 element per script run
            public int* ScriptRunVisualToLogicalMap;      // 1 element per script run

            public int GlyphCount;
            public int GlyphCapacity;
            public ushort* Glyphs;                         // 1 element per glyph
            public SCRIPT_VISATTR* GlyphVisualAttributes;  // 1 element per glyph
            public int* GlyphAdvanceWidths;                // 1 element per glyph
            public GOFFSET* GlyphOffsets;                  // 1 element per glyph

            public char* Chars(char* charZero)
            {
                return charZero + CharIndex;
            }

            public void Initialize()
            {
                InitializeCharBuffers();
                InitializeScriptRunBuffers();
                InitializeGlyphBuffers();
            }

            public void Free()
            {
                FreeCharBuffers();
                FreeScriptRunBuffers();
                FreeGlyphBuffers();
            }

            public void EnsureCharCapacity(int capacity)
            {
                if (CharCapacity < capacity)
                {
                    FreeCharBuffers();
                    CharCapacity = NewCapacity(CharCapacity, InitialCapacityForCharsPerParagraph, capacity);
                    AllocCharBuffers();
                }
            }

            public void EnsureScriptRunCapacity(int capacity)
            {
                if (ScriptRunCapacity < capacity)
                {
                    FreeScriptRunBuffers();
                    ScriptRunCapacity = NewCapacity(ScriptRunCapacity, InitialCapacityForScriptRunsPerParagraph, capacity);
                    AllocScriptRunBuffers();
                }
            }

            public void EnsureGlyphCapacityAndPreserveContents(int capacity)
            {
                if (GlyphCapacity < capacity)
                {
                    int newGlyphCapacity = NewCapacity(GlyphCapacity, InitialCapacityForGlyphsPerParagraph, capacity);
                    ReAllocGlyphBuffers(newGlyphCapacity);
                    GlyphCapacity = newGlyphCapacity;
                }
            }

            private void InitializeCharBuffers()
            {
                CharCapacity = 0;
                CharLogicalClusters = null;
                CharLogicalAttributes = null;
            }

            private void AllocCharBuffers()
            {
                CharLogicalClusters = (ushort*)AllocMemory(CharCapacity * sizeof(ushort));
                CharLogicalAttributes = (SCRIPT_LOGATTR*)AllocMemory(CharCapacity * sizeof(SCRIPT_LOGATTR));
            }

            private void FreeCharBuffers()
            {
                CharCapacity = 0;

                if (CharLogicalClusters != null)
                {
                    FreeMemory(CharLogicalClusters);
                    CharLogicalClusters = null;
                }

                if (CharLogicalAttributes != null)
                {
                    FreeMemory(CharLogicalAttributes);
                    CharLogicalAttributes = null;
                }
            }

            private void InitializeScriptRunBuffers()
            {
                ScriptRunCapacity = 0;
                ScriptRuns = null;
                ScriptRunVisualToLogicalMap = null;
            }

            private void AllocScriptRunBuffers()
            {
                ScriptRuns = (ScriptRun*)AllocMemory(ScriptRunCapacity * sizeof(ScriptRun));
                ScriptRunVisualToLogicalMap = (int*)AllocMemory(ScriptRunCapacity * sizeof(int));
            }

            private void FreeScriptRunBuffers()
            {
                ScriptRunCapacity = 0;

                if (ScriptRuns != null)
                {
                    FreeMemory(ScriptRuns);
                    ScriptRuns = null;
                }

                if (ScriptRunVisualToLogicalMap != null)
                {
                    FreeMemory(ScriptRunVisualToLogicalMap);
                    ScriptRunVisualToLogicalMap = null;
                }
            }

            private void InitializeGlyphBuffers()
            {
                GlyphCapacity = 0;
                Glyphs = null;
                GlyphVisualAttributes = null;
                GlyphAdvanceWidths = null;
                GlyphOffsets = null;
            }

            private void ReAllocGlyphBuffers(int newGlyphCapacity)
            {
                Glyphs = (ushort*)ReAllocMemory(Glyphs, newGlyphCapacity * sizeof(ushort));
                GlyphVisualAttributes = (SCRIPT_VISATTR*)ReAllocMemory(GlyphVisualAttributes, newGlyphCapacity * sizeof(SCRIPT_VISATTR));
                GlyphAdvanceWidths = (int*)ReAllocMemory(GlyphAdvanceWidths, newGlyphCapacity * sizeof(int));
                GlyphOffsets = (GOFFSET*)ReAllocMemory(GlyphOffsets, newGlyphCapacity * sizeof(GOFFSET));
            }

            private void FreeGlyphBuffers()
            {
                GlyphCapacity = 0;

                if (Glyphs != null)
                {
                    FreeMemory(Glyphs);
                    Glyphs = null;
                }

                if (GlyphVisualAttributes != null)
                {
                    FreeMemory(GlyphVisualAttributes);
                    GlyphVisualAttributes = null;
                }

                if (GlyphAdvanceWidths != null)
                {
                    FreeMemory(GlyphAdvanceWidths);
                    GlyphAdvanceWidths = null;
                }

                if (GlyphOffsets != null)
                {
                    FreeMemory(GlyphOffsets);
                    GlyphOffsets = null;
                }
            }

            private static int NewCapacity(int currentCapacity, int initialCapacity, int desiredCapacity)
            {
                int newCapacity = Math.Max(currentCapacity, initialCapacity);

                while (newCapacity < desiredCapacity)
                    newCapacity *= 2;

                return newCapacity;
            }
        }

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
        private struct ScriptRun
        {
            private Run run;
            public SCRIPT_ANALYSIS ScriptAnalysis;
            public int CharIndexInParagraph;

            public int GlyphIndexInParagraph;
            public int GlyphCount;

            public int Height;
            public ABC ABC;

            public void Initialize(Run* run, SCRIPT_ITEM* scriptItem, int charIndexInParagraph, int charCount)
            {
                this.run = *run;
                if (this.run.RunKind == RunKind.Text)
                    this.run.CharCount = charCount;

                CharIndexInParagraph = charIndexInParagraph;
                ScriptAnalysis = scriptItem->a;
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

        /// <summary>
        /// Internal structure that maps a visual line onto a particular range of runs
        /// within a paragraph and includes the character position of required line breaks.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct ScriptLine
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
                ScriptRunCount = 0;
                TruncatedLeadingCharsCount = 0;
                TruncatedTrailingCharsCount = 0;
            }
        }

        private sealed class ScriptCacheTable
        {
            private readonly Dictionary<Font, ScriptCache> dict;

            public ScriptCacheTable()
            {
                dict = new Dictionary<Font, ScriptCache>();
            }

            ~ScriptCacheTable()
            {
                Clear();
            }

            public void Clear()
            {
                foreach (ScriptCache scriptCache in dict.Values)
                {
                    if (scriptCache.ScriptCachePtr != IntPtr.Zero)
                        NativeMethods.ScriptFreeCache(ref scriptCache.ScriptCachePtr);
                }
            }

            public ScriptCache this[Font font]
            {
                get
                {
                    ScriptCache scriptCache;
                    if (!dict.TryGetValue(font, out scriptCache))
                    {
                        scriptCache = new ScriptCache();
                        dict.Add(font, scriptCache);
                    }
                    return scriptCache;
                }
            }
        }

        private sealed class ScriptCache
        {
            public IntPtr ScriptCachePtr;
        }

        private sealed class ScriptParagraphCache
        {
            private readonly UnmanagedBuffer<LruEntry> buffer;
            private int nextToken;

            private struct LruEntry
            {
                public int ParagraphIndex;
                public int Token;
                public ScriptParagraph Paragraph;
            }

            public ScriptParagraphCache(int size)
            {
                buffer = new UnmanagedBuffer<LruEntry>(size);
            }

            ~ScriptParagraphCache()
            {
                FreeBuffer();
            }

            public void Clear()
            {
                nextToken = 0;
                buffer.Count = 0;
            }

            public bool TryGetScriptParagraph(int paragraphIndex, out ScriptParagraph* scriptParagraph)
            {
                // Handle wrap-around at 32bits by thrashing all tokens.
                LruEntry* firstEntry = (LruEntry*)buffer.GetPointer();
                LruEntry* endEntry = firstEntry + buffer.Count;
                if (nextToken < 0)
                {
                    nextToken = 0;
                    for (LruEntry* currentEntry = firstEntry; currentEntry != endEntry; currentEntry++)
                        currentEntry->Token = nextToken++;
                }

                // Search for a matching paragraph and return it if found.
                // Make note of least recently used entry just in case.
                LruEntry* lruEntry = null;
                int lruToken = -1;
                for (LruEntry* currentEntry = firstEntry; currentEntry != endEntry; currentEntry++)
                {
                    if (currentEntry->ParagraphIndex == paragraphIndex)
                    {
                        currentEntry->Token = nextToken++;
                        scriptParagraph = &currentEntry->Paragraph;
                        return true;
                    }

                    int token = currentEntry->Token;
                    if (token < lruToken)
                    {
                        lruToken = token;
                        lruEntry = currentEntry;
                    }
                }

                // Decide whether to allocate a new entry from remaining capacity or replace the least-recently used one.
                LruEntry* entryToReplace;
                if (buffer.Count != buffer.Capacity)
                {
                    entryToReplace = endEntry;
                    entryToReplace->Paragraph.Initialize();
                    buffer.Count += 1;
                }
                else
                {
                    entryToReplace = lruEntry;
                }

                // Return the entry.
                entryToReplace->Token = nextToken++;
                entryToReplace->ParagraphIndex = paragraphIndex;
                scriptParagraph = &entryToReplace->Paragraph;
                return false;
            }

            private void FreeBuffer()
            {
                LruEntry* first = (LruEntry*) buffer.GetPointer();
                LruEntry* end = first + buffer.Count;
                for (LruEntry* current = first; current != end; current++)
                    current->Paragraph.Free();

                buffer.Clear();
            }
        }
    }
}
