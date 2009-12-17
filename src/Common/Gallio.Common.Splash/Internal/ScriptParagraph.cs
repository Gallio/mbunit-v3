// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Splash.Native;

namespace Gallio.Common.Splash.Internal
{
    /// <summary>
    /// Internal structure with script information necessary for layout and display of a paragraph.
    /// Includes script information for each run in the paragraph.
    /// </summary>
    /// <remarks>
    /// We pool allocated script paragraph structures and cache them to improve performance
    /// and reduce the number of memory allocations.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct ScriptParagraph
    {
        private const int InitialCapacityForCharsPerParagraph = 64;
        private const int InitialCapacityForScriptRunsPerParagraph = 8;
        private const int InitialCapacityForGlyphsPerParagraph = 64 * 3 / 2 + 16;

        public int CharIndex;
        public int CharCount;
        public int CharCapacity;
        public ushort* CharLogicalClusters;           // 1 element per character
        public SCRIPT_LOGATTR* CharLogicalAttributes; // 1 element per character

        public int ScriptRunCount;
        public int ScriptRunCapacity;
        public ScriptRun* ScriptRuns;                 // 1 element per script run

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
            CharLogicalClusters = (ushort*)Memory.Alloc(CharCapacity * sizeof(ushort));
            CharLogicalAttributes = (SCRIPT_LOGATTR*)Memory.Alloc(CharCapacity * sizeof(SCRIPT_LOGATTR));
        }

        private void FreeCharBuffers()
        {
            CharCapacity = 0;

            if (CharLogicalClusters != null)
            {
                Memory.Free(CharLogicalClusters);
                CharLogicalClusters = null;
            }

            if (CharLogicalAttributes != null)
            {
                Memory.Free(CharLogicalAttributes);
                CharLogicalAttributes = null;
            }
        }

        private void InitializeScriptRunBuffers()
        {
            ScriptRunCapacity = 0;
            ScriptRuns = null;
        }

        private void AllocScriptRunBuffers()
        {
            ScriptRuns = (ScriptRun*)Memory.Alloc(ScriptRunCapacity * sizeof(ScriptRun));
        }

        private void FreeScriptRunBuffers()
        {
            ScriptRunCapacity = 0;

            if (ScriptRuns != null)
            {
                Memory.Free(ScriptRuns);
                ScriptRuns = null;
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
            Glyphs = (ushort*)Memory.ReAlloc(Glyphs, newGlyphCapacity * sizeof(ushort));
            GlyphVisualAttributes = (SCRIPT_VISATTR*)Memory.ReAlloc(GlyphVisualAttributes, newGlyphCapacity * sizeof(SCRIPT_VISATTR));
            GlyphAdvanceWidths = (int*)Memory.ReAlloc(GlyphAdvanceWidths, newGlyphCapacity * sizeof(int));
            GlyphOffsets = (GOFFSET*)Memory.ReAlloc(GlyphOffsets, newGlyphCapacity * sizeof(GOFFSET));
        }

        private void FreeGlyphBuffers()
        {
            GlyphCapacity = 0;

            if (Glyphs != null)
            {
                Memory.Free(Glyphs);
                Glyphs = null;
            }

            if (GlyphVisualAttributes != null)
            {
                Memory.Free(GlyphVisualAttributes);
                GlyphVisualAttributes = null;
            }

            if (GlyphAdvanceWidths != null)
            {
                Memory.Free(GlyphAdvanceWidths);
                GlyphAdvanceWidths = null;
            }

            if (GlyphOffsets != null)
            {
                Memory.Free(GlyphOffsets);
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
}
