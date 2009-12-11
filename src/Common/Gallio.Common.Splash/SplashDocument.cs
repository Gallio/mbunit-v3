using System;
using System.Diagnostics;
using Gallio.Common.Splash.Internal;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// A splash document contains styled text and embedded objects and retains layout information
    /// for presentation.
    /// </summary>
    public unsafe class SplashDocument
    {
        private readonly LookupTable<Style> styleTable;
        private readonly LookupTable<EmbeddedObject> objectTable;

        private readonly UnmanagedBuffer charBuffer;
        private readonly UnmanagedBuffer paragraphBuffer;
        private readonly UnmanagedBuffer runBuffer;

        private Style previousStyle;
        private int previousStyleIndex;

        private Paragraph* currentParagraph;
        private Run* currentRun;

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

        /// <summary>
        /// Creates an empty splash document.
        /// </summary>
        public SplashDocument()
        {
            styleTable = new LookupTable<Style>(MaxStyles, "This implementation only supports at most {0} distinct styles.");
            objectTable = new LookupTable<EmbeddedObject>(MaxObjects, "This implementation only supports at most {0} distinct objects.");

            charBuffer = new UnmanagedBuffer(InitialCapacityForCharsPerDocument, sizeof(char));
            paragraphBuffer = new UnmanagedBuffer(InitialCapacityForParagraphsPerDocument, sizeof(Paragraph));
            runBuffer = new UnmanagedBuffer(InitialCapacityForRunsPerDocument, sizeof(Run));

            InternalClear();
        }

        /// <summary>
        /// Event raised when the document is cleared.
        /// </summary>
        public event EventHandler DocumentCleared;

        /// <summary>
        /// Event raised when a paragraph is changed.
        /// </summary>
        public event EventHandler<ParagraphChangedEventArgs> ParagraphChanged;

        /// <summary>
        /// Gets the number of paragraphs in the document.
        /// </summary>
        /// <remarks>
        /// This number is guaranteed to always be at least 1 even in an empty document.
        /// </remarks>
        public int ParagraphCount
        {
            get { return paragraphBuffer.Count; }
        }

        /// <summary>
        /// Gets the number of characters in the document.
        /// </summary>
        public int CharCount
        {
            get { return charBuffer.Count; }
        }

        /// <summary>
        /// Clears the text in the document.
        /// </summary>
        public void Clear()
        {
            InternalClear();

            RaiseDocumentCleared();
        }

        private void InternalClear()
        {
            currentParagraph = null;
            currentRun = null;
            previousStyle = null;

            styleTable.Clear();
            objectTable.Clear();

            charBuffer.Clear();
            paragraphBuffer.Clear();
            runBuffer.Clear();

            InternalStartParagraph();
        }

        /// <summary>
        /// Gets a range of the document text.
        /// </summary>
        /// <param name="start">The start character index.</param>
        /// <param name="length">The length of the range.</param>
        /// <returns>The text in the specified range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="start"/>
        /// or <paramref name="length"/> are negative or refer to a range that exceeds the document length.</exception>
        public string GetText(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException("start",  start, "The starting position must be at least 0.");
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", length, "The length must be at least 0.");
            if (start + length > CharCount)
                throw new ArgumentOutOfRangeException("start", "The range must be within the document.");

            return new string(GetCharZero(), start, length);
        }

        /// <summary>
        /// Appends text to the document.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="text">The text to append.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> or <paramref name="text"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if more than <see cref="MaxStyles" /> distinct styles are used.</exception>
        public void AppendText(Style style, string text)
        {
            if (style == null)
                throw new ArgumentNullException("style");
            if (text == null)
                throw new ArgumentNullException("text");

            if (text.Length != 0)
            {
                int styleIndex = InternalAssignStyleIndex(style);
                int paragraphIndex = CurrentParagraphIndex;

                InternalAppendText(styleIndex, text);

                RaiseParagraphChanged(paragraphIndex);
            }
        }

        /// <summary>
        /// Appends a new line to the document.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if more than <see cref="MaxStyles" /> distinct styles are used.</exception>
        public void AppendLine(Style style)
        {
            if (style == null)
                throw new ArgumentNullException("style");

            int styleIndex = InternalAssignStyleIndex(style);
            int paragraphIndex = CurrentParagraphIndex;

            fixed (char* chars = "\n")
                InternalAppendChars(styleIndex, chars, 1);
            InternalStartParagraph();

            RaiseParagraphChanged(paragraphIndex);
        }

        /// <summary>
        /// Appends an object to the document.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="obj">The object to append.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> or <paramref name="obj"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if more than <see cref="MaxStyles" /> distinct styles are used.</exception>
        public void AppendObject(Style style, EmbeddedObject obj)
        {
            if (style == null)
                throw new ArgumentNullException("style");
            if (obj == null)
                throw new ArgumentNullException("obj");

            int styleIndex = InternalAssignStyleIndex(style);
            int paragraphIndex = CurrentParagraphIndex;

            InternalAppendObject(styleIndex, obj);

            RaiseParagraphChanged(paragraphIndex);
        }

        /// <summary>
        /// Returns the plain text content of the document as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetText(0, CharCount);
        }

        private int InternalAssignStyleIndex(Style style)
        {
            if (previousStyle == style)
                return previousStyleIndex;

            previousStyleIndex = styleTable.AssignIndex(style); // may fail due to too many styles
            previousStyle = style;
            return previousStyleIndex;
        }

        private void InternalAppendText(int styleIndex, string text)
        {
            int length = text.Length;
            fixed (char* textPtr = text)
            {
                char* source = textPtr;
                char* sourceEnd = source + length;

                char* mark = source;
                for (; source != sourceEnd; source++)
                {
                    char ch = *source;

                    if (char.IsControl(ch))
                    {
                        if (ch == '\n')
                        {
                            char* next = source + 1;
                            InternalAppendChars(styleIndex, mark, (int)(next - mark));
                            mark = next;
                            InternalStartParagraph();
                        }
                        else if (ch == '\t')
                        {
                            if (mark != source)
                                InternalAppendChars(styleIndex, mark, (int)(source - mark));
                            InternalAppendTabRun(styleIndex);
                            mark = source + 1;
                        }
                        else
                        {
                            // Discard all other control characters.
                            if (mark != source)
                                InternalAppendChars(styleIndex, mark, (int) (source - mark));
                            mark = source + 1;
                        }
                    }
                }

                if (mark != source)
                    InternalAppendChars(styleIndex, mark, (int)(source - mark));
            }
        }

        private void InternalAppendChars(int styleIndex, char* source, int count)
        {
            InternalEnsureTextRun(styleIndex);

            int charIndex = charBuffer.Count;
            charBuffer.GrowBy(count);
            char* chars = GetCharZero() + charIndex;

            currentParagraph->CharCount += count;

            int newCount = currentRun->CharCount + count;
            while (newCount > MaxCharsPerRun)
            {
                currentRun->CharCount = MaxCharsPerRun;
                newCount -= MaxCharsPerRun;
                InternalStartTextRun(styleIndex);
            }

            currentRun->CharCount = newCount;

            while (count-- > 0)
                *(chars++) = *(source++);
        }

        private void InternalStartParagraph()
        {
            Debug.Assert(currentRun != null || currentParagraph == null,
                "At least one run should be added to the current paragraph before a new one is started.");

            int paragraphIndex = paragraphBuffer.Count;
            paragraphBuffer.GrowBy(1);

            currentParagraph = GetParagraphZero() + paragraphIndex;
            currentParagraph->Initialize(charBuffer.Count, 0, runBuffer.Count, 0);

            currentRun = null;
        }

        private void InternalEnsureTextRun(int styleIndex)
        {
            if (currentRun != null
                && currentRun->RunKind == RunKind.Text
                && currentRun->StyleIndex == styleIndex)
                return;

            InternalStartTextRun(styleIndex);
        }

        private void InternalStartTextRun(int styleIndex)
        {
            int runIndex = runBuffer.Count;
            runBuffer.GrowBy(1);

            currentRun = GetRunZero() + runIndex;
            currentRun->InitializeTextRun(styleIndex);

            currentParagraph->RunCount += 1;
        }

        private void InternalAppendObject(int styleIndex, EmbeddedObject obj)
        {
            int objectIndex = objectTable.AssignIndex(obj);

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

        private void InternalAppendTabRun(int styleIndex)
        {
            int runIndex = runBuffer.Count;
            runBuffer.GrowBy(1);

            int charIndex = charBuffer.Count;
            charBuffer.GrowBy(1);

            currentRun = GetRunZero() + runIndex;
            currentRun->InitializeTabRun(styleIndex);

            char* chars = GetCharZero() + charIndex;
            *chars = '\t';

            currentParagraph->RunCount += 1;
            currentParagraph->CharCount += 1;
        }

        private void RaiseDocumentCleared()
        {
            if (DocumentCleared != null)
                DocumentCleared(this, EventArgs.Empty);
        }

        private void RaiseParagraphChanged(int paragraphIndex)
        {
            if (ParagraphChanged != null)
                ParagraphChanged(this, new ParagraphChangedEventArgs(paragraphIndex));
        }

        internal Style LookupStyle(int styleIndex)
        {
            return styleTable[styleIndex];
        }

        internal EmbeddedObject LookupObject(int objectIndex)
        {
            return objectTable[objectIndex];
        }

        internal char* GetCharZero()
        {
            return (char*)charBuffer.GetPointer();
        }

        internal Run* GetRunZero()
        {
            return (Run*)runBuffer.GetPointer();
        }

        internal Paragraph* GetParagraphZero()
        {
            return (Paragraph*)paragraphBuffer.GetPointer();
        }

        internal int StyleCount
        {
            get { return styleTable.Count; }
        }

        internal int ObjectCount
        {
            get { return objectTable.Count; }
        }

        internal int RunCount
        {
            get { return runBuffer.Count; }
        }

        internal int CurrentParagraphIndex
        {
            get { return ParagraphCount - 1; }
        }
    }
}
