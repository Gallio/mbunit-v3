using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Gallio.Common.Splash.Native;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// A SplashView is a custom control to display styled text intermixed with
    /// embedded content such as images.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control is optimized for high-performance append-only text output such as
    /// scrolling consoles.  Consequently, the control does not support editing or
    /// modifications to currently displayed content (besides clearing it).
    /// </para>
    /// <para>
    /// Display updates are performed asynchronously in batches.
    /// </para>
    /// </remarks>
    public unsafe partial class SplashView : ScrollableControl
    {
        private readonly LookupTable<Style> styleTable;
        private readonly LookupTable<EmbeddedObject> objectTable;
        private readonly ScriptCacheTable scriptCacheTable;
        private readonly ScriptParagraphCache scriptParagraphCache;

        private readonly UnmanagedBuffer<char> charBuffer;
        private readonly UnmanagedBuffer<Paragraph> paragraphBuffer;
        private readonly UnmanagedBuffer<Run> runBuffer;
        private readonly UnmanagedBuffer<ScriptLine> scriptLineBuffer;

        private readonly UnmanagedBuffer<SCRIPT_ITEM> tempScriptItemBuffer;
        private readonly UnmanagedBuffer<byte> tempEmbeddingLevelBuffer;
        private UnmanagedBuffer<ScriptRun> tempTruncatedScriptRunBuffer;
        private UnmanagedBuffer<ushort> tempTruncatedCharLogicalClusters;

        private readonly Style defaultStyle;
        private Style style;
        private int currentStyleIndex;

        private Paragraph* currentParagraph;
        private Run* currentRun;

        private int firstInvalidParagraphIndex;
        private bool layoutPending;
        private int cachedTextLayoutWidth;
        private int cachedTextLayoutHeight;
        private int minimumTextLayoutWidth = 100;

        /// <summary>
        /// The character used as a placeholder for text itemizing when a paragraph contains an object run.
        /// </summary>
        private const char ObjectRunPlaceholderChar = ' ';

        /// <summary>
        /// The maximum number of characters that a text run can contain.
        /// </summary>
        private const int MaxCharsPerRun = 65535;

        /// <summary>
        /// The maximum number of distinct objects supported by the implementation.
        /// </summary>
        public const int MaxObjects = 65535;

        /// <summary>
        /// The maximum number of distinct styles supported by the implementation.
        /// </summary>
        /// <value>256</value>
        public const int MaxStyles = 256;

        private const int InitialCapacityForCharsPerDocument = 4096;
        private const int InitialCapacityForParagraphsPerDocument = 64;
        private const int InitialCapacityForRunsPerDocument = 128;
        private const int InitialCapacityForLinesPerDocument = 64;

        private const int InitialCapacityForCharsPerParagraph = 128;
        private const int InitialCapacityForScriptRunsPerParagraph = 8;
        private const int InitialCapacityForGlyphsPerParagraph = 128;

        private const int ScriptParagraphCacheSize = 64;

        private const int MinimumScriptLineHeight = 1;

        /// <summary>
        /// Creates an empty SplashView.
        /// </summary>
        public SplashView()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.ResizeRedraw
                | ControlStyles.Selectable | ControlStyles.UserMouse, true);

            styleTable = new LookupTable<Style>(MaxStyles, "This implementation only supports at most {0} distinct styles.");
            objectTable = new LookupTable<EmbeddedObject>(MaxObjects, "This implementation only supports at most {0} distinct objects.");
            scriptCacheTable = new ScriptCacheTable();

            charBuffer = new UnmanagedBuffer<char>(InitialCapacityForCharsPerDocument);
            paragraphBuffer = new UnmanagedBuffer<Paragraph>(InitialCapacityForParagraphsPerDocument);
            runBuffer = new UnmanagedBuffer<Run>(InitialCapacityForRunsPerDocument);
            scriptLineBuffer = new UnmanagedBuffer<ScriptLine>(InitialCapacityForLinesPerDocument);

            tempScriptItemBuffer = new UnmanagedBuffer<SCRIPT_ITEM>(InitialCapacityForScriptRunsPerParagraph);
            tempEmbeddingLevelBuffer = new UnmanagedBuffer<byte>(InitialCapacityForScriptRunsPerParagraph);
            tempTruncatedScriptRunBuffer = new UnmanagedBuffer<ScriptRun>(1);
            tempTruncatedCharLogicalClusters = new UnmanagedBuffer<ushort>(InitialCapacityForCharsPerParagraph);

            scriptParagraphCache = new ScriptParagraphCache(ScriptParagraphCacheSize);

            defaultStyle = CreateDefaultStyle();

            InitializeForNewDocument();

            BackColor = SystemColors.Window;
            Padding = new Padding(3);
        }

        /// <summary>
        /// Gets or sets the minimum width of the text area.
        /// </summary>
        /// <remarks>
        /// If the control is resized to less than this width, then the control will automatically
        /// display a horizontal scrollbar.
        /// </remarks>
        public int MinimumTextLayoutWidth
        {
            get { return minimumTextLayoutWidth; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "Value must be at least 1.");
                minimumTextLayoutWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the current style.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if more than <see cref="MaxStyles" /> distinct styles are used.</exception>
        public Style Style
        {
            get { return style; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                currentStyleIndex = styleTable.AssignIndex(value); // may fail
                style = value;
            }
        }

        /// <summary>
        /// Gets the default style.
        /// </summary>
        public Style DefaultStyle
        {
            get { return defaultStyle; }
        }

        /// <summary>
        /// Clears the text in the view.
        /// </summary>
        public void Clear()
        {
            currentParagraph = null;
            currentRun = null;

            styleTable.Clear();
            objectTable.Clear();
            scriptCacheTable.Clear();
            scriptParagraphCache.Clear();

            charBuffer.Clear();
            paragraphBuffer.Clear();
            runBuffer.Clear();
            scriptLineBuffer.Clear();

            tempScriptItemBuffer.Clear();
            tempEmbeddingLevelBuffer.Clear();
            tempTruncatedScriptRunBuffer.Clear();
            tempTruncatedCharLogicalClusters.Clear();

            InitializeForNewDocument();
        }

        /// <summary>
        /// Appends text to the view.
        /// </summary>
        /// <param name="text">The text to append.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        public void AppendText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            InvalidateLayoutFromCurrentParagraph();
            InternalAppendText(currentStyleIndex, text);
        }

        /// <summary>
        /// Appends a new line to the view.
        /// </summary>
        public void AppendLine()
        {
            if (currentParagraph == null)
            {
                // Cannot use the optimization in the special case when the document is empty
                // because two paragraphs will actually be created.  The first one will be an
                // empty paragraph at the start of the document.  The second one will be the
                // start of the next paragraph created by the new line.
                InvalidateLayoutFromCurrentParagraph();
                InternalAppendNewLine(currentStyleIndex);
            }
            else
            {
                // Optimization: Appending a line does not alter the current paragraph so
                // we only need to invalidate the layout from the start of the next paragraph created.
                InternalAppendNewLine(currentStyleIndex);
                InvalidateLayoutFromCurrentParagraph();
            }
        }

        /// <summary>
        /// Appends an object to the view.
        /// </summary>
        /// <param name="obj">The object to append.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
        public void AppendObject(EmbeddedObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            InvalidateLayoutFromCurrentParagraph();
            InternalAppendObject(currentStyleIndex, obj);
        }

        /// <inheritdoc />
        protected override void OnPaint(PaintEventArgs e)
        {
            UpdateLayoutAndPaintRegion(e.Graphics, e.ClipRectangle);

            base.OnPaint(e);
        }

        /// <inheritdoc />
        protected override void OnPaddingChanged(EventArgs e)
        {
            InvalidateTextAreaDimensions();

            base.OnPaddingChanged(e);
        }

        /// <inheritdoc />
        protected override void OnResize(EventArgs e)
        {
            InvalidateTextAreaDimensions();

            base.OnResize(e);
        }

        /// <inheritdoc />
        protected override void OnScroll(ScrollEventArgs se)
        {
            Invalidate();

            base.OnScroll(se);
        }

        private void InitializeForNewDocument()
        {
            Style = defaultStyle;
            InvalidateLayout();
        }

        private void InternalAppendText(int styleIndex, string text)
        {
            int length = text.Length;
            if (length == 0)
                return;

            fixed (char* textPtr = text)
            {
                char* source = textPtr;
                char* sourceEnd = source + length;
                char* mark = null;

                bool requiresTabExpansion = false;
                for (; source != sourceEnd; source++)
                {
                    char ch = *source;

                    if (char.IsControl(ch))
                    {
                        if (ch == '\n')
                        {
                            InternalAppendCharsFromMarkIfNeeded(styleIndex, ref mark, source, requiresTabExpansion);
                            InternalAppendNewLine(styleIndex);
                            requiresTabExpansion = false;
                            continue;
                        }

                        if (ch == '\t')
                        {
                            requiresTabExpansion = true;
                        }
                        else
                        {
                            // Skip other control characters.
                            InternalAppendCharsFromMarkIfNeeded(styleIndex, ref mark, source, requiresTabExpansion);
                            continue;
                        }
                    }

                    InternalSetMarkIfNeeded(ref mark, source);
                }

                InternalAppendCharsFromMarkIfNeeded(styleIndex, ref mark, source, requiresTabExpansion);
            }
        }

        private static void InternalSetMarkIfNeeded(ref char* mark, char* current)
        {
            if (mark == null)
                mark = current;
        }

        private void InternalAppendCharsFromMarkIfNeeded(int styleIndex, ref char* mark, char* current, bool requiresTabExpansion)
        {
            if (mark != null)
            {
                InternalAppendChars(styleIndex, mark, (int) (current - mark));
                mark = null;

                if (requiresTabExpansion)
                    currentRun->SetRequiresTabExpansion();
            }
        }

        private void InternalAppendNewLine(int styleIndex)
        {
            EnsureParagraph(styleIndex);
            StartParagraph(styleIndex);
        }

        private void InternalAppendChars(int styleIndex, char* source, int count)
        {
            EnsureParagraph(styleIndex);
            EnsureTextRun(styleIndex);

            int charIndex = charBuffer.Count;
            charBuffer.GrowBy(count);
            char* chars = GetCharZero() + charIndex;

            currentParagraph->CharCount += count;

            int newCount = currentRun->CharCount + count;
            while (newCount > MaxCharsPerRun)
            {
                currentRun->CharCount = MaxCharsPerRun;
                newCount -= MaxCharsPerRun;
                StartTextRun(styleIndex);
            }

            currentRun->CharCount = newCount;

            while (count-- > 0)
                *(chars++) = *(source++);
        }

        private void InternalAppendObject(int styleIndex, EmbeddedObject obj)
        {
            int objectIndex = objectTable.AssignIndex(obj);

            EnsureParagraph(styleIndex);

            int runIndex = runBuffer.Count;
            runBuffer.GrowBy(1);

            int charIndex = charBuffer.Count;
            charBuffer.GrowBy(1);

            currentRun = GetRunZero() + runIndex;
            currentRun->InitializeObjectRun(styleIndex, objectIndex);

            char* chars = GetCharZero() + charIndex;
            *chars = ObjectRunPlaceholderChar;

            currentParagraph->RunCount += 1;
            currentParagraph->CharCount += 1;
        }

        private void EnsureParagraph(int styleIndex)
        {
            if (currentParagraph != null)
                return;

            StartParagraph(styleIndex);
        }

        private void StartParagraph(int styleIndex)
        {
            if (currentRun == null && currentParagraph != null)
                StartTextRun(styleIndex);

            int paragraphIndex = paragraphBuffer.Count;
            paragraphBuffer.GrowBy(1);

            currentParagraph = GetParagraphZero() + paragraphIndex;
            currentParagraph->Initialize(charBuffer.Count, 0, runBuffer.Count, 0);

            currentRun = null;
        }

        private void EnsureTextRun(int styleIndex)
        {
            if (currentRun != null
                && currentRun->RunKind == RunKind.Text
                && currentRun->StyleIndex == styleIndex)
                return;

            StartTextRun(styleIndex);
        }

        private void StartTextRun(int styleIndex)
        {
            int runIndex = runBuffer.Count;
            runBuffer.GrowBy(1);

            currentRun = GetRunZero() + runIndex;
            currentRun->InitializeTextRun(styleIndex);

            currentParagraph->RunCount += 1;
        }

        private void InvalidateLayout()
        {
            InvalidateLayout(0);
        }

        private void InvalidateLayoutFromCurrentParagraph()
        {
            InvalidateLayout(currentParagraph == null ? 0 : paragraphBuffer.Count - 1);
        }

        private void InvalidateLayout(int firstInvalidParagraphIndex)
        {
            if (firstInvalidParagraphIndex < this.firstInvalidParagraphIndex)
                this.firstInvalidParagraphIndex = firstInvalidParagraphIndex;

            if (!layoutPending)
            {
                layoutPending = true;
                Invalidate();
            }
        }

        private void InvalidateTextAreaDimensions()
        {
            Rectangle textPaintArea = CalculateTextPaintArea();
            int textLayoutWidth = CalculateTextLayoutWidth(textPaintArea);
            if (textLayoutWidth != cachedTextLayoutWidth)
            {
                InvalidateLayout();
            }
            else
            {
                UpdateScrollBars();
            }
        }

        private Rectangle CalculateTextPaintArea()
        {
            return DisplayRectangle;
        }

        private int CalculateTextLayoutWidth(Rectangle textPaintArea)
        {
            return Math.Max(textPaintArea.Width, minimumTextLayoutWidth);
        }

        private void UpdateLayoutAndPaintRegion(Graphics g, Rectangle clipRectangle)
        {
            Rectangle textPaintArea = CalculateTextPaintArea();

            if (textPaintArea.Width <= 0 || textPaintArea.Height <= 0)
                return;

            Rectangle textClipArea = textPaintArea;
            textClipArea.Intersect(clipRectangle);

            GraphicsState state = g.Save();
            try
            {
                g.SetClip(textClipArea);

                IntPtr hdc = g.GetHdc();
                try
                {
                    HDCState hdcState = new HDCState(hdc, scriptCacheTable);

                    if (layoutPending)
                        UpdateLayout(hdcState, textPaintArea);

                    PaintRegion(hdcState, textPaintArea, textClipArea);
                }
                finally
                {
                    g.ReleaseHdc(hdc);
                }
            }
            finally
            {
                g.Restore(state);
            }
        }

        private void UpdateLayout(HDCState hdcState, Rectangle textPaintArea)
        {
            int textLayoutWidth = CalculateTextLayoutWidth(textPaintArea);
            int textLayoutHeight;

            // Invalidate all lines beginning with first invalid paragraph.
            // Also compute text layout height of remaining valid portion.
            if (firstInvalidParagraphIndex == 0)
            {
                textLayoutHeight = 0;
                scriptLineBuffer.Count = 0;
            }
            else
            {
                int scriptLineIndex;
                ScriptLine* firstInvalidScriptLine = GetFirstScriptLineOfParagraph(firstInvalidParagraphIndex, out scriptLineIndex);

                textLayoutHeight = firstInvalidScriptLine->Y;
                scriptLineBuffer.Count = scriptLineIndex;
            }

            // Layout all paragraphs until end.
            int paragraphCount = paragraphBuffer.Count;
            for (int paragraphIndex = firstInvalidParagraphIndex; paragraphIndex < paragraphCount; paragraphIndex++)
            {
                AppendScriptLinesForScriptParagraph(hdcState, paragraphIndex, textLayoutWidth, ref textLayoutHeight);
            }

            // Update layout parameters.
            firstInvalidParagraphIndex = paragraphBuffer.Count;
            cachedTextLayoutWidth = textLayoutWidth;
            cachedTextLayoutHeight = textLayoutHeight;
            layoutPending = false;

            // Finish up.
            UpdateScrollBars();
        }

        private void UpdateScrollBars()
        {
            AutoScrollMinSize = new Size(minimumTextLayoutWidth, cachedTextLayoutHeight);
        }

        private void PaintRegion(HDCState hdcState, Rectangle textPaintArea, Rectangle textClipArea)
        {
            int yPosition = textClipArea.Top - textPaintArea.Top;
            int firstScriptLineIndex;
            ScriptLine* scriptLine = GetScriptLineAtYPositionOrNullIfNone(yPosition, out firstScriptLineIndex);
            if (scriptLine == null)
                return;

            ScriptLine* endScriptLine = GetScriptLineZero() + scriptLineBuffer.Count;
            int yStartOffset = scriptLine->Y - yPosition;
            int xPosition = textClipArea.Left - textPaintArea.Left;
            int xStartOffset = - xPosition;

            NativeMethods.SetBkMode(hdcState.HDC, NativeConstants.TRANSPARENT);

            int yOffset = yStartOffset;
            while (yOffset < textClipArea.Height && scriptLine != endScriptLine)
            {
                int xOffset = xStartOffset;

                int scriptLineHeight = scriptLine->Height;
                ScriptParagraph* scriptParagraph = GetScriptParagraph(hdcState, scriptLine->ParagraphIndex);
                int scriptRunIndex = scriptLine->ScriptRunIndex;
                int scriptRunCount = scriptLine->ScriptRunCount;
                for (int i = 0; i < scriptRunCount; i++)
                {
                    ScriptRun* scriptRun = scriptParagraph->ScriptRuns + scriptParagraph->ScriptRunVisualToLogicalMap[scriptRunIndex + i];

                    int measuredWidth;
                    bool truncateLeading = i == 0 && scriptLine->TruncatedLeadingCharsCount != 0;
                    bool truncateTrailing = i == scriptRunCount - 1 && scriptLine->TruncatedTrailingCharsCount != 0;
                    if (truncateLeading || truncateTrailing)
                    {
                        Style paragraphStyle = styleTable[scriptParagraph->ScriptRuns[0].StyleIndex];
                        Style style = styleTable[scriptRun->StyleIndex];
                        ScriptCache scriptCache = hdcState.SelectFont(style.Font);
                        ScriptRun* truncatedScriptRun = AcquireTruncatedScriptRunAndExpandTabs(
                            hdcState.HDC, ref scriptCache.ScriptCachePtr,
                            scriptParagraph, scriptRun, scriptParagraph->Chars(GetCharZero()),
                            truncateLeading ? scriptLine->TruncatedLeadingCharsCount : 0,
                            truncateTrailing ? scriptLine->TruncatedTrailingCharsCount : 0,
                            xOffset - textPaintArea.Left, paragraphStyle.TabStopRuler);

                        measuredWidth = truncatedScriptRun->ABC.TotalWidth;

                        if (xOffset + measuredWidth > textClipArea.Left)
                        {
                            int x = textClipArea.Left + xOffset + truncatedScriptRun->ABC.abcA;
                            int y = textClipArea.Top + yOffset + scriptLineHeight - scriptRun->Height; //truncatedScriptRun->Height is not populated
                            ScriptTextOut(hdcState.HDC, ref scriptCache.ScriptCachePtr,
                                x, y, ExtTextOutOptions.NONE, null, scriptParagraph, truncatedScriptRun);
                        }

                        ReleaseTruncatedScriptRun(scriptParagraph, truncatedScriptRun);
                    }
                    else
                    {
                        measuredWidth = scriptRun->ABC.TotalWidth;

                        if (xOffset + measuredWidth > textClipArea.Left)
                        {
                            Style style = styleTable[scriptRun->StyleIndex];
                            ScriptCache scriptCache = hdcState.SelectFont(style.Font);
                            int x = textClipArea.Left + xOffset + scriptRun->ABC.abcA;
                            int y = textClipArea.Top + yOffset + scriptLineHeight - scriptRun->Height;
                            ScriptTextOut(hdcState.HDC, ref scriptCache.ScriptCachePtr,
                                x, y, ExtTextOutOptions.NONE, null, scriptParagraph, scriptRun);
                        }
                    }

                    xOffset += measuredWidth;

                    if (xOffset >= textClipArea.Right)
                        break; // can't fit any more runs within the clip rectangle
                }

                yOffset += scriptLineHeight;
                scriptLine++;
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
                if (candidateParagraphIndex < paragraphIndex)
                {
                    high = mid;
                }
                else if (candidateParagraphIndex > paragraphIndex)
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

            throw new ArgumentException("There are no script lines for the requested paragraph.", "paragraphIndex");
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

        private char* GetCharZero()
        {
            return (char*) charBuffer.GetPointer();
        }

        private Run* GetRunZero()
        {
            return (Run*)runBuffer.GetPointer();
        }

        private Paragraph* GetParagraphZero()
        {
            return (Paragraph*)paragraphBuffer.GetPointer();
        }

        private ScriptLine* GetScriptLineZero()
        {
            return (ScriptLine*)scriptLineBuffer.GetPointer();
        }

        private Style CreateDefaultStyle()
        {
            return new StyleBuilder().ToStyle();
        }
    }
}
