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
using System.Linq;
using System.Text;
using Gallio.Collections;
using Gallio.Framework.Text;
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
                DiffSet diffSet = DiffSet.GetDiffSet("1", "2", optimize);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 1))
                }, diffSet.Diffs);
            }

            [Test]
            public void SingleCharInsert([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("", "1", optimize);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 0), new Range(0, 1))
                }, diffSet.Diffs);
            }

            [Test]
            public void SingleCharDelete([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("1", "", optimize);
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
                NewAssert.AreEqual(new Diff[] {
                    new Diff(DiffKind.Change, new Range(0, 1), new Range(0, 0))
                }, diffSet.Diffs);
            }

            [Test]
            public void InsertAtHeadAndDeleteAtTail([Column(false, true)] bool optimize)
            {
                DiffSet diffSet = DiffSet.GetDiffSet("234", "123", optimize);
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
                DiffSet diffSet = DiffSet.GetDiffSet("123", "234", optimize);
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
                DiffSet diffSet = DiffSet.GetDiffSet("123abcdef", "abc456def", optimize);
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
            [Test, Pending]
            public void RunTimeIsBoundedForPathologicalCaseWithNoCommonalities()
            {
                StringBuilder left = new StringBuilder();
                for (int i = 0; i < 10000; i++)
                    left.Append((char)i);

                StringBuilder right = new StringBuilder();
                for (int i = 0; i < 10000; i++)
                    right.Append((char)i + 10000);

                DiffSet diffSet = DiffSet.GetDiffSet(left.ToString(), right.ToString());
                Assert.IsFalse(diffSet.IsEmpty);
                Assert.IsTrue(diffSet.ContainsChanges);
            }
        }
    }
}
