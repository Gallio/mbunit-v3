using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Gallio.Common.Splash.Native;

namespace Gallio.Common.Splash
{
    public unsafe partial class SplashView
    {
        /// <summary>
        /// Computes the layout of a <see cref="ScriptParagraph"/> and appends one or
        /// more <see cref="ScriptLine"/>s to the <see cref="scriptLineBuffer"/>.
        /// </summary>
        /// <param name="hdcState">The HDC state.</param>
        /// <param name="paragraphIndex">The paragraph index to layout.</param>
        /// <param name="textLayoutWidth">The width of the layout.</param>
        /// <param name="textLayoutHeight">The height of the layout, updated as new lines are added.</param>
        private void AppendScriptLinesForScriptParagraph(HDCState hdcState, int paragraphIndex, int textLayoutWidth, ref int textLayoutHeight)
        {
            ScriptParagraph* scriptParagraph = GetScriptParagraph(hdcState, paragraphIndex);
            int scriptRunCount = scriptParagraph->ScriptRunCount;

            // Loop over all runs in visual order packing runs into lines as they fit and
            // performing tab expansion or word wrap if needed.  Always runs at least once
            // even if a paragraph has no runs to ensure that all paragraphs always have at
            // least one line.
            ScriptRun* scriptRuns = scriptParagraph->ScriptRuns;
            int scriptRunIndex = 0;
            int scriptLineTruncatedLeadingCharsCount = 0;
            bool firstLineAndScriptRun = true;
            do
            {
                int scriptLineIndex = scriptLineBuffer.Count;
                scriptLineBuffer.GrowBy(1);
                ScriptLine* scriptLine = GetScriptLineZero() + scriptLineIndex;

                scriptLine->ParagraphIndex = paragraphIndex;
                scriptLine->Y = textLayoutHeight;
                scriptLine->ScriptRunIndex = scriptRunIndex;
                scriptLine->TruncatedLeadingCharsCount = scriptLineTruncatedLeadingCharsCount;

                int nextScriptLineTruncatedLeadingCharsCount = 0;
                int scriptLineTruncatedTrailingCharsCount = 0;
                int scriptLineIndent = 0;
                int scriptLineWidthRemaining = textLayoutWidth;
                int scriptLineHeight = 0;
                int scriptLineScriptRunCount = 0;
                int currentX = 0;

                bool hasPrecedingWhitespace = false;
                int lastSoftBreakScriptRunIndex = -1;
                int lastSoftBreakCharIndex = -1;

                // Pack as many runs into the line as possible.  Skipped if the paragraph has no runs.
                for (; scriptRunIndex != scriptRunCount; scriptRunIndex++)
                {
                    ScriptRun* scriptRun = scriptRuns + scriptParagraph->ScriptRunVisualToLogicalMap[scriptRunIndex];
                    Style style = styleTable[scriptRun->StyleIndex];

                    bool firstScriptRun = scriptLineScriptRunCount == 0;
                    if (firstScriptRun)
                    {
                        // Handle block-level styles and layout.
                        if (firstLineAndScriptRun)
                        {
                            scriptLineIndent += style.FirstLineIndent;
                            firstLineAndScriptRun = false;
                        }

                        // Handle line-level styles and layout.
                        scriptLineIndent += style.LeftMargin;
                        scriptLineWidthRemaining -= style.RightMargin;
                        scriptLineWidthRemaining -= scriptLineIndent;
                        currentX = scriptLineIndent;

                        // If the line is already too wide due to indentation or margins, then
                        // allow some extra space so that layout actually terminates.
                        if (scriptLineWidthRemaining < MinimumTextLayoutWidth)
                            scriptLineWidthRemaining = MinimumTextLayoutWidth;
                    }

                    // Adjust height.
                    int scriptRunHeight = scriptRun->Height;
                    if (scriptRunHeight > scriptLineHeight)
                        scriptLineHeight = scriptRunHeight;

                    // Assume we will include this run.  We will adjust the count later if needed.
                    scriptLineScriptRunCount += 1;

                    // Process the run.
                    bool requiresTabExpansion = scriptRun->RequiresTabExpansion;
                    bool rtl = scriptRun->ScriptAnalysis.fRTL;
                    int charCount = scriptRun->CharCount;
                    char* chars = requiresTabExpansion ? scriptRun->Chars(scriptParagraph, GetCharZero()) : null;
                    SCRIPT_LOGATTR* logicalAttributes = scriptRun->CharLogicalAttributes(scriptParagraph);
                    ushort* logicalClusters = scriptRun->CharLogicalClusters(scriptParagraph);
                    int glyphCount = scriptRun->GlyphCount;
                    int* glyphAdvanceWidths = scriptRun->GlyphAdvanceWidths(scriptParagraph);
                    RunKind runKind = scriptRun->RunKind;
                    for (int charIndex = firstScriptRun ? scriptLineTruncatedLeadingCharsCount : 0; charIndex < charCount; )
                    {
                        int glyphClusterIndex = logicalClusters[charIndex];

                        // Perform tab expansion if necessary.
                        if (requiresTabExpansion && chars[charIndex] == '\t')
                        {
                            int nextX = style.TabStopRuler.AdvanceToNextTabStop(currentX);
                            int oldAdvanceWidth = glyphAdvanceWidths[glyphClusterIndex];
                            int newAdvanceWidth = nextX - currentX;
                            glyphAdvanceWidths[glyphClusterIndex] = newAdvanceWidth;
                            scriptRun->ABC.abcB += newAdvanceWidth - oldAdvanceWidth;
                        }

                        // Explore the glyph cluster at the current character position.
                        int clusterAdvanceWidth;
                        int clusterCharCount;
                        int clusterGlyphCount;
                        GetGlyphClusterInfo(charIndex, charCount, logicalClusters, glyphCount, glyphAdvanceWidths, rtl,
                            out clusterAdvanceWidth, out clusterCharCount, out clusterGlyphCount);

                        // Get information about this character range.
                        bool whiteSpace;
                        bool softBreak;
                        if (runKind == RunKind.Object)
                        {
                            whiteSpace = false;
                            softBreak = true;
                        }
                        else
                        {
                            CombineWordWrapAttributesOverCharacterRange(charIndex, clusterCharCount, logicalAttributes,
                                out whiteSpace, out softBreak);

                            if (hasPrecedingWhitespace)
                                softBreak = true;
                        }

                        if (whiteSpace)
                        {
                            // Whitespace.  Defer overflow processing until later.
                            hasPrecedingWhitespace = true;
                        }
                        else
                        {
                            // Not whitespace.
                            if (softBreak)
                            {
                                lastSoftBreakScriptRunIndex = scriptRunIndex;
                                lastSoftBreakCharIndex = charIndex;
                            }

                            // Check for overflow.
                            if (scriptLineWidthRemaining < clusterAdvanceWidth)
                            {
                                // Overflow happened.
                                // Choose a breakpoint.
                                if (hasPrecedingWhitespace)
                                {
                                    // Break on current character if there is preceding whitespace.
                                    if (charIndex == 0)
                                    {
                                        scriptLineScriptRunCount -= 1;
                                    }
                                    else
                                    {
                                        scriptLineTruncatedTrailingCharsCount = charCount - charIndex;
                                        nextScriptLineTruncatedLeadingCharsCount = charIndex;
                                    }
                                }
                                else if (firstScriptRun && charIndex == 0)
                                {
                                    // If this is the first run and we cannot fit any chars, then hardbreak
                                    // after first char and just let it overflow.
                                    scriptLineTruncatedTrailingCharsCount = charCount - 1;
                                    nextScriptLineTruncatedLeadingCharsCount = 1;
                                }
                                else if (lastSoftBreakCharIndex == 0)
                                {
                                    // Soft break on first character of a run.
                                    scriptLineScriptRunCount -= scriptRunIndex - lastSoftBreakScriptRunIndex + 1;
                                    scriptRunIndex = lastSoftBreakScriptRunIndex;
                                }
                                else if (lastSoftBreakCharIndex > 0)
                                {
                                    // Soft break in the middle of a run.
                                    ScriptRun* lastSoftBreakScriptRun = scriptRuns + scriptParagraph->ScriptRunVisualToLogicalMap[lastSoftBreakScriptRunIndex];
                                    scriptLineScriptRunCount -= scriptRunIndex - lastSoftBreakScriptRunIndex;
                                    scriptRunIndex = lastSoftBreakScriptRunIndex;
                                    scriptLineTruncatedTrailingCharsCount = lastSoftBreakScriptRun->CharCount - lastSoftBreakCharIndex;
                                    nextScriptLineTruncatedLeadingCharsCount = lastSoftBreakCharIndex;
                                }
                                else if (charIndex == 0)
                                {
                                    // Hard break on first character of a run.
                                    scriptLineScriptRunCount -= 1;
                                }
                                else
                                {
                                    // Hard break in the middle of a run.
                                    scriptLineTruncatedTrailingCharsCount = charCount - charIndex;
                                    nextScriptLineTruncatedLeadingCharsCount = charIndex;
                                }

                                Debug.Assert(scriptLineScriptRunCount != 0, "Overflow logic should not produce an empty line with no runs.");
                                goto FinishLine;
                            }

                            // Not whitespace and no overflow.
                            hasPrecedingWhitespace = false;
                        }

                        // Advance to next char.
                        scriptLineWidthRemaining -= clusterAdvanceWidth;
                        currentX += clusterAdvanceWidth;
                        charIndex += clusterCharCount;
                    } // end of loop over chars...
                }

            FinishLine:
                scriptLine->X = scriptLineIndent;
                scriptLine->Height = scriptLineHeight;
                scriptLine->ScriptRunCount = scriptLineScriptRunCount;
                scriptLine->TruncatedTrailingCharsCount = scriptLineTruncatedTrailingCharsCount;

                scriptLineTruncatedLeadingCharsCount = nextScriptLineTruncatedLeadingCharsCount;
                textLayoutHeight += scriptLineHeight;
            }
            while (scriptRunIndex != scriptRunCount);
        }
        
        /// <summary>
        /// Gets information about the advance width, number of characters and number of glyphs
        /// in a glyph cluster starting at a specified character position.
        /// </summary>
        private static void GetGlyphClusterInfo(int charIndex, int charCount,
            ushort* logicalClusters, int glyphCount, int* glyphAdvanceWidths, bool rtl,
            out int clusterAdvanceWidth, out int clusterCharCount, out int clusterGlyphCount)
        {
            int glyphClusterIndex = logicalClusters[charIndex];
            clusterAdvanceWidth = glyphAdvanceWidths[glyphClusterIndex];
            clusterCharCount = 1;

            int nextGlyphClusterIndex;
            for (; ; )
            {
                int nextCharIndex = charIndex + clusterCharCount;
                if (nextCharIndex >= charCount)
                {
                    nextGlyphClusterIndex = rtl ? -1 : glyphCount;
                    break;
                }

                nextGlyphClusterIndex = logicalClusters[nextCharIndex];
                if (nextGlyphClusterIndex != glyphClusterIndex)
                    break;

                clusterCharCount += 1;
            }

            if (rtl)
            {
                clusterGlyphCount = glyphClusterIndex - nextGlyphClusterIndex;
                while (--glyphClusterIndex > nextGlyphClusterIndex)
                    clusterAdvanceWidth += glyphAdvanceWidths[glyphClusterIndex];
            }
            else
            {
                clusterGlyphCount = nextGlyphClusterIndex - glyphClusterIndex;
                while (++glyphClusterIndex < nextGlyphClusterIndex)
                    clusterAdvanceWidth += glyphAdvanceWidths[glyphClusterIndex];
            }
        }

        /// <summary>
        /// Combines word-wrap related attributes over a character range.
        /// </summary>
        private void CombineWordWrapAttributesOverCharacterRange(int charIndex, int charCount, SCRIPT_LOGATTR* logicalAttributes,
            out bool whiteSpace, out bool softBreak)
        {
            whiteSpace = logicalAttributes[charIndex].fWhiteSpace;
            softBreak = logicalAttributes[charIndex].fSoftBreak;

            while (++charIndex < charCount)
            {
                whiteSpace |= logicalAttributes[charIndex].fWhiteSpace;
                softBreak |= logicalAttributes[charIndex].fSoftBreak;
            }
        }

        /// <summary>
        /// Gets a fully analyzed <see cref="ScriptParagraph"/> by paragraph index.
        /// </summary>
        private ScriptParagraph* GetScriptParagraph(HDCState hdcState, int paragraphIndex)
        {
            ScriptParagraph* scriptParagraph;
            if (! scriptParagraphCache.TryGetScriptParagraph(paragraphIndex, out scriptParagraph))
            {
                Paragraph* paragraph = GetParagraphZero() + paragraphIndex;
                AnalyzeParagraph(hdcState, paragraph, scriptParagraph);
            }

            return scriptParagraph;
        }

        /// <summary>
        /// Analyzes a <see cref="Paragraph" /> to populate a <see cref="ScriptParagraph"/>.
        /// </summary>
        private void AnalyzeParagraph(HDCState hdcState, Paragraph* paragraph, ScriptParagraph* scriptParagraph)
        {
            int charIndex = paragraph->CharIndex;
            int charCount = paragraph->CharCount;

            scriptParagraph->CharIndex = charIndex;
            scriptParagraph->CharCount = charCount;
            scriptParagraph->ScriptRunCount = 0;
            scriptParagraph->GlyphCount = 0;

            Run* runZero = GetRunZero();
            Run* runs = runZero + paragraph->RunIndex;
            int runCount = paragraph->RunCount;
            if (runCount == 0)
            {
                // skip empty trailing paragraphs (caused by having a newline at end of document)
                return;
            }

            Debug.Assert(charCount != 0, "Found a paragraph with at least one run but no characters!");

            char* charZero = GetCharZero();
            char* chars = charZero + charIndex;

            // Step 1. Itemize the script items within the paragraph.
            //         Each script item represents a chunk of text (such as a word)
            //         for Unicode rendering purposes.
            scriptParagraph->EnsureCharCapacity(charCount);

            SCRIPT_ITEM* tempScriptItems;
            int tempScriptItemCount;
            ScriptItemize(chars, charCount, tempScriptItemBuffer, out tempScriptItems, out tempScriptItemCount);

            // Step 2. Compute logical attributes for characters in the paragraph for word-break purposes.
            ScriptBreak(scriptParagraph, chars, tempScriptItems, tempScriptItemCount);

            // Step 3. Split the Runs on SCRIPT_ITEM boundaries to produce ScriptRuns.
            byte* tempEmbeddingLevels;
            SplitScriptRuns(scriptParagraph, runs, runCount, tempScriptItems, tempScriptItemCount, tempEmbeddingLevelBuffer, out tempEmbeddingLevels);

            // Step 4. Layout the ScriptRuns to generate a visual-to-logical order for rendering.
            ScriptLayoutVisualToLogicalMap(scriptParagraph, tempEmbeddingLevels);

            // Step 5. Shape and Place glyphs and Measure embedded objects one run at a time.
            //         We do this in linear order because it is simpler and we do not care about
            //         visual order until later when we pack runs into lines and draw them.
            ScriptRun* scriptRun = scriptParagraph->ScriptRuns;
            ScriptRun* endScriptRun = scriptRun + scriptParagraph->ScriptRunCount;
            for (; scriptRun != endScriptRun; scriptRun++)
            {
                Style scriptRunStyle = styleTable[scriptRun->StyleIndex];

                if (scriptRun->RunKind == RunKind.Text)
                {
                    ScriptCache scriptCache = hdcState.SelectFont(scriptRunStyle.Font);

                    ScriptShapeAndPlace(hdcState.HDC, ref scriptCache.ScriptCachePtr, scriptParagraph, scriptRun, chars);

                    int height = GetFontHeight(hdcState.HDC, ref scriptCache.ScriptCachePtr);
                    scriptRun->Height = height;

                    // Workaround the fact that UniScribe does not consider a TAB to be whitespace so
                    // ScriptBreak actually produced incorrect logical attributes for the TABs.
                    if (scriptRun->RequiresTabExpansion)
                    {
                        int scriptRunCharIndexInParagraph = scriptRun->CharIndexInParagraph;
                        int scriptRunCharEndIndex = scriptRunCharIndexInParagraph + scriptRun->CharCount;
                        SCRIPT_LOGATTR* scriptParagraphLogicalAttributes = scriptParagraph->CharLogicalAttributes;
                        for (int i = scriptRunCharIndexInParagraph; i < scriptRunCharEndIndex; i++)
                        {
                            if (chars[i] == '\t')
                                scriptParagraphLogicalAttributes[i].SetfWhiteSpace();
                        }
                    }
                }
                else
                {
                    EmbeddedObject obj = objectTable[scriptRun->ObjectIndex];

                    EmbeddedObjectMeasurements measurements = SafeMeasure(obj, scriptRunStyle);
                    scriptRun->GlyphCount = 0;
                    scriptRun->GlyphIndexInParagraph = 0;
                    scriptRun->Height = measurements.Size.Height + measurements.Margin.Vertical;
                    scriptRun->ABC.abcA = measurements.Margin.Left;
                    scriptRun->ABC.abcB = measurements.Size.Width;
                    scriptRun->ABC.abcC = measurements.Margin.Right;
                }
            }
        }

        private static void ScriptItemize(char* chars, int charCount, UnmanagedBuffer<SCRIPT_ITEM> tempScriptItemBuffer, out SCRIPT_ITEM* scriptItems, out int scriptItemCount)
        {
            var scriptControl = new SCRIPT_CONTROL();
            var scriptState = new SCRIPT_STATE();

            for (; ; )
            {
                scriptItems = (SCRIPT_ITEM*)tempScriptItemBuffer.GetPointer();
                int result = NativeMethods.ScriptItemize(chars, charCount,
                    tempScriptItemBuffer.Capacity - 1 /*itemize needs 1 extra item beyond capacity as a sentinel*/,
                    &scriptControl, &scriptState, scriptItems, out scriptItemCount);
                if (result == NativeConstants.S_OK)
                    return;
                if (result != NativeConstants.E_OUTOFMEMORY)
                    Marshal.ThrowExceptionForHR(result);

                tempScriptItemBuffer.SetCapacity(tempScriptItemBuffer.Capacity * 2);
            }
        }

        private static void ScriptBreak(ScriptParagraph* scriptParagraph, char* chars,
            SCRIPT_ITEM* scriptItems, int scriptItemCount)
        {
            SCRIPT_ITEM* scriptItem = scriptItems;
            SCRIPT_ITEM* endScriptItem = scriptItems + scriptItemCount;
            SCRIPT_LOGATTR* logicalAttributes = scriptParagraph->CharLogicalAttributes;

            for (; scriptItem != endScriptItem; scriptItem++)
            {
                int scriptItemCharPos = scriptItem->iCharPos;
                int nextScriptItemCharPos = (scriptItem + 1)->iCharPos;
                int scriptItemCharCount = nextScriptItemCharPos - scriptItemCharPos;
                char* scriptItemChars = chars + scriptItemCharPos;

                int result = NativeMethods.ScriptBreak(scriptItemChars,
                    scriptItemCharCount,
                    &scriptItem->a, logicalAttributes);
                if (result != 0)
                    Marshal.ThrowExceptionForHR(result);

                logicalAttributes += scriptItemCharCount;
            }
        }

        private static void SplitScriptRuns(ScriptParagraph* scriptParagraph, Run* runs, int runCount, SCRIPT_ITEM* scriptItems, int scriptItemCount, UnmanagedBuffer<byte> tempEmbeddingLevelBuffer, out byte* embeddingLevels)
        {
            // Reserve space for the largest possible number of script runs.
            int maxScriptRuns = runCount + scriptItemCount;
            scriptParagraph->EnsureScriptRunCapacity(maxScriptRuns);
            tempEmbeddingLevelBuffer.EnsureCapacity(maxScriptRuns);
            embeddingLevels = (byte*)tempEmbeddingLevelBuffer.GetPointer();

            // Merge Runs and SCRIPT_ITEMs to create ScriptRuns.
            Run* run = runs;
            int runStartCharIndex = 0;
            Run* endRun = runs + runCount;
            SCRIPT_ITEM* item = scriptItems;
            SCRIPT_ITEM* endItem = scriptItems + scriptItemCount;
            ScriptRun* startScriptRun = scriptParagraph->ScriptRuns;
            ScriptRun* scriptRun = startScriptRun;
            byte* embeddingLevel = embeddingLevels;

            int currentCharIndex = 0;
            for (; ; scriptRun++)
            {
                int runEndCharIndex = runStartCharIndex + run->CharCount;
                int itemEndCharIndex = (item + 1)->iCharPos;

                *(embeddingLevel++) = (byte)item->a.s.uBidiLevel;

                if (itemEndCharIndex < runEndCharIndex)
                {
                    scriptRun->Initialize(run, item, currentCharIndex, itemEndCharIndex - currentCharIndex);
                    currentCharIndex = itemEndCharIndex;
                    item++;
                }
                else if (runEndCharIndex < itemEndCharIndex)
                {
                    scriptRun->Initialize(run, item, currentCharIndex, runEndCharIndex - currentCharIndex);
                    runStartCharIndex = currentCharIndex = runEndCharIndex;
                    run++;
                }
                else
                {
                    scriptRun->Initialize(run, item, currentCharIndex, runEndCharIndex - currentCharIndex);
                    runStartCharIndex = currentCharIndex = runEndCharIndex;
                    run++;
                    item++;

                    // All sequences of Runs and Items should always end on the same char so we only
                    // need to check for termination here.
                    if (run == endRun)
                    {
                        Debug.Assert(item == endItem, "Inconsistent Run and SCRIPT_ITEM sequences.");
                        break;
                    }
                }
            }

            scriptParagraph->ScriptRunCount = (int)(scriptRun - startScriptRun) + 1;
        }

        private static void ScriptLayoutVisualToLogicalMap(ScriptParagraph* scriptParagraph, byte* embeddingLevels)
        {
            int result = NativeMethods.ScriptLayout(scriptParagraph->ScriptRunCount, embeddingLevels,
                scriptParagraph->ScriptRunVisualToLogicalMap, null);
            if (result != NativeConstants.S_OK)
                Marshal.ThrowExceptionForHR(result);
        }

        private static void ScriptShapeAndPlace(IntPtr hdc, ref IntPtr scriptCache, ScriptParagraph* scriptParagraph, ScriptRun* scriptRun, char* paragraphChars)
        {
            int charIndexInParagraph = scriptRun->CharIndexInParagraph;
            char* chars = paragraphChars + charIndexInParagraph;
            int charCount = scriptRun->CharCount;
            int glyphIndexInParagraph = scriptParagraph->GlyphCount;

            SCRIPT_ANALYSIS* scriptAnalysis = &scriptRun->ScriptAnalysis;
            int glyphCapacity = charCount * 2;
            int glyphCount;
            ushort* glyphs;
            SCRIPT_VISATTR* visualAttributes;
            int result;
            for (; ; )
            {
                scriptParagraph->EnsureGlyphCapacityAndPreserveContents(glyphIndexInParagraph + glyphCapacity);

                glyphs = scriptParagraph->Glyphs + glyphIndexInParagraph;
                ushort* logicalClusters = scriptParagraph->CharLogicalClusters + charIndexInParagraph;
                visualAttributes = scriptParagraph->GlyphVisualAttributes + glyphIndexInParagraph;
                result = NativeMethods.ScriptShape(hdc, ref scriptCache, chars, charCount, glyphCapacity,
                    scriptAnalysis, glyphs, logicalClusters, visualAttributes,
                    out glyphCount);

                if (result == NativeConstants.S_OK)
                    break;

                if (result != NativeConstants.E_OUTOFMEMORY)
                    Marshal.ThrowExceptionForHR(result);

                glyphCapacity *= 2;
            }

            int* advanceWidths = scriptParagraph->GlyphAdvanceWidths + glyphIndexInParagraph;
            GOFFSET* glyphOffsets = scriptParagraph->GlyphOffsets + glyphIndexInParagraph;
            result = NativeMethods.ScriptPlace(hdc, ref scriptCache, glyphs, glyphCount,
                visualAttributes, scriptAnalysis, advanceWidths, glyphOffsets, out scriptRun->ABC);
            if (result != NativeConstants.S_OK)
                Marshal.ThrowExceptionForHR(result);

            scriptRun->GlyphIndexInParagraph = glyphIndexInParagraph;
            scriptRun->GlyphCount = glyphCount;
            scriptParagraph->GlyphCount += glyphCount;
        }

        private static void ScriptTextOut(IntPtr hdc, ref IntPtr scriptCache, int x, int y,
            ExtTextOutOptions fuOptions, RECT* clipRect,
            ScriptParagraph* scriptParagraph, ScriptRun* scriptRun,
            int truncatedLeadingGlyphsCount, int truncatedTrailingGlyphsCount)
        {
            int result = NativeMethods.ScriptTextOut(hdc, ref scriptCache, x, y, fuOptions, clipRect,
                &scriptRun->ScriptAnalysis,
                null, 0,
                scriptRun->Glyphs(scriptParagraph) + truncatedLeadingGlyphsCount,
                scriptRun->GlyphCount - truncatedLeadingGlyphsCount - truncatedTrailingGlyphsCount,
                scriptRun->GlyphAdvanceWidths(scriptParagraph) + truncatedLeadingGlyphsCount,
                null,
                scriptRun->GlyphOffsets(scriptParagraph) + truncatedLeadingGlyphsCount);
            if (result != NativeConstants.S_OK)
                Marshal.ThrowExceptionForHR(result);
        }

        private static int GetFontHeight(IntPtr hdc, ref IntPtr scriptCache)
        {
            int height;
            int result = NativeMethods.ScriptCacheGetHeight(hdc, ref scriptCache, out height);
            if (result != NativeConstants.S_OK)
                Marshal.ThrowExceptionForHR(result);

            return height;
        }

        private static EmbeddedObjectMeasurements SafeMeasure(EmbeddedObject obj, Style style)
        {
            try
            {
                return obj.Measure(style);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception while measuring an embedded object.\n{0}", ex));
                return new EmbeddedObjectMeasurements(new Size(0, 0), new Padding(0));
            }
        }

        private sealed class HDCState
        {
            public readonly IntPtr HDC;

            private readonly ScriptCacheTable scriptCacheTable;
            private Font currentFont;
            private ScriptCache currentScriptCache;

            public HDCState(IntPtr hdc, ScriptCacheTable scriptCacheTable)
            {
                HDC = hdc;
                this.scriptCacheTable = scriptCacheTable;
            }

            public ScriptCache SelectFont(Font font)
            {
                if (currentFont != font)
                {
                    NativeMethods.SelectObject(HDC, font.ToHfont());
                    currentFont = font;
                    currentScriptCache = scriptCacheTable[currentFont];
                }

                return currentScriptCache;
            }
        }
    }
}
