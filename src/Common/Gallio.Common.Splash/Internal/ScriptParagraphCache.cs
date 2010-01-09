// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Text;

namespace Gallio.Common.Splash.Internal
{
    internal unsafe sealed class ScriptParagraphCache
    {
        private readonly UnmanagedBuffer buffer;
        private int nextToken;

        private struct LruEntry
        {
            public int ParagraphIndex;
            public int Token;
            public ScriptParagraph Paragraph;
        }

        public ScriptParagraphCache(int size)
        {
            buffer = new UnmanagedBuffer(size, sizeof(LruEntry));
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

        public void RemoveScriptParagraphsStartingFrom(int paragraphIndex)
        {
            if (paragraphIndex <= 0)
            {
                Clear();
            }
            else
            {
                LruEntry* firstEntry = (LruEntry*)buffer.GetPointer();
                LruEntry* endEntry = firstEntry + buffer.Count;
                for (LruEntry* currentEntry = firstEntry; currentEntry != endEntry; currentEntry++)
                {
                    if (currentEntry->ParagraphIndex >= paragraphIndex)
                    {
                        currentEntry->ParagraphIndex = -1;
                        currentEntry->Token = 0;
                    }
                }
            }
        }

        public bool TryGetScriptParagraph(int paragraphIndex, out ScriptParagraph* scriptParagraph)
        {
            // Handle wrap-around at 32bits by thrashing all tokens.
            LruEntry* firstEntry = (LruEntry*)buffer.GetPointer();
            LruEntry* endEntry = firstEntry + buffer.Count;
            if (nextToken == int.MaxValue)
            {
                nextToken = 0;
                for (LruEntry* currentEntry = firstEntry; currentEntry != endEntry; currentEntry++)
                    currentEntry->Token = nextToken++;
            }

            // Search for a matching paragraph and return it if found.
            // Make note of least recently used entry just in case.
            LruEntry* lruEntry = null;
            int lruToken = int.MaxValue;
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
            LruEntry* first = (LruEntry*)buffer.GetPointer();
            LruEntry* end = first + buffer.Count;
            for (LruEntry* current = first; current != end; current++)
                current->Paragraph.Free();

            buffer.Clear();
        }
    }
}
