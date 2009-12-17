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
        private readonly Dictionary<string, object> annotationTables;
        private readonly Stack<int> styleIndexStack;

        private readonly UnmanagedBuffer charBuffer;
        private readonly UnmanagedBuffer paragraphBuffer;
        private readonly UnmanagedBuffer runBuffer;

        private Paragraph* currentParagraph;
        private Run* currentRun;

        /// <summary>
        /// The character used as a placeholder for text itemizing when a paragraph contains an object run.
        /// </summary>
        private const char ObjectRunPlaceholderChar = ' ';

        /// <summary>
        /// The maximum number of characters that a text run can contain.
        /// </summary>
        internal const int MaxCharsPerRun = 65535;

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
            annotationTables = new Dictionary<string, object>();
            styleIndexStack = new Stack<int>();

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
        /// Gets the current style.
        /// </summary>
        public Style CurrentStyle
        {
            get { return LookupStyle(CurrentStyleIndex); }
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

            styleTable.Clear();
            objectTable.Clear();
            annotationTables.Clear();
            styleIndexStack.Clear();

            charBuffer.Clear();
            paragraphBuffer.Clear();
            runBuffer.Clear();

            InternalBeginStyle(Style.Default);
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
        public string GetTextRange(int start, int length)
        {
            ValidateCharacterRange(start, length);
            return new string(GetCharZero(), start, length);
        }

        /// <summary>
        /// Gets the style of the text at the specified character index.
        /// </summary>
        /// <param name="index">The character index.</param>
        /// <returns>The style.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is
        /// outside of the bounds of the document.</exception>
        public Style GetStyleAtIndex(int index)
        {
            Run* run = ValidateCharacterIndexAndGetRun(index);
            return LookupStyle(run->StyleIndex);
        }

        /// <summary>
        /// Gets the embedded object at the specified character index, or null if none.
        /// </summary>
        /// <param name="index">The character index.</param>
        /// <returns>The embedded object, or null if none.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is
        /// outside of the bounds of the document.</exception>
        public EmbeddedObject GetObjectAtIndex(int index)
        {
            Run* run = ValidateCharacterIndexAndGetRun(index);
            return run->RunKind == RunKind.Object ? LookupObject(run->ObjectIndex) : null;
        }

        /// <summary>
        /// Gets the annotation with the specified key at the specified character index.
        /// </summary>
        /// <param name="key">The annotation key.</param>
        /// <param name="index">The character index.</param>
        /// <param name="value">Set to the annotation value, or default if none.</param>
        /// <returns>True if an annotation was found.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is
        /// outside of the bounds of the document.</exception>
        public bool TryGetAnnotationAtIndex<T>(Key<T> key, int index, out T value)
        {
            ValidateCharacterIndex(index);

            AnnotationTable<T> annotationTable = GetAnnotationTable(key, false);
            if (annotationTable == null)
            {
                value = default(T);
                return false;
            }

            return annotationTable.TryGetValueAtIndex(index, out value);
        }

        /// <summary>
        /// Gets the current annotation with the specified key.
        /// </summary>
        /// <param name="key">The annotation key.</param>
        /// <param name="value">Set to the annotation value, or default if none.</param>
        /// <returns>True if there is a current annotation.</returns>
        public bool TryGetCurrentAnnotation<T>(Key<T> key, out T value)
        {
            AnnotationTable<T> annotationTable = GetAnnotationTable(key, false);
            if (annotationTable == null)
            {
                value = default(T);
                return false;
            }

            return annotationTable.TryGetCurrentValue(out value);
        }

        /// <summary>
        /// Appends text to the document.
        /// </summary>
        /// <param name="text">The text to append.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        public void AppendText(string text)
        {
            if (text == null)
                throw new ArgumentNullException("text");

            if (text.Length != 0)
            {
                int styleIndex = CurrentStyleIndex;
                int paragraphIndex = CurrentParagraphIndex;

                InternalAppendText(styleIndex, text);

                RaiseParagraphChanged(paragraphIndex);
            }
        }

        /// <summary>
        /// Appends a new line to the document.
        /// </summary>
        public void AppendLine()
        {
            int styleIndex = CurrentStyleIndex;
            int paragraphIndex = CurrentParagraphIndex;

            fixed (char* chars = "\n")
                InternalAppendChars(styleIndex, chars, 1);
            InternalStartParagraph();

            RaiseParagraphChanged(paragraphIndex);
        }

        /// <summary>
        /// Appends an object to the document.
        /// </summary>
        /// <param name="obj">The object to append.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="obj"/> is null.</exception>
        public void AppendObject(EmbeddedObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            int styleIndex = CurrentStyleIndex;
            int paragraphIndex = CurrentParagraphIndex;

            InternalAppendObject(styleIndex, obj);

            RaiseParagraphChanged(paragraphIndex);
        }

        /// <summary>
        /// Pushes a new style onto the style stack.
        /// Subsequently appended content will use the new style.
        /// </summary>
        /// <example>
        /// <code>
        /// using (document.BeginStyle(linkStyle))
        /// {
        ///     using (document.BeginAnnotation("href", "http://www.gallio.org"))
        ///     {
        ///         document.AppendText("Gallio");
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="style">The style to use.</param>
        /// <returns>A value that when disposed automatically calls <see cref="EndStyle"/>.
        /// Used with the C# "using" syntax to end the style at the end of a block scope.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if more than <see cref="MaxStyles" /> distinct styles are used.</exception>
        /// <seealso cref="EndStyle"/>
        public DisposableCookie BeginStyle(Style style)
        {
            if (style == null)
                throw new ArgumentNullException("style");

            InternalBeginStyle(style);
            return new DisposableCookie(EndStyle);
        }

        private void InternalBeginStyle(Style style)
        {
            int styleIndex = styleTable.AssignIndex(style); // may throw InvalidOperationException due to too many styles
            styleIndexStack.Push(styleIndex);
        }

        /// <summary>
        /// Pops the current style off the style stack.
        /// Subsequently appended content will use the previous style.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the style stack only contains the default style.</exception>
        /// <seealso cref="BeginStyle"/>
        public void EndStyle()
        {
            if (styleIndexStack.Count <= 1)
                throw new InvalidOperationException("The style stack only contains the default style which cannot be popped.");

            styleIndexStack.Pop();
        }

        /// <summary>
        /// Pushes an annotation value onto the keyed annotation stack.
        /// Subsequently appended content will acquire the new annotation value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each annotation key has its own separate stack.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// using (document.BeginStyle(linkStyle))
        /// {
        ///     using (document.BeginAnnotation("href", "http://www.gallio.org"))
        ///     {
        ///         document.AppendText("Gallio");
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <param name="key">The annotation key.</param>
        /// <param name="value">The annotation value.</param>
        /// <returns>A value that when disposed automatically calls <see cref="EndAnnotation{T}"/>.
        /// Used with the C# "using" syntax to end the annotation at the end of a block scope.</returns>
        /// <seealso cref="EndAnnotation{T}"/>
        public DisposableCookie BeginAnnotation<T>(Key<T> key, T value)
        {
            InternalBeginAnnotation(key, value);
            return new DisposableCookie(() => EndAnnotation(key));
        }

        private void InternalBeginAnnotation<T>(Key<T> key, T value)
        {
            AnnotationTable<T> annotationTable = GetAnnotationTable(key, true);
            annotationTable.BeginValue(CharCount, value);
        }

        /// <summary>
        /// Pushes an annotation value onto the keyed annotation stack.
        /// Subsequently appended content will acquire the new annotation value.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Each annotation key has its own separate stack.
        /// </para>
        /// </remarks>
        /// <param name="key">The annotation key.</param>
        /// <exception cref="InvalidOperationException">Thrown if there is no current annotation with the
        /// specified key.</exception>
        /// <seealso cref="BeginAnnotation{T}"/>
        public void EndAnnotation<T>(Key<T> key)
        {
            AnnotationTable<T> annotationTable = GetAnnotationTable(key, false);
            if (annotationTable == null || !annotationTable.TryEndValue(CharCount))
                throw new InvalidOperationException(string.Format("There is no current annotation with key '{0}'.", key.Name));
        }

        /// <summary>
        /// Returns the plain text content of the document as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetTextRange(0, CharCount);
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

        private int CurrentStyleIndex
        {
            get { return styleIndexStack.Peek(); }
        }

        private Paragraph* GetParagraphAtCharacterIndex(int index, out int paragraphIndex)
        {
            Paragraph* paragraphZero = GetParagraphZero();
            int paragraphCount = ParagraphCount;

            int low = 0;
            int high = paragraphCount;
            while (low < high)
            {
                int mid = (low + high) / 2;

                int candidateCharIndex = paragraphZero[mid].CharIndex;
                if (candidateCharIndex > index)
                {
                    high = mid;
                }
                else if (candidateCharIndex + paragraphZero[mid].CharCount < index)
                {
                    low = mid + 1;
                }
                else
                {
                    paragraphIndex = mid;
                    return paragraphZero + mid;
                }
            }

            paragraphIndex = -1;
            return null;
        }

        private Run* GetRunAtCharacterIndex(int index, out int runIndex)
        {
            int paragraphIndex;
            Paragraph* paragraph = GetParagraphAtCharacterIndex(index, out paragraphIndex);
            if (paragraph != null)
            {
                int remainingOffset = index - paragraph->CharIndex;

                Run* startRun = GetRunZero() + paragraph->RunIndex;
                Run* endRun = startRun + paragraph->RunCount;
                for (Run* run = startRun; run != endRun; run++)
                {
                    remainingOffset -= run->CharCount;

                    if (remainingOffset < 0)
                    {
                        runIndex = (int) (run - startRun);
                        return run;
                    }
                }
            }

            runIndex = -1;
            return null;
        }

        private void ValidateCharacterRange(int start, int length)
        {
            if (start < 0)
                throw new ArgumentOutOfRangeException("start", start, "The start index must be at least 0.");
            if (length < 0)
                throw new ArgumentOutOfRangeException("length", length, "The length must be at least 0.");
            if (start + length > CharCount)
                throw new ArgumentOutOfRangeException("start", "The range must be within the document.");
        }

        private void ValidateCharacterIndex(int index)
        {
            if (index < 0 || index >= CharCount)
                throw new ArgumentOutOfRangeException("index", index, "The index must be within the bounds of the document.");
        }

        private Run* ValidateCharacterIndexAndGetRun(int index)
        {
            ValidateCharacterIndex(index);

            int runIndex;
            Run* run = GetRunAtCharacterIndex(index, out runIndex);
            Debug.Assert(run != null, "We should have found a run at the character index since the index was within the bounds of the document.");
            return run;
        }

        private AnnotationTable<T> GetAnnotationTable<T>(Key<T> key, bool createIfAbsent)
        {
            object annotationTable;
            if (! annotationTables.TryGetValue(key.Name, out annotationTable) && createIfAbsent)
            {
                annotationTable = new AnnotationTable<T>();
                annotationTables.Add(key.Name, annotationTable);
            }

            return (AnnotationTable<T>) annotationTable;
        }

        private sealed class AnnotationTable<T>
        {
            private readonly Stack<T> valueStack;
            private readonly List<KeyValuePair<int, T>> runs;

            public AnnotationTable()
            {
                valueStack = new Stack<T>();
                runs = new List<KeyValuePair<int, T>>();
            }

            public bool TryGetCurrentValue(out T value)
            {
                if (valueStack.Count != 0)
                {
                    value = valueStack.Peek();
                    return true;
                }

                value = default(T);
                return false;
            }

            public bool TryGetValueAtIndex(int index, out T value)
            {
                int low = 0;
                int high = runs.Count;
                while (low < high)
                {
                    int mid = (low + high) / 2;

                    if (Math.Abs(runs[mid].Key) > index)
                    {
                        high = mid;
                    }
                    else if (mid + 1 < high && Math.Abs(runs[mid + 1].Key) < index)
                    {
                        low = mid + 1;
                    }
                    else
                    {
                        if (runs[mid].Key < 0)
                            break;

                        value = runs[mid].Value;
                        return true;
                    }
                }

                value = default(T);
                return false;
            }

            public void BeginValue(int index, T value)
            {
                valueStack.Push(value);
                RemoveSentinel(index);
                runs.Add(new KeyValuePair<int, T>(index, value));
            }

            public bool TryEndValue(int index)
            {
                if (valueStack.Count == 0)
                    return false;

                valueStack.Pop();
                RemoveSentinel(index);

                if (valueStack.Count == 0)
                {
                    if (index != 0)
                        runs.Add(new KeyValuePair<int, T>(-index, default(T)));
                }
                else
                {
                    runs.Add(new KeyValuePair<int, T>(index, valueStack.Peek()));
                }

                return true;
            }

            private void RemoveSentinel(int index)
            {
                if (runs.Count != 0 && index == Math.Abs(runs[runs.Count - 1].Key))
                    runs.RemoveAt(runs.Count - 1);
            }
        }
    }
}
