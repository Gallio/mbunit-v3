using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gallio.Common.Splash.Internal;
using Gallio.Common.Splash.Native;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the layout of a splash document.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Much of this implementation is based on information from the amazing Neatpad tutorial by James Brown.
    /// http://www.catch22.net/tuts/neatpad
    /// </para>
    /// </remarks>
    public unsafe class SplashLayout : IDisposable
    {
        private readonly SplashDocument document;
        private readonly Control parentControl;

        private readonly RecursionGuard recursionGuard;

        private readonly ScriptMetricsCache scriptMetricsCache;
        private readonly ScriptParagraphCache scriptParagraphCache;
        private readonly UnmanagedBuffer scriptLineBuffer;
        private readonly UnmanagedBuffer tempScriptItemBuffer;
        private readonly UnmanagedBuffer tempEmbeddingLevelBuffer;
        private readonly UnmanagedBuffer tempVisualToLogicalMapBuffer;
        private readonly UnmanagedBuffer tempLogicalClustersBuffer;

        private readonly Dictionary<int, EmbeddedObjectHost> embeddedObjectHosts;
        private readonly Queue<EmbeddedObjectHost> pendingEmbeddedObjectHostsToCreate;
        private readonly Queue<EmbeddedObjectHost> pendingEmbeddedObjectHostsToShow;

        private int pendingFirstParagraphToItemize;
        private int pendingFirstParagraphToLayout;

        private int desiredLayoutWidth;
        private bool desiredLayoutRightToLeft;

        private int currentLayoutWidth;
        private int currentLayoutHeight;
        private bool currentLayoutRightToLeft;

        private const int ScriptParagraphCacheSize = 64;

        private const int MinimumScriptLineHeight = 1;
        private const int InitialCapacityForLinesPerDocument = 64;
        private const int InitialCapacityForScriptRunsPerParagraph = 8;
        private const int InitialCapacityForCharsPerScriptRun = 128;

        /// <summary>
        /// Creates a layout for a document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="parentControl">The parent control to which embedded objects can
        /// attach themselves.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="document"/>
        /// or <paramref name="parentControl"/> is null.</exception>
        public SplashLayout(SplashDocument document, Control parentControl)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (parentControl == null)
                throw new ArgumentNullException("parentControl");

            this.document = document;
            this.parentControl = parentControl;

            recursionGuard = new RecursionGuard();

            scriptMetricsCache = new ScriptMetricsCache();
            scriptLineBuffer = new UnmanagedBuffer(InitialCapacityForLinesPerDocument, sizeof(ScriptLine));

            tempScriptItemBuffer = new UnmanagedBuffer(InitialCapacityForScriptRunsPerParagraph, sizeof(SCRIPT_ITEM));
            tempEmbeddingLevelBuffer = new UnmanagedBuffer(InitialCapacityForScriptRunsPerParagraph, sizeof(byte));
            tempVisualToLogicalMapBuffer = new UnmanagedBuffer(InitialCapacityForScriptRunsPerParagraph, sizeof(int));
            tempLogicalClustersBuffer = new UnmanagedBuffer(InitialCapacityForCharsPerScriptRun, sizeof(short));

            scriptParagraphCache = new ScriptParagraphCache(ScriptParagraphCacheSize);

            embeddedObjectHosts = new Dictionary<int, EmbeddedObjectHost>();
            pendingEmbeddedObjectHostsToCreate = new Queue<EmbeddedObjectHost>();
            pendingEmbeddedObjectHostsToShow = new Queue<EmbeddedObjectHost>();

            desiredLayoutWidth = 400; // arbitrary
            desiredLayoutRightToLeft = false; // arbitrary

            AttachDocumentEvents();
            Reset();
        }

        /// <summary>
        /// Disposes the layout.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the layout.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()"/> was called explicitly.</param>
        protected virtual void Dispose(bool disposing)
        {
            Reset();
        }

        private void AttachDocumentEvents()
        {
            document.DocumentCleared += (sender, e) => Reset();
            document.ParagraphChanged += (sender, e) => InvalidateParagraphItemization(e.ParagraphIndex);
        }

        /// <summary>
        /// Event raised when a layout property changes and requires an update.
        /// </summary>
        public event EventHandler UpdateRequired;

        /// <summary>
        /// Gets the document this layout is for.
        /// </summary>
        public SplashDocument Document
        {
            get { return document; }
        }

        /// <summary>
        /// Gets the parent control to which embedded objects can attach themselves.
        /// </summary>
        public Control ParentControl
        {
            get { return parentControl; }
        }

        /// <summary>
        /// Gets or sets the desired layout width.
        /// </summary>
        public int DesiredLayoutWidth
        {
            get { return desiredLayoutWidth; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "Value must be at least 1.");

                recursionGuard.Do(() =>
                {
                    if (desiredLayoutWidth != value)
                    {
                        desiredLayoutWidth = value;
                        InvalidateParagraphLayout(0);
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets the desired reading order for the layout.
        /// </summary>
        public bool DesiredLayoutRightToLeft
        {
            get { return desiredLayoutRightToLeft; }
            set
            {
                recursionGuard.Do(() =>
                {
                    if (desiredLayoutRightToLeft != value)
                    {
                        desiredLayoutRightToLeft = value;
                        InvalidateParagraphItemization(0);
                    }
                });
            }
        }

        /// <summary>
        /// Gets or sets the current layout width which may differ from the desired layout width
        /// if a relayout is pending.
        /// </summary>
        public int CurrentLayoutWidth
        {
            get { return currentLayoutWidth; }
        }

        /// <summary>
        /// Gets or sets the current layout height which may differ from the expected layout height
        /// if a relayout is pending.
        /// </summary>
        public int CurrentLayoutHeight
        {
            get { return currentLayoutHeight; }
        }

        /// <summary>
        /// Gets or sets the current reading order which may differ from the desired reading order
        /// if a relayout is pending.
        /// </summary>
        public bool CurrentLayoutRightToLeft
        {
            get { return currentLayoutRightToLeft; }
        }

        /// <summary>
        /// Resets all layout information and frees internal buffers.
        /// </summary>
        public void Reset()
        {
            recursionGuard.Do(() =>
            {
                DisposeEmbeddedObjectHosts();

                scriptMetricsCache.Clear();
                scriptParagraphCache.Clear();

                scriptLineBuffer.Clear();

                tempScriptItemBuffer.Clear();
                tempEmbeddingLevelBuffer.Clear();
                tempVisualToLogicalMapBuffer.Clear();
                tempLogicalClustersBuffer.Clear();

                pendingFirstParagraphToItemize = -1;
                pendingFirstParagraphToLayout = -1;

                InvalidateParagraphItemization(0);
            });
        }

        private void DisposeEmbeddedObjectHosts()
        {
            foreach (EmbeddedObjectHost host in embeddedObjectHosts.Values)
                host.Dispose();

            embeddedObjectHosts.Clear();
            pendingEmbeddedObjectHostsToCreate.Clear();
            pendingEmbeddedObjectHostsToShow.Clear();
        }

        /// <summary>
        /// Updates the layout.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Call this method some time after UpdateRequired is raised or whenever the layout origin changes.
        /// </para>
        /// </remarks>
        /// <param name="layoutOrigin">The origin of the document layout area in the parent control.</param>
        public void Update(Point layoutOrigin)
        {
            // Update itemized paragraphs and cached document structure.
            recursionGuard.Do(() => UpdateParagraphItemization());

            // Create embedded object hosts.  This might cause recursion into Paint if the
            // embedded objects are controls.
            while (pendingEmbeddedObjectHostsToCreate.Count != 0)
            {
                EmbeddedObjectHost embeddedObjectHost = pendingEmbeddedObjectHostsToCreate.Dequeue();
                embeddedObjectHost.CreateClient();
            }

            // Update the layout.
            recursionGuard.Do(() => UpdateParagraphLayout());

            // Set bounds of embedded objects.  This might cause recursion into Paint if
            // the embedded objects are controls.
            while (pendingEmbeddedObjectHostsToShow.Count != 0)
            {
                EmbeddedObjectHost embeddedObjectHost = pendingEmbeddedObjectHostsToShow.Dequeue();

                Rectangle bounds = GetDrawingBoundsOfEmbeddedObject(embeddedObjectHost, layoutOrigin);
                embeddedObjectHost.Show(bounds, currentLayoutRightToLeft);
            }
        }

        private Rectangle GetDrawingBoundsOfEmbeddedObject(EmbeddedObjectHost embeddedObjectHost, Point layoutOrigin)
        {
            Rectangle embeddedObjectHostLayoutBounds = embeddedObjectHost.LayoutBounds;
            int x = currentLayoutRightToLeft
                ? currentLayoutWidth - embeddedObjectHostLayoutBounds.Right + layoutOrigin.X
                : embeddedObjectHostLayoutBounds.X + layoutOrigin.X;
            int y = embeddedObjectHostLayoutBounds.Y + layoutOrigin.Y;

            return new Rectangle(x, y, embeddedObjectHostLayoutBounds.Width, embeddedObjectHostLayoutBounds.Height);
        }

        /// <summary>
        /// Gets the snap position that corresponds to a point in the control.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <param name="layoutOrigin">The origin of the document layout area in the parent control.</param>
        /// <returns>The character snap.</returns>
        public SnapPosition GetSnapPositionAtPoint(Point point, Point layoutOrigin)
        {
            return recursionGuard.Do(() =>
            {
                int xPosition = currentLayoutRightToLeft
                    ? currentLayoutWidth - point.X + layoutOrigin.X
                    : point.X - layoutOrigin.X;
                int yPosition = point.Y - layoutOrigin.Y;

                if (yPosition < 0)
                    return new SnapPosition(SnapKind.Leading, 0);

                int scriptLineIndex;
                ScriptLine* scriptLine = GetScriptLineAtYPositionOrNullIfNone(yPosition, out scriptLineIndex);
                if (scriptLine == null || scriptLine->ScriptRunCount == 0)
                {
                    int charCount = document.CharCount;
                    if (charCount == 0)
                        return new SnapPosition(SnapKind.Leading, 0);

                    return new SnapPosition(SnapKind.Trailing, charCount - 1);
                }

                int scriptRunCount = scriptLine->ScriptRunCount;
                using (DeviceContext deviceContext = DeviceContext.CreateFromScreen(scriptMetricsCache))
                {
                    int xCurrentPosition = scriptLine->X;
                    ScriptParagraph* scriptParagraph = GetScriptParagraph(deviceContext, scriptLine->ParagraphIndex);
                    int scriptRunIndex = scriptLine->ScriptRunIndex;
                    int* visualToLogicalMap = GetTempVisualToLogicalMap(
                        scriptParagraph->ScriptRuns + scriptRunIndex,
                        scriptRunCount);

                    if (xCurrentPosition > xPosition)
                    {
                        ScriptRun* scriptRun = scriptParagraph->ScriptRuns + scriptLine->ScriptRunIndex;
                        return new SnapPosition(SnapKind.Leading, scriptParagraph->CharIndex
                            + scriptRun->CharIndexInParagraph + scriptLine->TruncatedLeadingCharsCount);
                    }

                    for (int i = 0; i < scriptRunCount; i++)
                    {
                        int logicalScriptRunIndex =
                            visualToLogicalMap[currentLayoutRightToLeft ? scriptRunCount - i - 1 : i];
                        ScriptRun* scriptRun = scriptParagraph->ScriptRuns + logicalScriptRunIndex + scriptRunIndex;

                        int measuredWidth;
                        switch (scriptRun->RunKind)
                        {
                            case RunKind.Text:
                            {
                                int glyphIndexInParagraph, glyphCount;
                                int truncatedLeadingCharsCount = logicalScriptRunIndex == 0
                                    ? scriptLine->TruncatedLeadingCharsCount
                                    : 0;
                                int truncatedTrailingCharsCount = logicalScriptRunIndex == scriptRunCount - 1
                                    ? scriptLine->TruncatedTrailingCharsCount
                                    : 0;
                                measuredWidth = MeasureTextScriptRun(scriptParagraph, scriptRun,
                                    truncatedLeadingCharsCount, truncatedTrailingCharsCount,
                                    out glyphIndexInParagraph, out glyphCount);

                                if (xCurrentPosition + measuredWidth > xPosition)
                                {
                                    int charCount = scriptRun->CharCount - truncatedLeadingCharsCount -
                                        truncatedTrailingCharsCount;
                                    ushort* charLogicalClusters = scriptParagraph->CharLogicalClusters +
                                        scriptRun->CharIndexInParagraph + truncatedLeadingCharsCount;

                                    int glyphShift = glyphIndexInParagraph - scriptRun->GlyphIndexInParagraph;
                                    if (glyphShift != 0)
                                    {
                                        tempLogicalClustersBuffer.EnsureCapacity(charCount);
                                        ushort* shiftedCharLogicalClusters =
                                            (ushort*) tempLogicalClustersBuffer.GetPointer();

                                        for (int j = 0; j < charCount; j++)
                                            shiftedCharLogicalClusters[j] =
                                                (ushort) (charLogicalClusters[j] - glyphShift);

                                        charLogicalClusters = shiftedCharLogicalClusters;
                                    }

                                    int charIndexInTruncatedRun, trailingCodePoints;
                                    int x = currentLayoutRightToLeft
                                        ? measuredWidth - xPosition + xCurrentPosition
                                        : xPosition - xCurrentPosition;
                                    int result = NativeMethods.ScriptXtoCP(x,
                                        charCount, glyphCount, charLogicalClusters,
                                        scriptParagraph->GlyphVisualAttributes + glyphIndexInParagraph,
                                        scriptParagraph->GlyphAdvanceWidths + glyphIndexInParagraph,
                                        &scriptRun->ScriptAnalysis,
                                        out charIndexInTruncatedRun,
                                        out trailingCodePoints);

                                    if (result == 0)
                                    {
                                        return new SnapPosition(SnapKind.Exact, scriptParagraph->CharIndex
                                            + scriptRun->CharIndexInParagraph + charIndexInTruncatedRun +
                                                truncatedLeadingCharsCount);
                                    }
                                }
                                break;
                            }

                            case RunKind.Object:
                            {
                                measuredWidth = MeasureObjectScriptRun(scriptRun);

                                if (xCurrentPosition + measuredWidth > xPosition)
                                {
                                    return new SnapPosition(SnapKind.Exact,
                                        scriptParagraph->CharIndex + scriptRun->CharIndexInParagraph);
                                }
                                break;
                            }

                            case RunKind.Tab:
                            {
                                Style paragraphStyle = GetParagraphStyleAssumingItHasAtLeastOneRun(scriptParagraph);
                                measuredWidth = MeasureTabScriptRun(scriptRun, paragraphStyle, xCurrentPosition, false);

                                if (xCurrentPosition + measuredWidth > xPosition)
                                {
                                    return new SnapPosition(SnapKind.Exact,
                                        scriptParagraph->CharIndex + scriptRun->CharIndexInParagraph);
                                }
                                break;
                            }

                            default:
                                throw new NotSupportedException();
                        }

                        xCurrentPosition += measuredWidth;
                    }

                    {
                        ScriptRun* scriptRun = scriptParagraph->ScriptRuns + scriptLine->ScriptRunIndex +
                            scriptRunCount - 1;
                        return new SnapPosition(SnapKind.Trailing, scriptParagraph->CharIndex
                            + scriptRun->CharIndexInParagraph + scriptRun->CharCount
                                - 1 - scriptLine->TruncatedTrailingCharsCount);
                    }
                }
            });
        }

        /// <summary>
        /// Paints a region of text.
        /// </summary>
        /// <remarks>
        /// Be sure to update the layout before calling this method.
        /// </remarks>
        /// <param name="graphics">The graphics context.</param>
        /// <param name="layoutOrigin">The origin of the document layout area in the parent control.</param>
        /// <param name="clipRect">The clip rectangle of the region to paint in the device context.</param>
        /// <param name="paintOptions">The paint options.</param>
        /// <param name="selectedCharIndex">The index of the first selected character.  Ignored if
        /// the number of selected characters is 0.</param>
        /// <param name="selectedCharCount">The number of selected characters or 0 if none.
        /// May be negative if the selection is reversed.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphics"/> or
        /// <paramref name="paintOptions"/> is null.</exception>
        public void Paint(Graphics graphics, Point layoutOrigin, Rectangle clipRect, PaintOptions paintOptions,
            int selectedCharIndex, int selectedCharCount)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (paintOptions == null)
                throw new ArgumentNullException("paintOptions");

            recursionGuard.Do(() =>
            {
                Rectangle layoutRect = new Rectangle(layoutOrigin.X, layoutOrigin.Y, currentLayoutWidth,
                    currentLayoutHeight);
                Rectangle clippedLayoutRect = layoutRect;
                clippedLayoutRect.Intersect(clipRect);

                GraphicsState state = graphics.Save();
                try
                {
                    using (DeviceContext deviceContext = DeviceContext.CreateFromGraphics(graphics, scriptMetricsCache))
                    {
                        IntPtr clipRegion = DeviceContext.CreateRectRegion(clippedLayoutRect);
                        try
                        {
                            PaintRegion(deviceContext, layoutRect, clippedLayoutRect, clipRegion,
                                paintOptions, selectedCharIndex, selectedCharCount);
                        }
                        finally
                        {
                            DeviceContext.DeleteObject(clipRegion);
                        }
                    }
                }
                finally
                {
                    graphics.Restore(state);
                }
            });
        }

        private void PaintRegion(DeviceContext deviceContext, Rectangle layoutRect, Rectangle clipRect,
            IntPtr clipRegion, PaintOptions paintOptions, int selectedCharIndex, int selectedCharCount)
        {
            int yStartPosition = clipRect.Top - layoutRect.Top;
            int yEndPosition = clipRect.Bottom - layoutRect.Top;
            int firstScriptLineIndex;
            ScriptLine* scriptLine = GetScriptLineAtYPositionOrNullIfNone(yStartPosition, out firstScriptLineIndex);
            if (scriptLine == null)
                return;

            ScriptLine* endScriptLine = GetScriptLineZero() + scriptLineBuffer.Count;

            int xStartPosition, xEndPosition;
            if (currentLayoutRightToLeft)
            {
                xStartPosition = layoutRect.Right - clipRect.Right;
                xEndPosition = layoutRect.Right - clipRect.Left;
            }
            else
            {
                xStartPosition = clipRect.Left - layoutRect.Left;
                xEndPosition = clipRect.Right - layoutRect.Left;
            }

            // Note: Selection may extend beyond the range of the layout if improper index and count
            //       values were provided.  This is ok since we only care about the intersection of the
            //       selected range with actual range of characters on the lines.
            if (selectedCharCount < 0)
            {
                selectedCharIndex += selectedCharCount;
                selectedCharCount = -selectedCharCount;
            }

            deviceContext.SetBkMode(NativeConstants.TRANSPARENT);

            for (; scriptLine != endScriptLine && scriptLine->Y < yEndPosition; scriptLine++)
            {
                int scriptRunCount = scriptLine->ScriptRunCount;
                if (scriptRunCount != 0)
                {
                    ScriptParagraph* scriptParagraph = GetScriptParagraph(deviceContext, scriptLine->ParagraphIndex);
                    int scriptRunIndex = scriptLine->ScriptRunIndex;
                    int* visualToLogicalMap = GetTempVisualToLogicalMap(scriptParagraph->ScriptRuns + scriptRunIndex,
                        scriptRunCount);

                    if (selectedCharCount == 0
                        || selectedCharIndex >= scriptParagraph->CharIndex
                                + scriptParagraph->ScriptRuns[scriptRunIndex + scriptRunCount - 1].EndCharIndexInParagraph
                                - scriptLine->TruncatedTrailingCharsCount
                        || selectedCharIndex + selectedCharCount < scriptParagraph->CharIndex
                                + scriptParagraph->ScriptRuns[scriptRunIndex].CharIndexInParagraph
                                + scriptLine->TruncatedLeadingCharsCount)
                    {
                        deviceContext.SelectClipRegion(clipRegion);

                        // No selection.  Just paint the line as usual.
                        PaintLineForeground(deviceContext, paintOptions,
                            scriptLine, scriptParagraph, scriptRunIndex, scriptRunCount, visualToLogicalMap,
                            xStartPosition, xEndPosition,
                            layoutRect, false, false);
                    }
                    else
                    {
                        deviceContext.SelectClipRegion(clipRegion);

                        // Paint background.
                        PaintLineBackgroundAndExcludeSelectedTextFromClipRegion(deviceContext, paintOptions,
                            scriptLine, scriptParagraph, scriptRunIndex, scriptRunCount, visualToLogicalMap,
                            xStartPosition, xEndPosition,
                            layoutRect, selectedCharIndex, selectedCharCount);

                        // Paint all text and objects except in selected areas.
                        PaintLineForeground(deviceContext, paintOptions,
                            scriptLine, scriptParagraph, scriptRunIndex, scriptRunCount, visualToLogicalMap,
                            xStartPosition, xEndPosition,
                            layoutRect, false, false);

                        // Invert the clip region.
                        IntPtr lineRegion = DeviceContext.CreateRectRegion(clipRect);
                        deviceContext.XorClipRegion(lineRegion);
                        DeviceContext.DeleteObject(lineRegion);

                        // Paint all text in selected areas.
                        PaintLineForeground(deviceContext, paintOptions,
                            scriptLine, scriptParagraph, scriptRunIndex, scriptRunCount, visualToLogicalMap,
                            xStartPosition, xEndPosition,
                            layoutRect, true, true);
                    }
                }
            }
        }

        private void PaintLineBackgroundAndExcludeSelectedTextFromClipRegion(DeviceContext deviceContext, PaintOptions paintOptions,
            ScriptLine* scriptLine, ScriptParagraph* scriptParagraph,
            int scriptRunIndex, int scriptRunCount, int* visualToLogicalMap,
            int xStartPosition, int xEndPosition,
            Rectangle layoutRect, int selectedCharIndex, int selectedCharCount)
        {
            IntPtr brush = DeviceContext.GetStockObject(NativeConstants.DC_BRUSH);
            deviceContext.SetDCBrushColor(paintOptions.SelectedBackgroundColor);

            int yCurrentBaseline = scriptLine->Y + scriptLine->Ascent;
            int xCurrentPosition = scriptLine->X;
            for (int i = 0; i < scriptRunCount; i++)
            {
                int logicalScriptRunIndex = visualToLogicalMap[currentLayoutRightToLeft ? scriptRunCount - i - 1 : i];
                ScriptRun* scriptRun = scriptParagraph->ScriptRuns + logicalScriptRunIndex + scriptRunIndex;

                int measuredWidth;
                switch (scriptRun->RunKind)
                {
                    case RunKind.Text:
                    {
                        int glyphIndexInParagraph, glyphCount;
                        int truncatedLeadingCharsCount = logicalScriptRunIndex == 0
                            ? scriptLine->TruncatedLeadingCharsCount
                            : 0;
                        int truncatedTrailingCharsCount = logicalScriptRunIndex == scriptRunCount - 1
                            ? scriptLine->TruncatedTrailingCharsCount
                            : 0;
                        measuredWidth = MeasureTextScriptRun(scriptParagraph, scriptRun,
                            truncatedLeadingCharsCount, truncatedTrailingCharsCount,
                            out glyphIndexInParagraph, out glyphCount);

                        if (xCurrentPosition + measuredWidth > xStartPosition)
                        {
                            int scriptRunCharIndex = scriptParagraph->CharIndex + scriptRun->CharIndexInParagraph;
                            int leadingCharIndex = scriptRunCharIndex + truncatedLeadingCharsCount;
                            int trailingCharIndex = scriptRunCharIndex + scriptRun->CharCount -
                                truncatedTrailingCharsCount;
                            if (trailingCharIndex >= selectedCharIndex &&
                                leadingCharIndex <= selectedCharIndex + selectedCharCount)
                            {
                                int relativePositionOfSelection;
                                if (leadingCharIndex >= selectedCharIndex)
                                {
                                    relativePositionOfSelection = 0;
                                }
                                else
                                {
                                    relativePositionOfSelection = MeasureTextScriptRun(scriptParagraph, scriptRun,
                                        truncatedLeadingCharsCount,
                                        truncatedTrailingCharsCount + trailingCharIndex - selectedCharIndex,
                                        out glyphIndexInParagraph, out glyphCount);
                                }

                                int measuredWidthOfSelection;
                                if (trailingCharIndex <= selectedCharIndex + selectedCharCount)
                                {
                                    measuredWidthOfSelection = measuredWidth - relativePositionOfSelection;
                                }
                                else
                                {
                                    measuredWidthOfSelection = MeasureTextScriptRun(scriptParagraph, scriptRun,
                                        truncatedLeadingCharsCount + Math.Max(selectedCharIndex - leadingCharIndex, 0),
                                        truncatedTrailingCharsCount + trailingCharIndex - selectedCharIndex -
                                            selectedCharCount,
                                        out glyphIndexInParagraph, out glyphCount);
                                }

                                int x = currentLayoutRightToLeft
                                    ? layoutRect.Right - xCurrentPosition - measuredWidth
                                    : layoutRect.Left + xCurrentPosition;
                                int y = layoutRect.Top + yCurrentBaseline - scriptRun->Ascent;

                                if (scriptRun->ScriptAnalysis.fRTL)
                                    x += measuredWidth - relativePositionOfSelection - measuredWidthOfSelection;
                                else
                                    x += relativePositionOfSelection;

                                Rectangle selectedRect = new Rectangle(x, y, measuredWidthOfSelection, scriptRun->Height);
                                deviceContext.FillRect(selectedRect, brush);
                                deviceContext.ExcludeClipRect(selectedRect);
                            }
                        }
                        break;
                    }

                    case RunKind.Object:
                    case RunKind.Tab:
                    {
                        measuredWidth = scriptRun->RunKind == RunKind.Object
                            ? MeasureObjectScriptRun(scriptRun)
                            : MeasureTabScriptRun(scriptRun,
                                GetParagraphStyleAssumingItHasAtLeastOneRun(scriptParagraph),
                                xCurrentPosition, false);

                        if (xCurrentPosition + measuredWidth > xStartPosition)
                        {
                            int leadingCharIndex = scriptParagraph->CharIndex + scriptRun->CharIndexInParagraph;
                            int trailingCharIndex = leadingCharIndex + 1;
                            if (trailingCharIndex >= selectedCharIndex &&
                                leadingCharIndex <= selectedCharIndex + selectedCharCount)
                            {
                                int x = currentLayoutRightToLeft
                                    ? layoutRect.Right - xCurrentPosition - measuredWidth
                                    : layoutRect.Left + xCurrentPosition;
                                int y = layoutRect.Top + yCurrentBaseline - scriptRun->Ascent;

                                Rectangle selectedRect = new Rectangle(x, y, measuredWidth, scriptRun->Height);
                                deviceContext.FillRect(selectedRect, brush);
                                // Don't exclude clip rect for objects and tabs.
                                //deviceContext.ExcludeClipRect(selectedRect);
                            }
                        }
                        break;
                    }

                    default:
                        throw new NotSupportedException();
                }

                xCurrentPosition += measuredWidth;

                if (xCurrentPosition >= xEndPosition)
                    break; // can't fit any more runs within the clip rectangle
            }
        }

        private void PaintLineForeground(DeviceContext deviceContext, PaintOptions paintOptions,
            ScriptLine* scriptLine, ScriptParagraph* scriptParagraph,
            int scriptRunIndex, int scriptRunCount, int* visualToLogicalMap,
            int xStartPosition, int xEndPosition,
            Rectangle layoutRect, bool isSelected, bool skipObjects)
        {
            int yCurrentBaseline = scriptLine->Y + scriptLine->Ascent;
            int xCurrentPosition = scriptLine->X;
            for (int i = 0; i < scriptRunCount; i++)
            {
                int logicalScriptRunIndex = visualToLogicalMap[currentLayoutRightToLeft ? scriptRunCount - i - 1 : i];
                ScriptRun* scriptRun = scriptParagraph->ScriptRuns + logicalScriptRunIndex + scriptRunIndex;

                int measuredWidth;
                switch (scriptRun->RunKind)
                {
                    case RunKind.Text:
                    {
                        Style style = document.LookupStyle(scriptRun->StyleIndex);
                        ScriptMetrics scriptMetrics = deviceContext.SelectFont(style.Font);

                        int glyphIndexInParagraph, glyphCount;
                        measuredWidth = MeasureTextScriptRun(scriptParagraph, scriptRun,
                            logicalScriptRunIndex == 0 ? scriptLine->TruncatedLeadingCharsCount : 0,
                            logicalScriptRunIndex == scriptRunCount - 1 ? scriptLine->TruncatedTrailingCharsCount : 0,
                            out glyphIndexInParagraph, out glyphCount);

                        if (glyphCount > 0 && xCurrentPosition + measuredWidth > xStartPosition)
                        {
                            int x = currentLayoutRightToLeft
                                ? layoutRect.Right - xCurrentPosition - measuredWidth
                                : layoutRect.Left + xCurrentPosition;
                            int y = layoutRect.Top + yCurrentBaseline - scriptRun->Ascent;

                            deviceContext.SetTextColor(isSelected ? paintOptions.SelectedTextColor : style.Color);
                            ScriptTextOut(deviceContext.HDC, ref scriptMetrics.ScriptCache, x, y,
                                ExtTextOutOptions.NONE, null, &scriptRun->ScriptAnalysis,
                                scriptParagraph->Glyphs + glyphIndexInParagraph,
                                glyphCount,
                                scriptParagraph->GlyphAdvanceWidths + glyphIndexInParagraph,
                                null,
                                scriptParagraph->GlyphOffsets + glyphIndexInParagraph);
                        }
                        break;
                    }

                    case RunKind.Object:
                    {
                        measuredWidth = MeasureObjectScriptRun(scriptRun);

                        if (!skipObjects && xCurrentPosition + measuredWidth > xStartPosition)
                        {
                            int embeddedObjectCharIndex = scriptRun->CharIndexInParagraph + scriptParagraph->CharIndex;
                            EmbeddedObjectHost embeddedObjectHost;
                            if (embeddedObjectHosts.TryGetValue(embeddedObjectCharIndex, out embeddedObjectHost))
                            {
                                if (embeddedObjectHost.RequiresPaint)
                                {
                                    using (Graphics g = Graphics.FromHdc(deviceContext.HDC))
                                    {
                                        Rectangle bounds = GetDrawingBoundsOfEmbeddedObject(embeddedObjectHost, layoutRect.Location);
                                        embeddedObjectHost.Paint(g, paintOptions, bounds, currentLayoutRightToLeft);
                                    }
                                }
                            }
                        }
                        break;
                    }

                    case RunKind.Tab:
                    {
                        measuredWidth = MeasureTabScriptRun(scriptRun,
                            GetParagraphStyleAssumingItHasAtLeastOneRun(scriptParagraph),
                            xCurrentPosition, false);
                        break;
                    }

                    default:
                        throw new NotSupportedException();
                }

                xCurrentPosition += measuredWidth;

                if (xCurrentPosition >= xEndPosition)
                    break; // can't fit any more runs within the clip rectangle
            }
        }

        private static int MeasureTextScriptRun(ScriptParagraph* scriptParagraph, ScriptRun* scriptRun,
            int truncatedLeadingCharsCount, int truncatedTrailingCharsCount,
            out int glyphIndexInParagraph, out int glyphCount)
        {
            if (truncatedLeadingCharsCount != 0 || truncatedTrailingCharsCount != 0)
            {
                ushort* logicalClusters = scriptRun->CharLogicalClusters(scriptParagraph);
                int startGlyphIndexInRun, endGlyphIndexInRun;
                if (scriptRun->ScriptAnalysis.fRTL)
                {
                    startGlyphIndexInRun = truncatedTrailingCharsCount != 0
                        ? logicalClusters[scriptRun->CharCount - truncatedTrailingCharsCount] + 1
                        : 0;
                    endGlyphIndexInRun = truncatedLeadingCharsCount != 0
                        ? logicalClusters[truncatedLeadingCharsCount] + 1
                        : scriptRun->GlyphCount;
                }
                else
                {
                    startGlyphIndexInRun = truncatedLeadingCharsCount != 0
                        ? logicalClusters[truncatedLeadingCharsCount]
                        : 0;
                    endGlyphIndexInRun = truncatedTrailingCharsCount != 0
                        ? logicalClusters[scriptRun->CharCount - truncatedTrailingCharsCount]
                        : scriptRun->GlyphCount;
                }

                glyphIndexInParagraph = scriptRun->GlyphIndexInParagraph + startGlyphIndexInRun;
                glyphCount = endGlyphIndexInRun - startGlyphIndexInRun;

                int* glyphAdvanceWidths = scriptParagraph->GlyphAdvanceWidths + glyphIndexInParagraph;
                int measuredWidth = 0;
                for (int j = 0; j < glyphCount; j++)
                    measuredWidth += glyphAdvanceWidths[j];
                return measuredWidth;
            }

            glyphIndexInParagraph = scriptRun->GlyphIndexInParagraph;
            glyphCount = scriptRun->GlyphCount;
            return scriptRun->ABC.TotalWidth;
        }

        private static int MeasureObjectScriptRun(ScriptRun* scriptRun)
        {
            return scriptRun->ABC.TotalWidth;
        }

        private static int MeasureTabScriptRun(ScriptRun* scriptRun, Style paragraphStyle, int x, bool invalidate)
        {
            int advanceWidth = scriptRun->ABC.abcB;
            if (advanceWidth < 0 || invalidate)
            {
                int nextX = paragraphStyle.TabStopRuler.AdvanceToNextTabStop(x);
                advanceWidth = scriptRun->ABC.abcB = nextX - x;
            }
            return advanceWidth;
        }

        private Style GetParagraphStyleAssumingItHasAtLeastOneRun(ScriptParagraph* scriptParagraph)
        {
            return document.LookupStyle(scriptParagraph->ScriptRuns[0].StyleIndex);
        }

        private ScriptLine* GetScriptLineZero()
        {
            return (ScriptLine*)scriptLineBuffer.GetPointer();
        }

        private void InvalidateParagraphItemization(int firstParagraphToItemize)
        {
            if (pendingFirstParagraphToItemize == -1
                || firstParagraphToItemize < pendingFirstParagraphToItemize)
            {
                pendingFirstParagraphToItemize = firstParagraphToItemize;

                InvalidateParagraphLayout(firstParagraphToItemize);
            }
        }

        private void InvalidateParagraphLayout(int firstParagraphToLayout)
        {
            if (pendingFirstParagraphToLayout == -1
                || firstParagraphToLayout < pendingFirstParagraphToLayout)
            {
                pendingFirstParagraphToLayout = firstParagraphToLayout;

                RaiseUpdateRequired();
            }
        }

        private void UpdateParagraphItemization()
        {
            if (pendingFirstParagraphToItemize >= 0)
            {
                int firstParagraphToItemize = pendingFirstParagraphToItemize;
                pendingFirstParagraphToItemize = -1;

                UpdateParagraphItemization(firstParagraphToItemize);
            }
        }

        private void UpdateParagraphItemization(int firstParagraphToItemize)
        {
            currentLayoutRightToLeft = desiredLayoutRightToLeft;

            // Remove cached script paragraphs.  They will be reitemized on demand.
            scriptParagraphCache.RemoveScriptParagraphsStartingFrom(firstParagraphToItemize);

            // Create embedded object hosts.
            Paragraph* paragraphZero = document.GetParagraphZero();
            Paragraph* startParagraph = paragraphZero + firstParagraphToItemize;
            Paragraph* endParagraph = paragraphZero + document.ParagraphCount;
            Run* runZero = document.GetRunZero();
            for (Paragraph* paragraph = startParagraph; paragraph != endParagraph; paragraph++)
            {
                Run* startRun = runZero + paragraph->RunIndex;
                Run* endRun = startRun + paragraph->RunCount;
                int charIndex = paragraph->CharIndex;
                for (Run* run = startRun; run != endRun; run++)
                {
                    switch (run->RunKind)
                    {
                        case RunKind.Object:
                        {
                            EmbeddedObjectHost embeddedObjectHost;
                            if (!embeddedObjectHosts.TryGetValue(charIndex, out embeddedObjectHost))
                            {
                                EmbeddedObject embeddedObject = document.LookupObject(run->ObjectIndex);
                                embeddedObjectHost = new EmbeddedObjectHost(embeddedObject, charIndex,
                                    parentControl,
                                    document.LookupStyle(startRun->StyleIndex),
                                    document.LookupStyle(run->StyleIndex));

                                embeddedObjectHosts.Add(charIndex, embeddedObjectHost);
                                pendingEmbeddedObjectHostsToCreate.Enqueue(embeddedObjectHost);
                            }
                            break;
                        }

                        default:
                            break;
                    }

                    charIndex += run->CharCount;
                }
            }
        }

        private void UpdateParagraphLayout()
        {
            if (pendingFirstParagraphToLayout >= 0)
            {
                int firstParagraphToLayout = pendingFirstParagraphToLayout;
                pendingFirstParagraphToLayout = -1;

                UpdateParagraphLayout(firstParagraphToLayout);
            }
        }

        private void UpdateParagraphLayout(int firstParagraphToLayout)
        {
            currentLayoutWidth = desiredLayoutWidth;

            if (firstParagraphToLayout == 0)
            {
                currentLayoutHeight = 0;
                scriptLineBuffer.Count = 0;
            }
            else
            {
                int scriptLineIndex;
                ScriptLine* scriptLine = GetFirstScriptLineOfParagraph(firstParagraphToLayout, out scriptLineIndex);

                if (scriptLine != null)
                {
                    currentLayoutHeight = scriptLine->Y;
                    scriptLineBuffer.Count = scriptLineIndex;
                }
                else if (scriptLineBuffer.Count != 0)
                {
                    firstParagraphToLayout = GetScriptLineZero()[scriptLineBuffer.Count - 1].ParagraphIndex + 1;
                }
                else
                {
                    firstParagraphToLayout = 0;
                }
            }

            using (DeviceContext deviceContext = DeviceContext.CreateFromScreen(scriptMetricsCache))
            {
                int paragraphCount = document.ParagraphCount;
                for (int paragraphIndex = firstParagraphToLayout; paragraphIndex < paragraphCount; paragraphIndex++)
                {
                    AppendScriptLinesForScriptParagraph(deviceContext, paragraphIndex);
                }
            }
        }

        /// <summary>
        /// Computes the layout of a <see cref="ScriptParagraph"/> and appends one or
        /// more <see cref="ScriptLine"/>s to the <see cref="scriptLineBuffer"/>.
        /// </summary>
        /// <param name="deviceContext">The HDC state.</param>
        /// <param name="paragraphIndex">The paragraph index to layout.</param>
        private void AppendScriptLinesForScriptParagraph(DeviceContext deviceContext, int paragraphIndex)
        {
            ScriptParagraph* scriptParagraph = GetScriptParagraph(deviceContext, paragraphIndex);
            int scriptRunCount = scriptParagraph->ScriptRunCount;

            // Always allocate at least one script line for a paragraph.
            ScriptLine* scriptLine = AddScriptLine(paragraphIndex, 0, currentLayoutHeight);

            // Handle paragraph with no runs (sentinel).
            if (scriptRunCount == 0)
            {
                scriptLine->Height = MinimumScriptLineHeight;
                currentLayoutHeight += MinimumScriptLineHeight;
                return;
            }

            // Apply block-level style information for the paragraph from the first logical run.
            ScriptRun* scriptRuns = scriptParagraph->ScriptRuns;
            Style paragraphStyle = GetParagraphStyleAssumingItHasAtLeastOneRun(scriptParagraph);
            scriptLine->X = paragraphStyle.LeftMargin + paragraphStyle.FirstLineIndent;
            int maxX = currentLayoutWidth - paragraphStyle.RightMargin - paragraphStyle.LeftMargin;

            // Loop over all runs in logical order to determine whether everything fits on one line
            // so we can skip the expensive word wrap calculations.
            {
                bool mustLayoutObjects = false;
                int currentX = scriptLine->X;
                for (int scriptRunIndex = 0; scriptRunIndex < scriptRunCount; scriptRunIndex++)
                {
                    ScriptRun* scriptRun = scriptRuns + scriptRunIndex;

                    int measuredWidth;
                    switch (scriptRun->RunKind)
                    {
                        case RunKind.Text:
                        {
                            measuredWidth = scriptRun->ABC.TotalWidth;
                            break;
                        }

                        case RunKind.Object:
                        {
                            measuredWidth = MeasureObjectScriptRun(scriptRun);
                            mustLayoutObjects = true;
                            break;
                        }

                        case RunKind.Tab:
                        {
                            measuredWidth = MeasureTabScriptRun(scriptRun, paragraphStyle, currentX, true);
                            break;
                        }

                        default:
                            throw new NotSupportedException();
                    }

                    currentX += measuredWidth;
                    if (currentX > maxX && paragraphStyle.WordWrap)
                        goto ComplexLoop;
                }

                // Apply pending changes to the script line now that we're done.
                scriptLine->ScriptRunCount = scriptRunCount;
                FinishScriptLineLayout(scriptLine, scriptParagraph, mustLayoutObjects);
                currentLayoutHeight += scriptLine->Height;
                return;
            }

        // Loop over all runs in logical order and pack as many of them as will fit on each line
        // then wrap the rest.
        ComplexLoop:
            {
                Debug.Assert(scriptParagraph->CharCount != 0,
                    "Script paragraph should not have zero chars because that would have been handled by the simple case.");

                int currentX = scriptLine->X;
                int scriptRunIndex = 0;
                int breakCharIndex = 0;
                int breakScriptRunIndex = 0;
                bool mustLayoutObjects = false;
                while (scriptRunIndex < scriptRunCount)
                {
                    ScriptRun* firstRunOnLine = scriptRuns + scriptLine->ScriptRunIndex;
                    int firstCharIndexOnLine = firstRunOnLine->CharIndexInParagraph + scriptLine->TruncatedLeadingCharsCount;

                    // Sum glyph widths in logical order until an overflow is discovered.
                    {
                        ScriptRun* scriptRun = scriptRuns + scriptRunIndex;

                        switch (scriptRun->RunKind)
                        {
                            case RunKind.Text:
                            {
                                int firstCharIndex = scriptRun->CharIndexInParagraph;
                                int lastCharIndex = firstCharIndex + scriptRun->CharCount;
                                if (scriptRun == firstRunOnLine)
                                    firstCharIndex += scriptLine->TruncatedLeadingCharsCount;

                                bool runRightToLeft = scriptRun->ScriptAnalysis.fRTL;
                                int* glyphAdvanceWidths = scriptRun->GlyphAdvanceWidths(scriptParagraph);
                                int glyphIndex = scriptParagraph->CharLogicalClusters[firstCharIndex];
                                for (int charIndex = firstCharIndex; charIndex < lastCharIndex; charIndex++)
                                {
                                    if (scriptParagraph->CharLogicalAttributes[charIndex].fSoftBreakOrfWhitespace
                                        && (charIndex == firstCharIndexOnLine
                                            || !scriptParagraph->CharLogicalAttributes[charIndex - 1].fWhiteSpace))
                                    {
                                        breakCharIndex = charIndex;
                                        breakScriptRunIndex = scriptRunIndex;
                                    }

                                    int nextCharIndex = charIndex + 1;
                                    if (runRightToLeft)
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

                                    if (currentX > maxX && paragraphStyle.WordWrap)
                                        goto BreakLine;
                                }
                                break;
                            }

                            case RunKind.Object:
                            {
                                mustLayoutObjects = true;
                                breakCharIndex = scriptRun->CharIndexInParagraph;
                                breakScriptRunIndex = scriptRunIndex;

                                currentX += MeasureObjectScriptRun(scriptRun);

                                if (currentX > maxX && paragraphStyle.WordWrap)
                                    goto BreakLine;
                                break;
                            }

                            case RunKind.Tab:
                            {
                                breakCharIndex = scriptRun->CharIndexInParagraph;
                                breakScriptRunIndex = scriptRunIndex;

                                currentX += MeasureTabScriptRun(scriptRun, paragraphStyle, currentX, true);

                                if (currentX > maxX && paragraphStyle.WordWrap)
                                    goto BreakLine;
                                break;
                            }
                        }
                    }

                    // No overflow.
                    scriptRunIndex++;
                    continue;

                BreakLine:
                    // Overflow.
                    {
                        // Introduce a hard break at the current script run when we do not find a useful breakpoint.
                        if (breakCharIndex <= firstCharIndexOnLine)
                        {
                            breakScriptRunIndex = scriptRunIndex;
                            breakCharIndex = scriptRuns[breakScriptRunIndex].CharIndexInParagraph;
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

                            // Skip past remaining whitespace if run is in same direction as layout.
                            // This will leave the whitespace hanging off the ragged end of the line.
                            if (breakScriptRun->ScriptAnalysis.fRTL != currentLayoutRightToLeft
                                || !scriptParagraph->CharLogicalAttributes[breakCharIndex].fWhiteSpace)
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
                            FinishScriptLineLayout(scriptLine, scriptParagraph, mustLayoutObjects);
                            mustLayoutObjects = false;
                            currentLayoutHeight += scriptLine->Height;

                            // Start a new script line.
                            scriptRunIndex = breakScriptRunIndex;
                            currentX = paragraphStyle.LeftMargin;

                            scriptLine = AddScriptLine(paragraphIndex, scriptRunIndex, currentLayoutHeight);
                            scriptLine->X = currentX;
                            scriptLine->TruncatedLeadingCharsCount = breakCharOffset;
                        }
                    }
                }

            Finish:
                // Finish the last script line.
                scriptLine->ScriptRunCount = scriptRunCount - scriptLine->ScriptRunIndex;
                FinishScriptLineLayout(scriptLine, scriptParagraph, mustLayoutObjects);
                currentLayoutHeight += scriptLine->Height;
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

        private void FinishScriptLineLayout(ScriptLine* scriptLine, ScriptParagraph* scriptParagraph,
            bool mustLayoutObjects)
        {
            int ascent = MinimumScriptLineHeight;
            int descent = 0;
            ScriptRun* startScriptRun = scriptParagraph->ScriptRuns + scriptLine->ScriptRunIndex;
            ScriptRun* endScriptRun = startScriptRun + scriptLine->ScriptRunCount;
            for (ScriptRun* scriptRun = startScriptRun; scriptRun != endScriptRun; scriptRun++)
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

            if (mustLayoutObjects)
            {
                int xCurrentPosition = scriptLine->X;
                int scriptRunIndex = scriptLine->ScriptRunIndex;
                int scriptRunCount = scriptLine->ScriptRunCount;
                int* visualToLogicalMap = GetTempVisualToLogicalMap(
                    scriptParagraph->ScriptRuns + scriptRunIndex,
                    scriptRunCount);

                for (int i = 0; i < scriptRunCount; i++)
                {
                    int logicalScriptRunIndex =
                        visualToLogicalMap[currentLayoutRightToLeft ? scriptRunCount - i - 1 : i];
                    ScriptRun* scriptRun = scriptParagraph->ScriptRuns + logicalScriptRunIndex + scriptRunIndex;

                    int measuredWidth;
                    switch (scriptRun->RunKind)
                    {
                        case RunKind.Text:
                        {
                            int glyphIndexInParagraph, glyphCount;
                            int truncatedLeadingCharsCount = logicalScriptRunIndex == 0
                                ? scriptLine->TruncatedLeadingCharsCount
                                : 0;
                            int truncatedTrailingCharsCount = logicalScriptRunIndex == scriptRunCount - 1
                                ? scriptLine->TruncatedTrailingCharsCount
                                : 0;
                            measuredWidth = MeasureTextScriptRun(scriptParagraph, scriptRun,
                                truncatedLeadingCharsCount, truncatedTrailingCharsCount,
                                out glyphIndexInParagraph, out glyphCount);
                            break;
                        }

                        case RunKind.Object:
                        {
                            measuredWidth = scriptRun->ABC.TotalWidth;

                            EmbeddedObjectHost embeddedObjectHost;
                            int charIndex = scriptRun->CharIndexInParagraph + scriptParagraph->CharIndex;
                            if (embeddedObjectHosts.TryGetValue(charIndex, out embeddedObjectHost))
                            {
                                embeddedObjectHost.LayoutBounds = new Rectangle(
                                    xCurrentPosition + scriptRun->ABC.abcA,
                                    scriptLine->Y + scriptLine->Ascent - scriptRun->Ascent,
                                    scriptRun->ABC.abcB,
                                    scriptRun->Height);

                                pendingEmbeddedObjectHostsToShow.Enqueue(embeddedObjectHost);
                            }
                            break;
                        }

                        case RunKind.Tab:
                        {
                            measuredWidth = scriptRun->ABC.abcB;
                            break;
                        }

                        default:
                            throw new NotSupportedException();
                    }

                    xCurrentPosition += measuredWidth;
                }
            }
        }

        /// <summary>
        /// Gets a fully analyzed <see cref="ScriptParagraph"/> by paragraph index.
        /// </summary>
        private ScriptParagraph* GetScriptParagraph(DeviceContext deviceContext, int paragraphIndex)
        {
            ScriptParagraph* scriptParagraph;
            if (!scriptParagraphCache.TryGetScriptParagraph(paragraphIndex, out scriptParagraph))
            {
                Paragraph* paragraph = document.GetParagraphZero() + paragraphIndex;
                AnalyzeParagraph(deviceContext, paragraph, scriptParagraph);
            }

            return scriptParagraph;
        }

        /// <summary>
        /// Analyzes a <see cref="Paragraph" /> to populate a <see cref="ScriptParagraph"/>.
        /// </summary>
        private void AnalyzeParagraph(DeviceContext deviceContext, Paragraph* paragraph, ScriptParagraph* scriptParagraph)
        {
            int paragraphCharIndex = paragraph->CharIndex;
            int paragraphCharCount = paragraph->CharCount;

            scriptParagraph->CharIndex = paragraphCharIndex;
            scriptParagraph->CharCount = paragraphCharCount;
            scriptParagraph->ScriptRunCount = 0;
            scriptParagraph->GlyphCount = 0;

            Run* runZero = document.GetRunZero();
            Run* runs = runZero + paragraph->RunIndex;
            int runCount = paragraph->RunCount;

            // Skip paragraphs with no runs (this is the sentinel at end of document)
            if (runCount == 0)
            {
                return;
            }

            // Handle paragraphs with exactly one empty run (simple newline)
            Debug.Assert(paragraphCharCount != 0, "A paragraph with at least one run should always have at least one character.");

            char* charZero = document.GetCharZero();
            char* chars = charZero + paragraphCharIndex;

            // Step 1. Itemize the script items within the paragraph.
            //         Each script item represents a chunk of text (such as a word)
            //         for Unicode rendering purposes.
            scriptParagraph->EnsureCharCapacity(paragraphCharCount);

            SCRIPT_ITEM* tempScriptItems;
            int tempScriptItemCount;
            ScriptItemize(chars, paragraphCharCount, currentLayoutRightToLeft, tempScriptItemBuffer, out tempScriptItems, out tempScriptItemCount);

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
                Style scriptRunStyle = document.LookupStyle(scriptRun->StyleIndex);

                switch (scriptRun->RunKind)
                {
                    case RunKind.Text:
                    {
                        // Fill in basic metrics.
                        ScriptMetrics scriptMetrics = deviceContext.SelectFont(scriptRunStyle.Font);
                        scriptRun->Height = scriptMetrics.Height;
                        scriptRun->Descent = scriptMetrics.Descent;
                        scriptRun->TopMargin = 0;
                        scriptRun->BottomMargin = 0;

                        // Set glyphs.
                        ScriptShapeAndPlace(deviceContext.HDC, ref scriptMetrics.ScriptCache, scriptParagraph, scriptRun, chars);
                        break;
                    }

                    case RunKind.Object:
                    {
                        // No glyphs for an embedded object.
                        scriptRun->GlyphCount = 0;
                        scriptRun->GlyphIndexInParagraph = 0;

                        // Fill in object measurements.
                        int charIndex = scriptRun->CharIndexInParagraph + paragraphCharIndex;
                        EmbeddedObjectHost embeddedObjectHost;
                        EmbeddedObjectMeasurements embeddedObjectMeasurements;
                        if (embeddedObjectHosts.TryGetValue(charIndex, out embeddedObjectHost))
                            embeddedObjectMeasurements = embeddedObjectHost.Measure();
                        else
                            embeddedObjectMeasurements = EmbeddedObjectHost.CreateDefaultEmbeddedObjectMeasurements();

                        scriptRun->Height = embeddedObjectMeasurements.Size.Height;
                        scriptRun->Descent = embeddedObjectMeasurements.Descent;
                        scriptRun->TopMargin = embeddedObjectMeasurements.Margin.Top;
                        scriptRun->BottomMargin = embeddedObjectMeasurements.Margin.Bottom;
                        scriptRun->ABC.abcA = embeddedObjectMeasurements.Margin.Left;
                        scriptRun->ABC.abcB = embeddedObjectMeasurements.Size.Width;
                        scriptRun->ABC.abcC = embeddedObjectMeasurements.Margin.Right;

                        // Change the logical attributes of the character so that an object behaves like
                        // a word rather than whitespace when performing word wrap.
                        scriptParagraph->CharLogicalAttributes[scriptRun->CharIndexInParagraph].SetfSoftBreakfCharStopAndfWordStop();
                        break;
                    }

                    case RunKind.Tab:
                    {
                        // No glyphs for a tab.
                        scriptRun->GlyphCount = 0;
                        scriptRun->GlyphIndexInParagraph = 0;

                        // Use same metrics as inline text.
                        ScriptMetrics scriptMetrics = deviceContext.SelectFont(scriptRunStyle.Font);
                        scriptRun->Height = scriptMetrics.Height;
                        scriptRun->Descent = scriptMetrics.Descent;
                        scriptRun->TopMargin = 0;
                        scriptRun->BottomMargin = 0;
                        scriptRun->ABC.abcA = 0;
                        scriptRun->ABC.abcB = -1; // will be filled in when the TAB is measured
                        scriptRun->ABC.abcC = 0;

                        // Change the logical attributes of the character to make it whitespace since Uniscribe
                        // does not consider a TAB to be whitespace.
                        scriptParagraph->CharLogicalAttributes[scriptRun->CharIndexInParagraph].SetfWhiteSpace();
                        break;
                    }
                }
            }
        }

        private ScriptLine* GetFirstScriptLineOfParagraph(int paragraphIndex, out int scriptLineIndex)
        {
            return BinarySearchForScriptLineOfParagraph(paragraphIndex, true, out scriptLineIndex);
        }

        private ScriptLine* GetLastScriptLineOfParagraph(int paragraphIndex, out int scriptLineIndex)
        {
            return BinarySearchForScriptLineOfParagraph(paragraphIndex, false, out scriptLineIndex);
        }

        private ScriptLine* BinarySearchForScriptLineOfParagraph(int paragraphIndex, bool first, out int scriptLineIndex)
        {
            ScriptLine* scriptLineZero = GetScriptLineZero();
            int scriptLineCount = scriptLineBuffer.Count;

            int low = 0;
            int high = scriptLineCount;
            while (low < high)
            {
                int mid = (low + high) / 2;

                int candidateParagraphIndex = scriptLineZero[mid].ParagraphIndex;
                if (candidateParagraphIndex > paragraphIndex)
                {
                    high = mid;
                }
                else if (candidateParagraphIndex < paragraphIndex)
                {
                    low = mid + 1;
                }
                else
                {
                    if (first)
                    {
                        while (mid > 0 && scriptLineZero[mid - 1].ParagraphIndex == paragraphIndex)
                            mid -= 1;
                    }
                    else
                    {
                        while (mid < scriptLineCount - 1 && scriptLineZero[mid + 1].ParagraphIndex == paragraphIndex)
                            mid += 1;
                    }

                    scriptLineIndex = mid;
                    return scriptLineZero + mid;
                }
            }

            scriptLineIndex = -1;
            return null;
        }

        private ScriptLine* GetScriptLineAtYPositionOrNullIfNone(int yPosition, out int scriptLineIndex)
        {
            ScriptLine* scriptLineZero = GetScriptLineZero();
            int scriptLineCount = scriptLineBuffer.Count;

            int low = 0;
            int high = scriptLineCount;

            while (low < high)
            {
                int mid = (low + high) / 2;

                ScriptLine* scriptLine = scriptLineZero + mid;
                if (yPosition < scriptLine->Y)
                {
                    high = mid;
                }
                else if (yPosition >= scriptLine->Y + scriptLine->Height)
                {
                    low = mid + 1;
                }
                else
                {
                    scriptLineIndex = mid;
                    return scriptLine;
                }
            }

            scriptLineIndex = -1;
            return null;
        }

        private void RaiseUpdateRequired()
        {
            if (UpdateRequired != null)
                UpdateRequired(this, EventArgs.Empty);
        }

        private static void ScriptItemize(char* chars, int charCount, bool rightToLeft,
            UnmanagedBuffer tempScriptItemBuffer, out SCRIPT_ITEM* scriptItems, out int scriptItemCount)
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
            tempEmbeddingLevelBuffer.EnsureCapacity(count);
            byte* embeddingLevels = (byte*)tempEmbeddingLevelBuffer.GetPointer();
            for (int i = 0; i < count; i++)
                embeddingLevels[i] = (byte)scriptRuns[i].ScriptAnalysis.s.uBidiLevel;

            tempVisualToLogicalMapBuffer.EnsureCapacity(count);
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
    }
}
