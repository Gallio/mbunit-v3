using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
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
        /// <param name="rightToLeftLayout">True if the document direction is right to left.</param>
        /// <param name="textLayoutWidth">The width of the layout.</param>
        /// <param name="textLayoutHeight">The height of the layout, updated as new lines are added.</param>
        private void AppendScriptLinesForScriptParagraph(HDCState hdcState, int paragraphIndex, bool rightToLeftLayout,
            int textLayoutWidth, ref int textLayoutHeight)
        {
            ScriptParagraph* scriptParagraph = GetScriptParagraph(hdcState, paragraphIndex, rightToLeftLayout);
            int scriptRunCount = scriptParagraph->ScriptRunCount;

            // Always allocate at least one script line for a paragraph.
            ScriptLine* scriptLine = AddScriptLine(paragraphIndex, 0, textLayoutHeight);

            // Handle paragraph with no runs (sentinel).
            if (scriptRunCount == 0)
            {
                scriptLine->Height = MinimumScriptLineHeight;
                textLayoutHeight += MinimumScriptLineHeight;
                return;
            }

            // Apply block-level style information for the paragraph from the first logical run.
            ScriptRun* scriptRuns = scriptParagraph->ScriptRuns;
            Style paragraphStyle = styleTable[scriptRuns[0].StyleIndex];
            scriptLine->X = paragraphStyle.LeftMargin + paragraphStyle.FirstLineIndent;
            int maxX = textLayoutWidth - paragraphStyle.RightMargin - paragraphStyle.LeftMargin;

            // Loop over all runs in logical order to determine whether word wrap or
            // tab expansion is needed.  If not, then everything fits on one line and we are done.
            {
                int currentX = scriptLine->X;
                for (int scriptRunIndex = 0; scriptRunIndex < scriptRunCount; scriptRunIndex++)
                {
                    ScriptRun* scriptRun = scriptRuns + scriptRunIndex;
                    if (scriptRun->RequiresTabExpansion)
                        goto ComplexLoop;

                    currentX += scriptRun->ABC.TotalWidth;
                    if (currentX > maxX && paragraphStyle.WordWrap)
                        goto ComplexLoop;
                }

                // Apply pending changes to the script line now that we're done.
                scriptLine->ScriptRunCount = scriptRunCount;
                SetScriptLineMetricsFromScriptRuns(scriptLine, scriptParagraph, ref textLayoutHeight);
                return;
            }

            // Loop over all runs in logical order and pack as many of them as will fit
            // on each line.  Perform tab expansion and word wrapping as necessary.
            ComplexLoop:
            {
                Debug.Assert(scriptParagraph->CharCount != 0,
                    "Script paragraph should not have zero chars because that would have been handled by the simple case.");

                char* paragraphChars = scriptParagraph->Chars(GetCharZero());
                int currentX = scriptLine->X;
                int scriptRunIndex = 0;
                int breakCharIndex = 0;
                int breakScriptRunIndex = 0;
                while (scriptRunIndex < scriptRunCount)
                {
                    ScriptRun* scriptRun = scriptRuns + scriptRunIndex;

                    bool isFirstRunOnLine = scriptRunIndex == scriptLine->ScriptRunIndex;
                    int firstCharIndex = scriptRun->CharIndexInParagraph;
                    int lastCharIndex = firstCharIndex + scriptRun->CharCount;
                    if (isFirstRunOnLine)
                        firstCharIndex += scriptLine->TruncatedLeadingCharsCount;

                    bool rightToLeftRun = scriptRun->ScriptAnalysis.fRTL;
                    bool performTabExpansion = rightToLeftRun == rightToLeftLayout;

                    // Sum glyph widths in logical order.
                    int* glyphAdvanceWidths = scriptRun->GlyphAdvanceWidths(scriptParagraph);
                    int glyphIndex = scriptParagraph->CharLogicalClusters[firstCharIndex];
                    for (int charIndex = firstCharIndex; charIndex < lastCharIndex; charIndex++)
                    {
                        if (scriptParagraph->CharLogicalAttributes[charIndex].fSoftBreakOrfWhitespace)
                        {
                            breakCharIndex = charIndex;
                            breakScriptRunIndex = scriptRunIndex;
                        }

                        if (performTabExpansion && paragraphChars[charIndex] == '\t')
                        {
                            int tabbedX = paragraphStyle.TabStopRuler.AdvanceToNextTabStop(currentX);
                            int oldGlyphAdvanceWidth = glyphAdvanceWidths[glyphIndex];
                            int newGlyphAdvanceWidth = tabbedX - currentX;
                            glyphAdvanceWidths[glyphIndex] = newGlyphAdvanceWidth;
                            scriptRun->ABC.abcB += newGlyphAdvanceWidth - oldGlyphAdvanceWidth;
                            currentX = tabbedX;
                            glyphIndex += rightToLeftRun ? -1 : 1;
                        }
                        else
                        {
                            int nextCharIndex = charIndex + 1;
                            if (rightToLeftRun)
                            {
                                int nextGlyphIndex = nextCharIndex != lastCharIndex
                                    ? scriptParagraph->CharLogicalClusters[nextCharIndex]
                                    : -1;
                                while (glyphIndex > nextGlyphIndex)
                                    currentX += glyphAdvanceWidths[glyphIndex--];
                            }
                            else
                            {
                                int nextGlyphIndex = nextCharIndex != lastCharIndex
                                    ? scriptParagraph->CharLogicalClusters[nextCharIndex]
                                    : scriptRun->GlyphCount;
                                while (glyphIndex < nextGlyphIndex)
                                    currentX += glyphAdvanceWidths[glyphIndex++];
                            }
                        }

                        if (currentX > maxX && paragraphStyle.WordWrap)
                            goto BreakLine;
                    }

                    // No overflow.
                    scriptRunIndex++;
                    continue;

                BreakLine:
                    {
                        // Overflow.
                        ScriptRun* firstRunOnLine = scriptRuns + scriptLine->ScriptRunIndex;
                        int firstCharIndexOnLine = firstRunOnLine->CharIndexInParagraph + scriptLine->TruncatedLeadingCharsCount;

                        // Introduce a hard break at the current script run when we do not find a useful breakpoint.
                        if (breakCharIndex <= firstCharIndexOnLine)
                        {
                            breakScriptRunIndex = scriptRunIndex;
                            breakCharIndex = scriptRun->CharIndexInParagraph;
                            if (breakCharIndex <= firstCharIndexOnLine)
                                breakCharIndex = firstCharIndexOnLine + 1; // ensure at least one char gets shown
                        }

                        // Fix the breakScriptRun pointer if the breakCharIndex is past the end of the run.
                        // Also advance the index beyond all remaining whitespace on the line so it does not
                        // get pushed into the beginning of the next line.
                        ScriptRun* breakScriptRun = scriptRuns + breakScriptRunIndex;
                        for (; ; )
                        {
                            int breakCharOffset = breakCharIndex - breakScriptRun->CharIndexInParagraph;
                            if (breakCharOffset == breakScriptRun->CharCount)
                            {
                                breakScriptRun++;
                                breakScriptRunIndex++;
                            }

                            if (breakCharIndex == scriptParagraph->CharCount)
                                goto Finish; // paragraph is completely finished!

                            if (!scriptParagraph->CharLogicalAttributes[breakCharIndex].fWhiteSpace)
                                break;

                            breakCharIndex += 1;
                        }

                        // Finalize the script line properties.
                        {
                            int breakCharOffset = breakCharIndex - breakScriptRun->CharIndexInParagraph;
                            if (breakCharOffset == 0)
                            {
                                scriptLine->ScriptRunCount = breakScriptRunIndex - scriptLine->ScriptRunIndex;
                            }
                            else
                            {
                                scriptLine->ScriptRunCount = breakScriptRunIndex - scriptLine->ScriptRunIndex + 1;
                                scriptLine->TruncatedTrailingCharsCount = breakScriptRun->CharCount - breakCharOffset;
                            }
                            SetScriptLineMetricsFromScriptRuns(scriptLine, scriptParagraph, ref textLayoutHeight);

                            // Start a new script line.
                            scriptRunIndex = breakScriptRunIndex;
                            currentX = paragraphStyle.LeftMargin;

                            scriptLine = AddScriptLine(paragraphIndex, scriptRunIndex, textLayoutHeight);
                            scriptLine->X = currentX;
                            scriptLine->TruncatedLeadingCharsCount = breakCharOffset;
                        }
                    }
                }

            Finish:
                // Finish the last script line.
                scriptLine->ScriptRunCount = scriptRunCount - scriptLine->ScriptRunIndex;
                SetScriptLineMetricsFromScriptRuns(scriptLine, scriptParagraph, ref textLayoutHeight);
            }
        }

        private ScriptLine* AddScriptLine(int paragraphIndex, int scriptRunIndex, int y)
        {
            int scriptLineIndex = scriptLineBuffer.Count;
            scriptLineBuffer.GrowBy(1);
            ScriptLine* scriptLine = GetScriptLineZero() + scriptLineIndex;
            scriptLine->Initialize(paragraphIndex, scriptRunIndex, y);
            return scriptLine;
        }

        private static void SetScriptLineMetricsFromScriptRuns(ScriptLine* scriptLine, ScriptParagraph* scriptParagraph, ref int textLayoutHeight)
        {
            int ascent = MinimumScriptLineHeight;
            int descent = 0;
            ScriptRun* scriptRun = scriptParagraph->ScriptRuns + scriptLine->ScriptRunIndex;
            ScriptRun* endScriptRun = scriptRun + scriptLine->ScriptRunCount;
            for (; scriptRun != endScriptRun; scriptRun++)
            {
                int scriptRunDescentAndMargin = scriptRun->Descent + scriptRun->BottomMargin;
                int scriptRunAscentAndMargin = scriptRun->Ascent + scriptRun->TopMargin;

                if (scriptRunAscentAndMargin > ascent)
                    ascent = scriptRunAscentAndMargin;
                if (scriptRunDescentAndMargin > descent)
                    descent = scriptRunDescentAndMargin;
            }

            int height = ascent + descent;
            scriptLine->Height = height;
            scriptLine->Descent = descent;

            textLayoutHeight += height;
        }

        /// <summary>
        /// Gets a fully analyzed <see cref="ScriptParagraph"/> by paragraph index.
        /// </summary>
        private ScriptParagraph* GetScriptParagraph(HDCState hdcState, int paragraphIndex, bool rightToLeft)
        {
            ScriptParagraph* scriptParagraph;
            if (! scriptParagraphCache.TryGetScriptParagraph(paragraphIndex, out scriptParagraph))
            {
                Paragraph* paragraph = GetParagraphZero() + paragraphIndex;
                AnalyzeParagraph(hdcState, paragraph, scriptParagraph, rightToLeft);
            }

            return scriptParagraph;
        }

        /// <summary>
        /// Analyzes a <see cref="Paragraph" /> to populate a <see cref="ScriptParagraph"/>.
        /// </summary>
        private void AnalyzeParagraph(HDCState hdcState, Paragraph* paragraph, ScriptParagraph* scriptParagraph, bool rightToLeft)
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

            // Skip paragraphs with no runs (this is the sentinel at end of document)
            if (runCount == 0)
            {
                return;
            }

            // Handle paragraphs with exactly one empty run (simple newline)
            Debug.Assert(charCount != 0, "A paragraph with at least one run should always have at least one character.");

            char* charZero = GetCharZero();
            char* chars = charZero + charIndex;

            // Step 1. Itemize the script items within the paragraph.
            //         Each script item represents a chunk of text (such as a word)
            //         for Unicode rendering purposes.
            scriptParagraph->EnsureCharCapacity(charCount);

            SCRIPT_ITEM* tempScriptItems;
            int tempScriptItemCount;
            ScriptItemize(chars, charCount, rightToLeft, tempScriptItemBuffer, out tempScriptItems, out tempScriptItemCount);

            // Step 2. Compute logical attributes for characters in the paragraph for word-break purposes.
            ScriptBreak(scriptParagraph, chars, tempScriptItems, tempScriptItemCount);

            // Step 3. Split the Runs on SCRIPT_ITEM boundaries to produce ScriptRuns.
            SplitScriptRuns(scriptParagraph, runs, runCount, tempScriptItems, tempScriptItemCount);

            // Step 4. Shape and Place glyphs and Measure embedded objects one run at a time.
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

                    scriptRun->Height = scriptCache.Height;
                    scriptRun->Descent = scriptCache.Descent;
                    scriptRun->TopMargin = 0;
                    scriptRun->BottomMargin = 0;

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
                    scriptRun->Height = measurements.Size.Height;
                    scriptRun->Descent = measurements.Descent;
                    scriptRun->TopMargin = measurements.Margin.Top;
                    scriptRun->BottomMargin = measurements.Margin.Bottom;
                    scriptRun->ABC.abcA = measurements.Margin.Left;
                    scriptRun->ABC.abcB = measurements.Size.Width;
                    scriptRun->ABC.abcC = measurements.Margin.Right;

                    // Change the logical attributes of the character so that an object behaves like
                    // a word rather than whitespace when performing word wrap.
                    scriptParagraph->CharLogicalAttributes[scriptRun->CharIndexInParagraph].SetfSoftBreakfCharStopAndfWordStop();
                }
            }
        }

        private static void ScriptItemize(char* chars, int charCount, bool rightToLeft,
            UnmanagedBuffer<SCRIPT_ITEM> tempScriptItemBuffer, out SCRIPT_ITEM* scriptItems, out int scriptItemCount)
        {
            var scriptControl = new SCRIPT_CONTROL();
            var scriptState = new SCRIPT_STATE();
            if (rightToLeft)
            {
                // Start in a RTL context.
                scriptState.uBidiLevel = 1;
            }

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

        private static void SplitScriptRuns(ScriptParagraph* scriptParagraph, Run* runs, int runCount, SCRIPT_ITEM* scriptItems, int scriptItemCount)
        {
            // Reserve space for the largest possible number of script runs.
            int maxScriptRuns = runCount + scriptItemCount;
            scriptParagraph->EnsureScriptRunCapacity(maxScriptRuns);

            // Merge Runs and SCRIPT_ITEMs to create ScriptRuns.
            Run* run = runs;
            int runStartCharIndex = 0;
            Run* endRun = runs + runCount;
            SCRIPT_ITEM* item = scriptItems;
            SCRIPT_ITEM* endItem = scriptItems + scriptItemCount;
            ScriptRun* startScriptRun = scriptParagraph->ScriptRuns;
            ScriptRun* scriptRun = startScriptRun;

            int currentCharIndex = 0;
            for (; ; scriptRun++)
            {
                int runEndCharIndex = runStartCharIndex + run->CharCount;
                int itemEndCharIndex = (item + 1)->iCharPos;

                if (itemEndCharIndex < runEndCharIndex)
                {
                    scriptRun->Initialize(run, &item->a, currentCharIndex, itemEndCharIndex - currentCharIndex);
                    currentCharIndex = itemEndCharIndex;
                    item++;
                }
                else if (runEndCharIndex < itemEndCharIndex)
                {
                    scriptRun->Initialize(run, &item->a, currentCharIndex, runEndCharIndex - currentCharIndex);
                    runStartCharIndex = currentCharIndex = runEndCharIndex;
                    run++;
                }
                else
                {
                    scriptRun->Initialize(run, &item->a, currentCharIndex, runEndCharIndex - currentCharIndex);
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

        private int* GetTempVisualToLogicalMap(ScriptRun* scriptRuns, int count)
        {
            tempEmbeddingLevelBuffer.SetCapacity(count);
            byte* embeddingLevels = (byte*) tempEmbeddingLevelBuffer.GetPointer();
            for (int i = 0; i < count; i++)
                embeddingLevels[i] = (byte) scriptRuns[i].ScriptAnalysis.s.uBidiLevel;

            tempVisualToLogicalMapBuffer.SetCapacity(count);
            int* visualToLogicalMap = (int*)tempVisualToLogicalMapBuffer.GetPointer();

            int result = NativeMethods.ScriptLayout(count, embeddingLevels, visualToLogicalMap, null);
            if (result != NativeConstants.S_OK)
                Marshal.ThrowExceptionForHR(result);

            return visualToLogicalMap;
        }

        private static void ScriptShapeAndPlace(IntPtr hdc, ref IntPtr scriptCache, ScriptParagraph* scriptParagraph, ScriptRun* scriptRun, char* paragraphChars)
        {
            int charIndexInParagraph = scriptRun->CharIndexInParagraph;
            char* chars = paragraphChars + charIndexInParagraph;
            int charCount = scriptRun->CharCount;
            int glyphIndexInParagraph = scriptParagraph->GlyphCount;
            ushort* charLogicalClusters = scriptParagraph->CharLogicalClusters + charIndexInParagraph;

            SCRIPT_ANALYSIS* scriptAnalysis = &scriptRun->ScriptAnalysis;
            int glyphCapacity = charCount * 3 / 2 + 16; // * 1.5 + 16 per Uniscribe recommendations for ScriptShape.
            int glyphCount;
            ushort* glyphs;
            SCRIPT_VISATTR* glyphVisualAttributes;
            int result;
            for (; ; )
            {
                scriptParagraph->EnsureGlyphCapacityAndPreserveContents(glyphIndexInParagraph + glyphCapacity);

                glyphs = scriptParagraph->Glyphs + glyphIndexInParagraph;
                glyphVisualAttributes = scriptParagraph->GlyphVisualAttributes + glyphIndexInParagraph;
                result = NativeMethods.ScriptShape(hdc, ref scriptCache, chars, charCount, glyphCapacity,
                    scriptAnalysis, glyphs, charLogicalClusters, glyphVisualAttributes,
                    out glyphCount);

                if (result == NativeConstants.S_OK)
                    break;

                if (result == NativeConstants.USP_E_SCRIPT_NOT_IN_FONT)
                {
                    SCRIPT_ANALYSIS modifiedScriptAnalysis = *scriptAnalysis;
                    modifiedScriptAnalysis.eScript = NativeConstants.SCRIPT_UNDEFINED;
                    result = NativeMethods.ScriptShape(hdc, ref scriptCache, chars, charCount, glyphCapacity,
                        &modifiedScriptAnalysis, glyphs, charLogicalClusters, glyphVisualAttributes,
                        out glyphCount);
                    if (result == NativeConstants.S_OK)
                        break;
                }

                if (result != NativeConstants.E_OUTOFMEMORY)
                    Marshal.ThrowExceptionForHR(result);

                glyphCapacity *= 2;
            }

            int* glyphAdvanceWidths = scriptParagraph->GlyphAdvanceWidths + glyphIndexInParagraph;
            GOFFSET* glyphOffsets = scriptParagraph->GlyphOffsets + glyphIndexInParagraph;
            result = NativeMethods.ScriptPlace(hdc, ref scriptCache, glyphs, glyphCount,
                glyphVisualAttributes, scriptAnalysis, glyphAdvanceWidths, glyphOffsets, out scriptRun->ABC);
            if (result != NativeConstants.S_OK)
                Marshal.ThrowExceptionForHR(result);

            scriptRun->GlyphIndexInParagraph = glyphIndexInParagraph;
            scriptRun->GlyphCount = glyphCount;
            scriptParagraph->GlyphCount += glyphCount;
        }

        private static void ScriptTextOut(IntPtr hdc, ref IntPtr scriptCache, int x, int y,
            ExtTextOutOptions fuOptions, RECT* clipRect, SCRIPT_ANALYSIS* scriptAnalysis,
            ushort* glyphs, int glyphCount, int* glyphAdvanceWidths, int* justifiedGlyphAdvanceWidths,
            GOFFSET* glyphOffets)
        {
            int result = NativeMethods.ScriptTextOut(hdc, ref scriptCache, x, y, fuOptions, clipRect,
                scriptAnalysis, null, 0, glyphs, glyphCount, glyphAdvanceWidths, justifiedGlyphAdvanceWidths,
                glyphOffets);
            if (result != NativeConstants.S_OK)
                Marshal.ThrowExceptionForHR(result);
        }

        /*
        private static int GetFontHeight(IntPtr hdc, ref IntPtr scriptCache)
        {
            int height;
            int result = NativeMethods.ScriptCacheGetHeight(hdc, ref scriptCache, out height);
            if (result != NativeConstants.S_OK)
                Marshal.ThrowExceptionForHR(result);

            return height;
        }
         */

        private static EmbeddedObjectMeasurements SafeMeasure(EmbeddedObject obj, Style style)
        {
            try
            {
                return obj.Measure(style);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception while measuring an embedded object.\n{0}", ex));
                return new EmbeddedObjectMeasurements(new Size(0, 0));
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

                    if (! currentScriptCache.HaveMetrics)
                    {
                        TEXTMETRIC textMetric;
                        NativeMethods.GetTextMetrics(HDC, out textMetric);

                        currentScriptCache.Height = textMetric.tmHeight;
                        currentScriptCache.Descent = textMetric.tmDescent;
                        currentScriptCache.HaveMetrics = true;
                    }
                }

                return currentScriptCache;
            }

            public void SetBkMode(int bkMode)
            {
                NativeMethods.SetBkMode(HDC, bkMode);
            }

            public void SetPenColor(Color color)
            {
                NativeMethods.SelectObject(HDC, NativeMethods.GetStockObject(NativeConstants.DC_PEN));
                NativeMethods.SetDCPenColor(HDC, ToCOLORREF(color));
            }

            public void SetBrushColor(Color color)
            {
                NativeMethods.SelectObject(HDC, NativeMethods.GetStockObject(NativeConstants.DC_BRUSH));
                NativeMethods.SetDCBrushColor(HDC, ToCOLORREF(color));
            }

            public void SetTextColor(Color color)
            {
                NativeMethods.SetTextColor(HDC, ToCOLORREF(color));
            }

            private static int ToCOLORREF(Color color)
            {
                return (color.B << 16) | (color.G << 8) | color.R;
            }
        }
    }
}
