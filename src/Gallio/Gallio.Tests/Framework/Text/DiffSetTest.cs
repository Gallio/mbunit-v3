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
using System.Diagnostics;
using System.Text;
using Gallio.Framework.Text;
using Gallio.Model.Logging;
using Gallio.Model.Logging.Tags;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Text
{
    [TestsOn(typeof(DiffSet))]
    public class DiffSetTest
    {
        [Test]
        public void ContainsDocumentAndDiffs()
        {
            DiffSet diffSet = new DiffSet(new[] { new Diff(DiffKind.Change, new Range(0, 3), new Range(0, 3)) }, "abc", "def");
            Assert.AreEqual("abc", diffSet.LeftDocument);
            Assert.AreEqual("def", diffSet.RightDocument);
            NewAssert.AreEqual(new[] { new Diff(DiffKind.Change, new Range(0, 3), new Range(0, 3)) }, diffSet.Diffs);
        }

        [Test, ExpectedArgumentNullException]
        public void LeftDocumentCannotBeNull()
        {
            new DiffSet(new Diff[] { }, null, "");
        }

        [Test, ExpectedArgumentNullException]
        public void RightDocumentCannotBeNull()
        {
            new DiffSet(new Diff[] { }, "", null);
        }

        [Test, ExpectedArgumentNullException]
        public void DiffListCannotBeNull()
        {
            new DiffSet(null, "", "");
        }

        [Test, ExpectedArgumentException]
        public void DiffListCannotBeEmptyWhenDocumentsAreNot()
        {
            new DiffSet(new Diff[] { }, "abc", "def");
        }

        [Test]
        [Row(0, 3, 0, 2, ExpectedException=typeof(ArgumentException))]
        [Row(0, 2, 0, 4, ExpectedException=typeof(ArgumentException))]
        [Row(1, 2, 0, 4, ExpectedException=typeof(ArgumentException))]
        [Row(0, 3, 1, 3, ExpectedException=typeof(ArgumentException))]
        [Row(1, 3, 0, 4, ExpectedException=typeof(ArgumentException))]
        [Row(0, 3, 1, 4, ExpectedException=typeof(ArgumentException))]
        [Row(0, 3, 0, 4)]
        public void DiffListMustCoverBothDocuments(int x, int y, int u, int v)
        {
            new DiffSet(new Diff[] { new Diff(DiffKind.Change, new Range(x, y), new Range(u, v)) }, "abc", "defg");
        }

        [Test, ExpectedArgumentException]
        public void DiffListMustNotContainGaps()
        {
            new DiffSet(new Diff[] { new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 1)),
                new Diff(DiffKind.Change, new Range(2, 1), new Range(2, 2)) }, "abc", "dbfg");
        }

        [Test, ExpectedArgumentException]
        public void DiffListMustNotContainOverlappingSegments()
        {
            new DiffSet(new Diff[] { new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 1)),
                new Diff(DiffKind.Change, new Range(0, 3), new Range(2, 2)) }, "abc", "dbfg");
        }

        [Test]
        public void DiffCanConsistOfMultipleContiguousSegments()
        {
            new DiffSet(new Diff[] { new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 1)),
                new Diff(DiffKind.NoChange, new Range(1, 1), new Range(1, 1)),
                new Diff(DiffKind.Change, new Range(2, 1), new Range(2, 2)) }, "abc", "dbfg");
        }

        [Test, ExpectedArgumentNullException]
        public void GetDiffSetThrowsWhenLeftDocumentIsNull()
        {
            DiffSet.GetDiffSet(null, "");
        }

        [Test, ExpectedArgumentNullException]
        public void GetDiffSetThrowsWhenRightDocumentIsNull()
        {
            DiffSet.GetDiffSet("", null);
        }

        [Test]
        public void WriteAnnotatedLeftDocumentToThrowsWhenWriterIsNull()
        {
            DiffSet diffSet = new DiffSet(new Diff[] { }, "", "");
            NewAssert.Throws<ArgumentNullException>(() => diffSet.WriteAnnotatedLeftDocumentTo(null));
        }

        [Test]
        public void WriteAnnotatedRightDocumentToThrowsWhenWriterIsNull()
        {
            DiffSet diffSet = new DiffSet(new Diff[] { }, "", "");
            NewAssert.Throws<ArgumentNullException>(() => diffSet.WriteAnnotatedRightDocumentTo(null));
        }

        [Test]
        public void WriteAnnotatedLeftDocumentTo()
        {
            DiffSet diffSet = new DiffSet(new Diff[] { 
                new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 1)), // change
                new Diff(DiffKind.NoChange, new Range(1, 1), new Range(1, 1)), // same
                new Diff(DiffKind.Change, new Range(2, 1), new Range(2, 0)), // deletion
                new Diff(DiffKind.NoChange, new Range(3, 1), new Range(2, 1)), // same
                new Diff(DiffKind.Change, new Range(4, 0), new Range(3, 1)), // addition
            }, "acde", "bcef");

            StructuredTextWriter writer = new StructuredTextWriter();
            diffSet.WriteAnnotatedLeftDocumentTo(writer);

            NewAssert.AreEqual(new StructuredText(new BodyTag()
            {
                Contents = {
                    new MarkerTag(Marker.DiffChange) { Contents = { new TextTag("a") } },
                    new TextTag("c"),
                    new MarkerTag(Marker.DiffDeletion) { Contents = { new TextTag("d") } },
                    new TextTag("e")
                }
            }), writer.ToStructuredText());
        }

        [Test]
        public void WriteAnnotatedRightDocumentTo()
        {
            DiffSet diffSet = new DiffSet(new Diff[] { 
                new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 1)), // change
                new Diff(DiffKind.NoChange, new Range(1, 1), new Range(1, 1)), // same
                new Diff(DiffKind.Change, new Range(2, 1), new Range(2, 0)), // deletion
                new Diff(DiffKind.NoChange, new Range(3, 1), new Range(2, 1)), // same
                new Diff(DiffKind.Change, new Range(4, 0), new Range(3, 1)), // addition
            }, "acde", "bcef");

            StructuredTextWriter writer = new StructuredTextWriter();
            diffSet.WriteAnnotatedRightDocumentTo(writer);

            NewAssert.AreEqual(new StructuredText(new BodyTag()
            {
                Contents = {
                    new MarkerTag(Marker.DiffChange) { Contents = { new TextTag("b") } },
                    new TextTag("ce"),
                    new MarkerTag(Marker.DiffAddition) { Contents = { new TextTag("f") } }
                }
            }), writer.ToStructuredText());
        }

        public class WhenDocumentsAreEmpty
        {
            [Test]
            public void ComputedDiffIsEmpty()
            {
                DiffSet diffSet = DiffSet.GetDiffSet("", "");
                Assert.IsTrue(diffSet.IsEmpty);
                Assert.IsFalse(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] { }, diffSet.Diffs);
            }
        }

        public class WhenDocumentsAreEqualy
        {
            [Test]
            public void NoChangeSpansBothDocuments()
            {
                DiffSet diffSet = DiffSet.GetDiffSet("this is a test", "this is a test");
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsFalse(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] { new Diff(DiffKind.NoChange, new Range(0, 14), new Range(0, 14)) }, diffSet.Diffs);
            }
        }

        public class WhenExactlyOneDocumentIsEmpty
        {
            [Test]
            public void IfLeftDocumentIsEmptyChangeSpansRightDocument()
            {
                DiffSet diffSet = DiffSet.GetDiffSet("", "abcde");
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] { new Diff(DiffKind.Change, new Range(0, 0), new Range(0, 5)) },
                    diffSet.Diffs);
            }

            [Test]
            public void IfRightDocumentIsEmptyChangeSpansLeftDocument()
            {
                DiffSet diffSet = DiffSet.GetDiffSet("abcde", "");
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] { new Diff(DiffKind.Change, new Range(0, 5), new Range(0, 0)) },
                    diffSet.Diffs);
            }
        }

        public class WhenCommonPrefixOrSuffixIsFound
        {
            [Test]
            public void ProblemSizeIsReducedButDiffOffsetsAreStillCorrect()
            {
                DiffSet diffSet = DiffSet.GetDiffSet("123abcZZdef45", "123uvZZxy45");
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.NoChange, new Range(0, 3), new Range(0, 3)),
                    new Diff(DiffKind.Change, new Range(3, 3), new Range(3, 2)),
                    new Diff(DiffKind.NoChange, new Range(6, 2), new Range(5, 2)),
                    new Diff(DiffKind.Change, new Range(8, 3), new Range(7, 2)),
                    new Diff(DiffKind.NoChange, new Range(11, 2), new Range(9, 2))
                }, diffSet.Diffs);
            }
        }

        public class WhenOptimizationUsedShouldYieldSameResultsAsWhenOptimizationIsUsed
        {
            [Test]
            public void SingleCharEdit([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("1", "2", optimize, true);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 1))
                }, diffSet.Diffs);
            }

            [Test]
            public void SingleCharInsert([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("", "1", optimize, true);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 0), new Range(0, 1))
                }, diffSet.Diffs);
            }

            [Test]
            public void SingleCharDelete([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("1", "", optimize, true);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 0))
                }, diffSet.Diffs);
            }

            [Test]
            public void InsertAtHeadAndDeleteAtTail([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("234", "123", optimize, true);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 0), new Range(0, 1)),
                    new Diff(DiffKind.NoChange, new Range(0, 2), new Range(1, 2)),
                    new Diff(DiffKind.Change, new Range(2, 1), new Range(3, 0))
                }, diffSet.Diffs);
            }

            [Test]
            public void DeleteAtHeadAndInsertAtTail([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("123", "234", optimize, true);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 0)),
                    new Diff(DiffKind.NoChange, new Range(1, 2), new Range(0, 2)),
                    new Diff(DiffKind.Change, new Range(3, 0), new Range(2, 1))
                }, diffSet.Diffs);
            }

            [Test]
            public void MultipleAdjacentChanges([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("123abcdef", "abc456def", optimize, true);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 3), new Range(0, 0)),
                    new Diff(DiffKind.NoChange, new Range(3, 3), new Range(0, 3)),
                    new Diff(DiffKind.Change, new Range(6, 0), new Range(3, 3)),
                    new Diff(DiffKind.NoChange, new Range(6, 3), new Range(6, 3))
                }, diffSet.Diffs);
            }
        }

        public class WhenDocumentsAreVeryLargeAndContainManyDifferences
        {
            [Test]
            public void RunTimeIsBoundedForPathologicalCaseWithNoCommonalities()
            {
                const int problemSize = 3000;

                long boundedMillis = RunWorstCaseDiff(problemSize, true);
                long unboundedMillis = RunWorstCaseDiff(problemSize, false);
                NewAssert.LessThan(boundedMillis, unboundedMillis, "The bounded approximated algorithm should be faster than the unbounded precise algorithm.");
            }

            private static long RunWorstCaseDiff(int problemSize, bool bounded)
            {
                StringBuilder left = new StringBuilder();
                for (int i = 0; i < problemSize; i++)
                    left.Append((char)i);

                StringBuilder right = new StringBuilder();
                for (int i = problemSize; i < problemSize * 2; i++)
                    right.Append((char)i);

                Stopwatch timer = Stopwatch.StartNew();
                DiffSet diffSet = DiffSet.GetDiffSet(left.ToString(), right.ToString(), false, bounded);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new[] { new Diff(DiffKind.Change, new Range(0, problemSize), new Range(0, problemSize)) }, diffSet.Diffs);
                return timer.ElapsedMilliseconds;
            }
        }
    }
}
