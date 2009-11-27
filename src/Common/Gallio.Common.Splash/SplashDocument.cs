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

        private readonly UnmanagedBuffer<char> charBuffer;
        private readonly UnmanagedBuffer<Paragraph> paragraphBuffer;
        private readonly UnmanagedBuffer<Run> runBuffer;

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

            charBuffer = new UnmanagedBuffer<char>(InitialCapacityForCharsPerDocument);
            paragraphBuffer = new UnmanagedBuffer<Paragraph>(InitialCapacityForParagraphsPerDocument);
            runBuffer = new UnmanagedBuffer<Run>(InitialCapacityForRunsPerDocument);

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
        public int ParagraphCount
        {
            get { return paragraphBuffer.Count; }
        }

        /// <summary>
        /// Gets the index of the current paragraph.
        /// </summary>
        public int CurrentParagraphIndex
        {
            get { return currentParagraph == null ? 0 : paragraphBuffer.Count - 1; }
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
                InternalAppendChars(styleIndex, chars, 1, false);
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
                bool requiresTabExpansion = false;
                for (; source != sourceEnd; source++)
                {
                    char ch = *source;

                    if (char.IsControl(ch))
                    {
                        if (ch == '\n')
                        {
                            char* next = source + 1;
                            InternalAppendChars(styleIndex, mark, (int)(next - mark), requiresTabExpansion);
                            mark = next;
                            requiresTabExpansion = false;
                            InternalStartParagraph();
                            continue;
                        }

                        if (ch == '\t')
                        {
                            requiresTabExpansion = true;
                        }
                        else
                        {
                            // Discard all other control characters.
                            if (mark != source)
                                InternalAppendChars(styleIndex, mark, (int)(source - mark), requiresTabExpansion);
                            mark = source + 1;
                            continue;
                        }
                    }
                }

                if (mark != source)
                    InternalAppendChars(styleIndex, mark, (int)(source - mark), requiresTabExpansion);
            }
        }

        private void InternalAppendChars(int styleIndex, char* source, int count, bool requiresTabExpansion)
        {
            InternalEnsureTextRun(styleIndex);
            if (requiresTabExpansion)
                currentRun->SetRequiresTabExpansion();

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
                if (requiresTabExpansion)
                    currentRun->SetRequiresTabExpansion();
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
    }
}
