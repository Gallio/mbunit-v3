// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections.ObjectModel;
using Gallio.Framework.Comparisons;
using Gallio.Model.Logging;
using Gallio.Utilities;

namespace Gallio.Framework.Comparisons
{
    /// <summary>
    /// <para>
    /// A diff set consists of a sequence of differences between a left document and a right document
    /// that indicate changed and unchanged regions.
    /// </para>
    /// <para>
    /// If the changes are applied in order to the left document, the right document will be
    /// reproduced.  If the inverse changes are applied in order to the right document, the
    /// left document will be reproduced.
    /// </para>
    /// </summary>
    /// <todo author="jeff">
    /// Generalize over other document and element types.
    /// </todo>
    [Serializable]
    public sealed class DiffSet
    {
        private readonly IList<Diff> diffs;
        private readonly string leftDocument;
        private readonly string rightDocument;

        /// <summary>
        /// Constructs a diff set.
        /// </summary>
        /// <param name="diffs">The list of differences that indicate the changed and
        /// unchanged regions between the left and right documents.  The diffs span
        /// the entire range of the left and right documents and are listed in document order.</param>
        /// <param name="leftDocument">The left document</param>
        /// <param name="rightDocument">The right document</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="leftDocument"/>,
        /// <paramref name="rightDocument"/> or <paramref name="diffs"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="diffs"/> does not
        /// completely cover the left and right documents or are not listed in the correct order</exception>
        public DiffSet(IList<Diff> diffs, string leftDocument, string rightDocument)
        {
            if (diffs == null)
                throw new ArgumentNullException("diffs");
            if (leftDocument == null)
                throw new ArgumentNullException("leftDocument");
            if (rightDocument == null)
                throw new ArgumentNullException("rightDocument");

            if (!ValidateDiffs(diffs, leftDocument.Length, rightDocument.Length))
                throw new ArgumentException("The list of differences should cover the left and right documents in order.", "diffs");

            this.diffs = diffs;
            this.rightDocument = rightDocument;
            this.leftDocument = leftDocument;
        }

        /// <summary>
        /// Computes the difference between a left document and a right document.
        /// </summary>
        /// <param name="leftDocument">The left document</param>
        /// <param name="rightDocument">The right document</param>
        /// <returns>The set of differences</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="leftDocument"/> or
        /// <paramref name="rightDocument"/> is null</exception>
        public static DiffSet Compute(string leftDocument, string rightDocument)
        {
            if (leftDocument == null)
                throw new ArgumentNullException("leftDocument");
            if (rightDocument == null)
                throw new ArgumentNullException("rightDocument");

            var diffs = new List<Diff>();

            // FIXME: Placeholder.  Replace me pronto with something better researched! -- Jeff.
            if (leftDocument != rightDocument)
                diffs.Add(new Diff(DiffKind.Change, new Range(0, leftDocument.Length), new Range(0, rightDocument.Length)));
            return new DiffSet(diffs, leftDocument, rightDocument);
        }

        /// <summary>
        /// Gets the list of differences that indicate the changed and
        /// unchanged regions between the left and right documents.  The diffs span
        /// the entire range of the left and right documents and are listed in document order.
        /// </summary>
        public IList<Diff> Diffs
        {
            get { return new ReadOnlyCollection<Diff>(diffs); }
        }

        /// <summary>
        /// Gets the left document.
        /// </summary>
        public string LeftDocument
        {
            get { return leftDocument; }
        }

        /// <summary>
        /// Gets the right document.
        /// </summary>
        public string RightDocument
        {
            get { return rightDocument; }
        }

        /// <summary>
        /// Returns true if the list of differences contains changed regions.
        /// </summary>
        public bool ContainsChanges
        {
            get
            {
                foreach (Diff diff in diffs)
                    if (diff.Kind == DiffKind.Change)
                        return true;
                return false;
            }
        }

        /// <summary>
        /// Writes the left document to the structured text writer, with changes highlighted.
        /// </summary>
        /// <param name="writer">The structured text writer to receive the highlighted document</param>
        /// <exception cref="ArgumentNullException">Thrown if <param nameref="builder" /> if null</exception>
        public void WriteHighlightedLeftDocumentTo(TestLogStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            foreach (Diff diff in diffs)
            {
                if (diff.LeftRange.Length != 0)
                {
                    string part = diff.LeftRange.SubstringOf(leftDocument);
                    if (diff.Kind == DiffKind.NoChange)
                        writer.Write(part);
                    else
                        writer.WriteHighlighted(part);
                }
            }
        }

        /// <summary>
        /// Writes the right document to the structured text writer, with changes highlighted.
        /// </summary>
        /// <param name="writer">The structured text writer to receive the highlighted document</param>
        /// <exception cref="ArgumentNullException">Thrown if <param nameref="builder" /> if null</exception>
        public void WriteHighlightedRightDocumentTo(TestLogStreamWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            foreach (Diff diff in diffs)
            {
                if (diff.RightRange.Length != 0)
                {
                    string part = diff.RightRange.SubstringOf(rightDocument);
                    if (diff.Kind == DiffKind.NoChange)
                        writer.Write(part);
                    else
                        writer.WriteHighlighted(part);
                }
            }
        }

        private static bool ValidateDiffs(IEnumerable<Diff> diffs, int leftLength, int rightLength)
        {
            Range prevLeftRange = new Range(0, 0);
            Range prevRightRange = new Range(0, 0);
            foreach (Diff diff in diffs)
            {
                if (diff.LeftRange.StartIndex != prevLeftRange.EndIndex)
                    return false;
                if (diff.RightRange.StartIndex != prevRightRange.EndIndex)
                    return false;

                prevLeftRange = diff.LeftRange;
                prevRightRange = diff.RightRange;
            }

            return prevLeftRange.EndIndex == leftLength && prevRightRange.EndIndex == rightLength;
        }

        private static void Canonicalize(IList<Diff> diffs)
        {
            for (int i = 1; i < diffs.Count; i++)
            {
                Diff prevDiff = diffs[i - 1];
                Diff curDiff = diffs[i];

                if (prevDiff.Kind == curDiff.Kind)
                {
                    diffs.RemoveAt(i);
                    diffs[--i] = new Diff(prevDiff.Kind,
                        prevDiff.LeftRange.ExtendWith(curDiff.LeftRange),
                        prevDiff.RightRange.ExtendWith(curDiff.RightRange));
                }
            }
        }
    }
}
