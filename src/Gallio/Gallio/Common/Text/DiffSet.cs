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
using System.Collections.ObjectModel;
using Gallio.Common.Markup;

namespace Gallio.Common.Text
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
    /// <remarks>
    /// <para>
    /// This implementation is based on Myers' O((M + N) * D) time and O(M + N) space algorithm
    /// for computing the longest common subsequence from his paper
    /// "An O(ND) Difference Algorithm and Its Variations."
    /// (http://citeseer.ist.psu.edu/myers86ond.html)
    /// </para>
    /// <para>
    /// There were two other sources for inspiration although this implementation is not
    /// a direct port of either of them.
    /// <list type="bullet">
    /// <item>The org.eclipse.compare.internal.LCS class in Eclipse (http://www.eclipse.org) from
    /// which we borrow the concept of limiting the number of differences to produce an approximate
    /// result with a reduced time bound for large data sets.  Since the Eclipse implementation
    /// of the LCS follows Myers' algorithm pretty closely, it was also very useful as a point
    /// of comparison for finding bugs.</item>
    /// <item>Neil Fraser's "Diff Match and Patch" algorithm (http://code.google.com/p/google-diff-match-patch/).
    /// We borrow some ideas about semantic cleanup from here.</item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class DiffSet : IMarkupStreamWritable
    {
        private const int SmallChangeThreshold = 5;

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
        /// Gets the set of differences between a left document and a right document.
        /// </summary>
        /// <param name="leftDocument">The left document</param>
        /// <param name="rightDocument">The right document</param>
        /// <returns>The set of differences</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="leftDocument"/> or
        /// <paramref name="rightDocument"/> is null</exception>
        public static DiffSet GetDiffSet(string leftDocument, string rightDocument)
        {
            return GetDiffSet(leftDocument, rightDocument, true, true);
        }

        internal static DiffSet GetDiffSet(string leftDocument, string rightDocument, bool optimize, bool boundRuntime)
        {
            if (leftDocument == null)
                throw new ArgumentNullException("leftDocument");
            if (rightDocument == null)
                throw new ArgumentNullException("rightDocument");

            var diffs = new List<Diff>();

            if (optimize)
                FastDiff(diffs, new Substring(leftDocument), new Substring(rightDocument), boundRuntime);
            else
                SlowDiff(diffs, new Substring(leftDocument), new Substring(rightDocument), boundRuntime);

            CanonicalizeDiffs(diffs);
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
        /// Returns true if the list of differences is empty which can only occur when
        /// both document being compared are empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return diffs.Count == 0; }
        }

        /// <summary>
        /// <para>
        /// Writes the diffs using the <see cref="DiffStyle.Interleaved" />
        /// presentation style and no limits on the context length.
        /// </para>
        /// <para>
        /// For the purposes of determining additions and deletions, the left document
        /// is considered the original and the right document is the considered to be the
        /// one that was modified.  Changes are annotated by markers:
        /// by <see cref="Marker.DiffAddition" />, <see cref="Marker.DiffDeletion" />
        /// and <see cref="Marker.DiffChange" />.
        /// </para>
        /// </summary>
        /// <param name="writer">The test log stream writer to receive the highlighted document</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref nameref="writer" /> if null</exception>
        public void WriteTo(MarkupStreamWriter writer)
        {
            WriteTo(writer, DiffStyle.Interleaved, int.MaxValue);
        }

        /// <summary>
        /// <para>
        /// Writes the diffs using the specified
        /// presentation style and no limits on the context length.
        /// </para>
        /// <para>
        /// Changes are annotated by markers: <see cref="Marker.DiffAddition" />, <see cref="Marker.DiffDeletion" />
        /// and <see cref="Marker.DiffChange" />.
        /// </para>
        /// <para>
        /// If the style is <see cref="DiffStyle.Interleaved" /> then the left document
        /// is considered the original and the right document is the considered to be the
        /// one that was modified so deletions appear within the left and additions within the right.
        /// </para>
        /// <para>
        /// If the style is <see cref="DiffStyle.LeftOnly" /> or <see cref="DiffStyle.RightOnly" />
        /// then only the deletion and changed markers are used.
        /// </para>
        /// </summary>
        /// <param name="writer">The test log stream writer to receive the highlighted document</param>
        /// <param name="style">The presentation style</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref nameref="writer" /> if null</exception>
        public void WriteTo(MarkupStreamWriter writer, DiffStyle style)
        {
            WriteTo(writer, style, int.MaxValue);
        }

        /// <summary>
        /// <para>
        /// Writes the diffs using the specified
        /// presentation style and max context length.
        /// </para>
        /// <para>
        /// Changes are annotated by markers: <see cref="Marker.DiffAddition" />, <see cref="Marker.DiffDeletion" />
        /// and <see cref="Marker.DiffChange" />.
        /// </para>
        /// <para>
        /// If the style is <see cref="DiffStyle.Interleaved" /> then the left document
        /// is considered the original and the right document is the considered to be the
        /// one that was modified so deletions appear within the left and additions within the right.
        /// </para>
        /// <para>
        /// If the style is <see cref="DiffStyle.LeftOnly" /> or <see cref="DiffStyle.RightOnly" />
        /// then only the deletion and changed markers are used.
        /// </para>
        /// </summary>
        /// <param name="writer">The test log stream writer to receive the highlighted document</param>
        /// <param name="style">The presentation style</param>
        /// <param name="maxContextLength">The maximum number of characters of unchanged regions
        /// to display for context, or <see cref="int.MaxValue" /> for no limit.  Extraneous context
        /// is split in two with an ellipsis inserted in between both halves</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref nameref="writer" /> if null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxContextLength"/>
        /// is negative</exception>
        public void WriteTo(MarkupStreamWriter writer, DiffStyle style, int maxContextLength)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");
            if (maxContextLength < 0)
                throw new ArgumentOutOfRangeException("maxContextLength");

            foreach (Diff diff in diffs)
            {
                if (diff.Kind == DiffKind.NoChange)
                {
                    WriteContext(writer, new Substring(leftDocument, diff.LeftRange), maxContextLength);
                }
                else
                {
                    if (diff.LeftRange.Length != 0)
                    {
                        switch (style)
                        {
                            case DiffStyle.Interleaved:
                                using (writer.BeginMarker(Marker.DiffDeletion))
                                    writer.Write(diff.LeftRange.SubstringOf(leftDocument));
                                break;

                            case DiffStyle.LeftOnly:
                                using (writer.BeginMarker(diff.RightRange.Length == 0 ? Marker.DiffDeletion : Marker.DiffChange))
                                    writer.Write(diff.LeftRange.SubstringOf(leftDocument));
                                break;
                        }
                    }

                    if (diff.RightRange.Length != 0)
                    {
                        switch (style)
                        {
                            case DiffStyle.Interleaved:
                                using (writer.BeginMarker(Marker.DiffAddition))
                                    writer.Write(diff.RightRange.SubstringOf(rightDocument));
                                break;

                            case DiffStyle.RightOnly:
                                using (writer.BeginMarker(diff.LeftRange.Length == 0 ? Marker.DiffDeletion : Marker.DiffChange))
                                    writer.Write(diff.RightRange.SubstringOf(rightDocument));
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// <para>
        /// Simplifies the diff for presentation.
        /// </para>
        /// <para>
        /// This method applies a series of heuristics to make the diff easier to read
        /// but perhaps less optimal, including the following:
        /// <list type="bullet">
        /// <item>Adjacent diffs of the same kind are combined.</item>
        /// <item>Small unchanged regions sandwiched between larger changed regions are
        /// converted to larger changed regions.  This improves the case when only
        /// a few scattered characters coincidentally match between the two documents.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <returns>Returns a simplified diff</returns>
        public DiffSet Simplify()
        {
            if (diffs.Count <= 1)
                return this;

            List<Diff> simplifiedDiffs = new List<Diff>(diffs);
            CanonicalizeDiffs(simplifiedDiffs);

            for (int i = 1; i < simplifiedDiffs.Count - 1; i++)
            {
                Diff middleDiff = simplifiedDiffs[i];
                if (middleDiff.Kind == DiffKind.NoChange)
                {
                    int middleLength = middleDiff.EffectiveLength;
                    if (middleLength > SmallChangeThreshold)
                        continue;

                    // Note: Because the diffs have been canonicalized, we know that the adjacent
                    //       diffs must be Changes.
                    Diff leftDiff = simplifiedDiffs[i - 1];
                    Diff rightDiff = simplifiedDiffs[i + 1];

                    if (middleLength <= leftDiff.EffectiveLength && middleLength <= rightDiff.EffectiveLength)
                    {
                        Diff simplifiedDiff = new Diff(DiffKind.Change,
                            Range.Between(leftDiff.LeftRange.StartIndex, rightDiff.LeftRange.EndIndex),
                            Range.Between(leftDiff.RightRange.StartIndex, rightDiff.RightRange.EndIndex));

                        simplifiedDiffs[i - 1] = simplifiedDiff;
                        simplifiedDiffs.RemoveRange(i, 2);

                        // Go back to the previous unchanged region, if there is one, and re-evaluate
                        // whether it should be merged given that we just increased the length of its
                        // successor.  Otherwise we simply continue on to the next unchanged region.
                        if (i > 2)
                            i -= 3;
                        else
                            i -= 1;
                    }
                }
            }

            return new DiffSet(simplifiedDiffs, leftDocument, rightDocument);
        }

        private static void WriteContext(MarkupStreamWriter writer, Substring context, int maxContextLength)
        {
            if (context.Length < maxContextLength)
            {
                writer.Write(context.ToString());
            }
            else
            {
                int split = maxContextLength / 2;
                if (split > 0)
                {
                    writer.Write(context.Extract(0, split).ToString());
                    writer.WriteEllipsis();
                    writer.Write(context.Extract(context.Length - split));
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

        private static void FastDiff(IList<Diff> diffs, Substring left, Substring right, bool boundRuntime)
        {
            // If either document is empty, then the change covers the whole document.
            if (left.Length == 0 || right.Length == 0)
            {
                diffs.Add(new Diff(DiffKind.Change, left.Range, right.Range));
                return;
            }

            // Reduce the problem size by identifying a common prefix and suffix, if any.
            int commonPrefixLength = left.FindCommonPrefixLength(right);
            if (commonPrefixLength != 0)
            {
                if (commonPrefixLength == left.Length && commonPrefixLength == right.Length)
                {
                    diffs.Add(new Diff(DiffKind.NoChange, left.Range, right.Range));
                    return;
                }
                
                diffs.Add(new Diff(DiffKind.NoChange, new Range(left.Range.StartIndex, commonPrefixLength),
                    new Range(right.Range.StartIndex, commonPrefixLength)));
            }

            int commonSuffixLength = left.Extract(commonPrefixLength).FindCommonSuffixLength(right.Extract(commonPrefixLength));

            // Now work on the middle part.
            Substring leftMiddle = left.Extract(commonPrefixLength, left.Length - commonPrefixLength - commonSuffixLength);
            Substring rightMiddle = right.Extract(commonPrefixLength, right.Length - commonPrefixLength - commonSuffixLength);
            SlowDiff(diffs, leftMiddle, rightMiddle, boundRuntime);

            // Tack on the final diff for the common suffix, if any.
            if (commonSuffixLength != 0)
            {
                diffs.Add(new Diff(DiffKind.NoChange,
                    new Range(leftMiddle.Range.EndIndex, commonSuffixLength),
                    new Range(rightMiddle.Range.EndIndex, commonSuffixLength)));
            }
        }

        private static void SlowDiff(IList<Diff> diffs, Substring left, Substring right, bool boundRuntime)
        {
            DiffAlgorithm.PopulateDiffs(diffs, left, right, boundRuntime);
        }

        private static void CanonicalizeDiffs(IList<Diff> diffs)
        {
            for (int i = 0; i < diffs.Count; i++)
            {
                Diff curDiff = diffs[i];

                if (curDiff.LeftRange.Length == 0 && curDiff.RightRange.Length == 0)
                {
                    diffs.RemoveAt(i);
                }
                else if (i != 0)
                {
                    Diff prevDiff = diffs[i - 1];
                    if (prevDiff.Kind == curDiff.Kind)
                    {
                        diffs.RemoveAt(i);
                        i -= 1;
                        diffs[i] = new Diff(prevDiff.Kind,
                            prevDiff.LeftRange.ExtendWith(curDiff.LeftRange),
                            prevDiff.RightRange.ExtendWith(curDiff.RightRange));
                    }
                }
            }
        }

        private sealed class DiffAlgorithm
        {
            // Ordinarily the worst case runtime is O((N + M) * D) which can be very large
            // as D approaches N + M.  Here we attempt to limit the worst case to O(D ^ PowLimit)
            // when N * M is too big.
            private const double PowLimit = 1.5;

            // The value of N * M at which to start binding the runtime.
            // We want a value sufficiently high that we will get accurate diffs
            // for sequences that contain relatively a large number of adjacent differences
            // but not so big that it takes too long to run.
            //
            // Experimental results on completely different strings of identical size
            // with disjoint alphabets on Intel Core 2 Duo U7700, 1.33Ghz laptop.
            //
            //   N and M    Unbounded    Bounded
            //    1000        169ms         16ms
            //    2000        615ms         40ms
            //    3000       1545ms         66ms
            //    4000       2481ms         91ms
            //    5000       4416ms        169ms
            //    6000       6189ms        210ms
            //    7000       7430ms        267ms
            //    8000      10337ms        293ms
            //    9000      13661ms        381ms
            //   10000      17579ms        464ms
            //
            // The Eclipse implementation uses a limit of 10,000,000 which in our chart would
            // mean applying to runtime bounded approximations at a problem size of about 3162.
            // Still rather slow and there are plenty of slower machines out there.
            //
            // So instead we bound the problem size to 2500 ^ 2 = 6,250,000 for now.
            private const long TooLong = 6250000;

            // The maximum number of non-diagonal edits (differences) to consider.
            private readonly int max;

            // Each of these represents a "V" vector from Myers' algorithm which allows signed integer indices in
            // a range -MAX .. MAX.  We must add the offset "max" to find the center of the vector.
            private readonly int[] leftVector;
            private readonly int[] rightVector;

            private readonly IList<Diff> diffs;
            private int commonSeqLeftStartIndex, commonSeqRightStartIndex, commonSeqLength;

            private DiffAlgorithm(IList<Diff> diffs, int leftStartIndex, int rightStartIndex, int max)
            {
                this.diffs = diffs;
                this.commonSeqLeftStartIndex = leftStartIndex;
                this.commonSeqRightStartIndex = rightStartIndex;
                this.max = max;

                int vectorLength = max * 2 + 1;
                leftVector = new int[vectorLength];
                rightVector = new int[vectorLength];
            }

            public static void PopulateDiffs(IList<Diff> diffs, Substring left, Substring right, bool boundRuntime)
            {
                int n = left.Length;
                int m = right.Length;

                int max = CeilNPlusMOverTwo(n, m);
                if (boundRuntime && ((long) n) * ((long) m) > TooLong)
                    max = (int) Math.Pow(max, PowLimit - 1.0);

                DiffAlgorithm algorithm = new DiffAlgorithm(diffs, left.Range.StartIndex, right.Range.StartIndex, max);

                algorithm.ComputeLCS(left, right);
                algorithm.FlushDiffs(left.Range.EndIndex, right.Range.EndIndex);
            }

            private void EmitDiffsFromCommonSequence(int leftIndex, int rightIndex, int length)
            {
                if (length == 0)
                    return;

                int commonSeqLeftEndIndex = commonSeqLeftStartIndex + commonSeqLength;
                int commonSeqRightEndIndex = commonSeqRightStartIndex + commonSeqLength;

                if (leftIndex == commonSeqLeftEndIndex && rightIndex == commonSeqRightEndIndex)
                {
                    commonSeqLength += length;
                }
                else
                {
                    if (commonSeqLength != 0)
                        diffs.Add(new Diff(DiffKind.NoChange,
                            new Range(commonSeqLeftStartIndex, commonSeqLength),
                            new Range(commonSeqRightStartIndex, commonSeqLength)));

                    diffs.Add(new Diff(DiffKind.Change,
                        new Range(commonSeqLeftEndIndex, leftIndex - commonSeqLeftEndIndex),
                        new Range(commonSeqRightEndIndex, rightIndex - commonSeqRightEndIndex)));

                    commonSeqLeftStartIndex = leftIndex;
                    commonSeqRightStartIndex = rightIndex;
                    commonSeqLength = length;
                }
            }

            private void FlushDiffs(int leftEndIndex, int rightEndIndex)
            {
                int commonSeqLeftEndIndex = commonSeqLeftStartIndex + commonSeqLength;
                int commonSeqRightEndIndex = commonSeqRightStartIndex + commonSeqLength;

                if (commonSeqLength != 0)
                    diffs.Add(new Diff(DiffKind.NoChange,
                        new Range(commonSeqLeftStartIndex, commonSeqLength),
                        new Range(commonSeqRightStartIndex, commonSeqLength)));

                if (leftEndIndex != commonSeqLeftEndIndex || rightEndIndex != commonSeqRightEndIndex)
                    diffs.Add(new Diff(DiffKind.Change,
                        new Range(commonSeqLeftEndIndex, leftEndIndex - commonSeqLeftEndIndex),
                        new Range(commonSeqRightEndIndex, rightEndIndex - commonSeqRightEndIndex)));
            }

            /// <summary>
            /// <para>
            /// Determines the longest common subsequence between two sequences and populates the
            /// list of diffs derived from the result as we go.  Each recursive step identifies
            /// a middle "snake" (a common sequence) then splits the problem until nothing remains.
            /// </para>
            /// </summary>
            /// <param name="left">The sequence "A"</param>
            /// <param name="right">The sequence "B"</param>
            private void ComputeLCS(Substring left, Substring right)
            {
                int n = left.Length;
                int m = right.Length;

                if (n != 0 && m != 0)
                {
                    int middleSnakeLeftStartIndex, middleSnakeRightStartIndex, middleSnakeLength;
                    int ses = FindMiddleSnake(left, right, out middleSnakeLeftStartIndex, out middleSnakeRightStartIndex, out middleSnakeLength);
                    if (ses > 1)
                    {
                        // If SES >= 2 then the edit script includes at least 2 differences, so we divide the problem.
                        ComputeLCS(
                            left.Extract(0, middleSnakeLeftStartIndex),
                            right.Extract(0, middleSnakeRightStartIndex));

                        EmitDiffsFromCommonSequence(
                            left.Range.StartIndex + middleSnakeLeftStartIndex,
                            right.Range.StartIndex + middleSnakeRightStartIndex,
                            middleSnakeLength);

                        ComputeLCS(
                            left.Extract(middleSnakeLeftStartIndex + middleSnakeLength),
                            right.Extract(middleSnakeRightStartIndex + middleSnakeLength));
                    }
                    else
                    {
                        // If SES = 1, then exactly one symbol needs to be added or deleted from either sequence.
                        // If SES = 0, then both sequences are equal.

                        if (ses != 0)
                        {
                            // The middle snake is the common part after the change so we just need to grab the
                            // common part before the change.
                            EmitDiffsFromCommonSequence(
                                left.Range.StartIndex,
                                right.Range.StartIndex,
                                Math.Min(middleSnakeLeftStartIndex, middleSnakeRightStartIndex));
                        }

                        EmitDiffsFromCommonSequence(
                            left.Range.StartIndex + middleSnakeLeftStartIndex,
                            right.Range.StartIndex + middleSnakeRightStartIndex,
                            middleSnakeLength);
                    }
                }
            }

            /// <summary>
            /// <para>
            /// Finds a middle "snake", which is a (possibly empty) sequence of diagonal edges in the edit
            /// graph.  Thus it directly represents a common sequence.
            /// </para>
            /// <para>
            /// In essence, this function searches D-paths forward and backward in the sequence until it
            /// finds the middle snake.  The middle snake informs us about a common sequence sandwiched
            /// between two other sequences that may contain changes.  By definition, the left and right
            /// middle snakes must be of equal length.
            /// </para>
            /// </summary>
            /// <param name="left">The sequence "A"</param>
            /// <param name="right">The sequence "B"</param>
            /// <param name="middleSnakeLeftStartIndex">The starting index of the middle snake in "A"</param>
            /// <param name="middleSnakeRightStartIndex">The starting index of the middle snake in "B"</param>
            /// <param name="middleSnakeLength">The middle snake length</param>
            /// <returns>The length of the shorted edit script between "A" and "B"</returns>
            private int FindMiddleSnake(Substring left, Substring right, out int middleSnakeLeftStartIndex, out int middleSnakeRightStartIndex, out int middleSnakeLength)
            {
                int n = left.Length;
                int m = right.Length;

                int delta = n - m;
                bool isDeltaOdd = (delta & 1) != 0;

                leftVector[max + 1] = 0;
                rightVector[max - 1] = n;

                int end = Math.Min(CeilNPlusMOverTwo(n, m), max);
                for (int d = 0; d <= end; d++)
                {
                    // Search forward D-paths.
                    for (int k = -d; k <= d; k += 2)
                    {
                        // Find the end of the furthest reaching forward D-path in diagonal k.
                        int x = k == -d || k != d && leftVector[max + k - 1] < leftVector[max + k + 1]
                            ? leftVector[max + k + 1]
                            : leftVector[max + k - 1] + 1;

                        int origX = x;
                        for (int y = x - k; x < n && y < m && left[x] == right[y]; )
                        {
                            x += 1;
                            y += 1;
                        }

                        leftVector[max + k] = x;

                        // If the D-path is feasible and overlaps the furthest reaching reverse (D-1)-Path in diagonal k
                        // then we have found the middle snake.
                        if (isDeltaOdd && k >= delta - d + 1 && k <= delta + d - 1)
                        {
                            int u = rightVector[max + k - delta];
                            if (x >= u)
                            {
                                middleSnakeLeftStartIndex = origX;
                                middleSnakeRightStartIndex = origX - k;
                                middleSnakeLength = x - origX;
                                return d * 2 - 1;
                            }
                        }
                    }

                    // Search reverse D-paths.
                    for (int k = -d; k <= d; k += 2)
                    {
                        // Find the end of the furthest reaching reverse D-path in diagonal k + delta.
                        int u = k == d || k != -d && rightVector[max + k - 1] < rightVector[max + k + 1]
                            ? rightVector[max + k - 1]
                            : rightVector[max + k + 1] - 1;

                        int kPlusDelta = k + delta;
                        int origU = u;
                        int v;
                        for (v = u - kPlusDelta; u > 0 && v > 0 && left[u - 1] == right[v - 1]; )
                        {
                            u -= 1;
                            v -= 1;
                        }

                        rightVector[max + k] = u;

                        // If the D-path is feasible and overlaps the furthest reaching forward D-Path in diagonal k
                        // then we have found the middle snake.
                        if (!isDeltaOdd && kPlusDelta >= -d && kPlusDelta <= d)
                        {
                            int x = leftVector[max + kPlusDelta];
                            if (u <= x)
                            {
                                middleSnakeLeftStartIndex = u;
                                middleSnakeRightStartIndex = v;
                                middleSnakeLength = origU - u;
                                return d * 2;
                            }
                        }
                    }
                }

                // We have exceeded the maximum effort we are willing to expend finding a diff.
                //
                // So we artificially divide the problem by finding the snakes in the forward / reverse
                // direction that have the most progress toward (N, M) / (0, 0).  These are the
                // ones that maximize x + y / minimize u + v.  The snake we return will not actually
                // be the middle snake (since we haven't found it yet) but it will be good enough
                // to reduce the problem.
                //
                // These snakes all begin on the same diagonal as the others of equal
                // progress in the same direction.  As there may be several of them, we need a way
                // to decide which one to pursue.
                //
                // The Eclipse LCS implementation chooses the median of these snakes with respect to k.
                // Intuitively this is the one that is nearer the direct line between (0, 0) and (N, M)
                // so it has a good chance of forming a path with more balanced changes between A and B
                // than the snakes that consist of significantly more changes to A than B or vice-versa.
                // Consequently the median of theses snakes should yield a pretty good approximation. -- Jeff.

                int bestProgress = 0;
                Dictionary<int, bool> bestKs = new Dictionary<int,bool>(); // with the forward direction indicated by value true

                for (int k = -end; k <= end; k += 2)
                {
                    // Forward direction.
                    int x = leftVector[max + k];
                    int y = x - k;
                    if (x < n && y < m)
                    {
                        int progress = x + y;
                        if (progress >= bestProgress)
                        {
                            if (progress > bestProgress)
                            {
                                bestProgress = progress;
                                bestKs.Clear();
                            }
                            bestKs[k] = true;
                        }
                    }

                    // Reverse direction.
                    int u = rightVector[max + k];
                    int v = u - k - delta;
                    if (u >= 0 && v >= 0)
                    {
                        int progress = n + m - u - v;
                        if (progress >= bestProgress)
                        {
                            if (progress > bestProgress)
                            {
                                bestProgress = progress;
                                bestKs.Clear();
                            }
                            bestKs[k] = false;
                        }
                    }
                }

                int[] sortedKs = new int[bestKs.Count];
                bestKs.Keys.CopyTo(sortedKs, 0);
                Array.Sort(sortedKs);

                int medianK = sortedKs[sortedKs.Length / 2];

                if (bestKs[medianK])
                {
                    int x = leftVector[max + medianK];
                    int y = x - medianK;
                    middleSnakeLeftStartIndex = x;
                    middleSnakeRightStartIndex = y;
                }
                else
                {
                    int u = rightVector[max + medianK];
                    int v = u - medianK - delta;
                    middleSnakeLeftStartIndex = u;
                    middleSnakeRightStartIndex = v;
                }

                middleSnakeLength = 0;

                // We need to return the length of the shortest edit script but we don't actually know
                // what it is.  Fortunately the caller does not care as long as it's greater than 2, which
                // it must be since d > end >= max > 2.
                return int.MaxValue;
            }

            private static int CeilNPlusMOverTwo(int n, int m)
            {
                return (n + m + 1) / 2;
            }
        }
    }
}
