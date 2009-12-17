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
using System.Drawing;
using System.Linq;
using System.Text;
using Gallio.Common.Splash.Internal;
using MbUnit.Framework;

namespace Gallio.Common.Splash.Tests
{
    public class SplashDocumentTest
    {
        [Test]
        public void Constructor_CreatesAnEmptyDocument()
        {
            var document = new SplashDocument();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, document.CharCount);
                Assert.AreEqual(1, document.ParagraphCount);
                Assert.AreEqual(1, document.StyleCount);
                Assert.AreEqual(0, document.ObjectCount);
                Assert.AreEqual(0, document.RunCount);
                Assert.AreEqual("", document.ToString());
            });
        }

        [Test]
        public void Clear_EmptiesTheDocumentAndRaisesEvent()
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            bool documentClearedEventRaised = false;
            document.DocumentCleared += (sender, e) => documentClearedEventRaised = true;
            document.Clear();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, document.CharCount);
                Assert.AreEqual(1, document.ParagraphCount);
                Assert.AreEqual(1, document.StyleCount);
                Assert.AreEqual(0, document.ObjectCount);
                Assert.AreEqual(0, document.RunCount);
                Assert.AreEqual("", document.ToString());

                Assert.IsTrue(documentClearedEventRaised);
            });
        }

        [Test]
        public void GetTextRange_WhenStartIndexNegative_Throws()
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetTextRange(-1, 1));
        }

        [Test]
        public void GetTextRange_WhenLengthNegative_Throws()
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetTextRange(0, -1));
        }

        [Test]
        public void GetTextRange_WhenStartIndexPlusLengthExceedsDocument_Throws()
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetTextRange(10, 3));
        }

        [Test]
        [Row(0, 12)]
        [Row(10, 2)]
        [Row(0, 4)]
        [Row(3, 7)]
        public void GetTextRange_WhenRangeValid_ReturnsRange(int startIndex, int length)
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            Assert.AreEqual("some text...".Substring(startIndex, length), document.GetTextRange(startIndex, length));
        }

        [Test]
        public void BeginStyle_WhenStyleIsNull_Throws()
        {
            var document = new SplashDocument();

            Assert.Throws<ArgumentNullException>(() => document.BeginStyle(null));
        }

        [Test]
        public void EndStyle_WhenOnlyDefaultStyleOnStack_Throws()
        {
            var document = new SplashDocument();

            Assert.Throws<InvalidOperationException>(() => document.EndStyle());
        }

        [Test]
        public void BeginStyleAndEndStyle_PushAndPopStylesOnStack()
        {
            var document = new SplashDocument();
            var style1 = new StyleBuilder() { Color = Color.Red }.ToStyle();
            var style2 = new StyleBuilder() { Color = Color.Green }.ToStyle();
            var style3 = new StyleBuilder() { Color = Color.Blue }.ToStyle();

            Assert.AreEqual(Style.Default, document.CurrentStyle);

            document.BeginStyle(style1);
            Assert.AreEqual(style1, document.CurrentStyle);

            document.EndStyle();
            Assert.AreEqual(Style.Default, document.CurrentStyle);

            using (document.BeginStyle(style2))
            {
                Assert.AreEqual(style2, document.CurrentStyle);

                using (document.BeginStyle(style3))
                {
                    Assert.AreEqual(style3, document.CurrentStyle);
                }

                Assert.AreEqual(style2, document.CurrentStyle);
            }

            Assert.AreEqual(Style.Default, document.CurrentStyle);
        }

        [Test]
        public void BeginStyle_CoalescesEqualStyles()
        {
            var document = new SplashDocument();
            var style = new StyleBuilder().ToStyle(); // equal to default style

            document.BeginStyle(style);
            Assert.AreSame(Style.Default, document.CurrentStyle);

            document.EndStyle();
            Assert.AreSame(Style.Default, document.CurrentStyle);
        }

        [Test]
        public void GetStyleAtIndex_WhenIndexNegative_Throws()
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetStyleAtIndex(-1));
        }

        [Test]
        public void GetStyleAtIndex_WhenIndexBeyondDocument_Throws()
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetStyleAtIndex(12));
        }

        [Test]
        public void GetStyleAtIndex_WhenIndexValid_ReturnsStyleAtIndex()
        {
            var document = new SplashDocument();
            var style = new StyleBuilder() { Color = Color.Red }.ToStyle();
            document.AppendText("some ");
            using (document.BeginStyle(style))
                document.AppendText("text");
            document.AppendText("...");

            Assert.Multiple(() =>
            {
                for (int i = 0; i < 5; i++)
                    Assert.AreEqual(Style.Default, document.GetStyleAtIndex(i));
                for (int i = 5; i < 9; i++)
                    Assert.AreEqual(style, document.GetStyleAtIndex(i));
                for (int i = 9; i < 12; i++)
                    Assert.AreEqual(Style.Default, document.GetStyleAtIndex(i));
            });
        }

        [Test]
        public void EndAnnotation_WhenNoCurrentAnnotation_Throws()
        {
            var document = new SplashDocument();
            var key = new Key<string>("href");

            // 1st case: no annotation table exists
            Assert.Throws<InvalidOperationException>(() => document.EndAnnotation(key));

            // 2nd case: annotation table exists but there is no current annotation
            document.BeginAnnotation(key, "value");
            document.EndAnnotation(key);
            Assert.Throws<InvalidOperationException>(() => document.EndAnnotation(key));
        }

        [Test]
        public void BeginAnnotationAndEndAnnotation_PushAndPopAnnotationsOnStack()
        {
            var document = new SplashDocument();
            var key = new Key<string>("href");
            string value;

            Assert.IsFalse(document.TryGetCurrentAnnotation(key, out value));
            Assert.IsNull(value);

            document.BeginAnnotation(key, "a");
            Assert.IsTrue(document.TryGetCurrentAnnotation(key, out value));
            Assert.AreEqual("a", value);

            document.EndAnnotation(key);
            Assert.IsFalse(document.TryGetCurrentAnnotation(key, out value));
            Assert.IsNull(value);

            using (document.BeginAnnotation(key, "b"))
            {
                Assert.IsTrue(document.TryGetCurrentAnnotation(key, out value));
                Assert.AreEqual("b", value);

                using (document.BeginAnnotation(key, "c"))
                {
                    Assert.IsTrue(document.TryGetCurrentAnnotation(key, out value));
                    Assert.AreEqual("c", value);
                }

                Assert.IsTrue(document.TryGetCurrentAnnotation(key, out value));
                Assert.AreEqual("b", value);
            }

            Assert.IsFalse(document.TryGetCurrentAnnotation(key, out value));
            Assert.IsNull(value);
        }

        [Test]
        public void TryGetAnnotationAtIndex_WhenIndexNegative_Throws()
        {
            var document = new SplashDocument();
            var key = new Key<string>("href");
            string value;
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.TryGetAnnotationAtIndex(key, -1, out value));
        }

        [Test]
        public void TryGetAnnotationAtIndex_WhenIndexBeyondDocument_Throws()
        {
            var document = new SplashDocument();
            var key = new Key<string>("href");
            string value;
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.TryGetAnnotationAtIndex(key, 12, out value));
        }

        [Test]
        public void TryGetAnnotationAtIndex_WhenIndexValidAndNoAnnotations_ReturnsFalse()
        {
            var document = new SplashDocument();
            var key = new Key<string>("href");

            document.AppendText("foo");

            string value;
            Assert.IsFalse(document.TryGetAnnotationAtIndex(key, 0, out value));
            Assert.IsNull(value);
        }

        [Test]
        public void TryGetAnnotationAtIndex_WhenIndexValidAndAtLeastOneAnnotation_ReturnsAnnotationAtIndex()
        {
            var document = new SplashDocument();
            var key = new Key<string>("href");

            // begin/end an empty block of annotations, these will get stripped out
            document.BeginAnnotation(key, "empty");
            document.BeginAnnotation(key, "empty2");
            document.EndAnnotation(key);
            document.EndAnnotation(key);

            // add some unannotated text
            document.AppendText("some ");

            // add some annotated text
            using (document.BeginAnnotation(key, "a"))
            {
                document.AppendText("text ");

                using (document.BeginAnnotation(key, "b"))
                    document.AppendText("more");
            }

            // add some unannotated text
            document.AppendText("...");

            // begin another empty annotation at the end, should have no effect on current text
            document.BeginAnnotation(key, "trailer");

            Assert.Multiple(() =>
            {
                string value;
                for (int i = 0; i < 5; i++)
                {
                    Assert.IsFalse(document.TryGetAnnotationAtIndex(key, i, out value));
                    Assert.IsNull(value);
                }
                for (int i = 5; i < 10; i++)
                {
                    Assert.IsTrue(document.TryGetAnnotationAtIndex(key, i, out value));
                    Assert.AreEqual("a", value);
                }
                for (int i = 10; i < 14; i++)
                {
                    Assert.IsTrue(document.TryGetAnnotationAtIndex(key, i, out value));
                    Assert.AreEqual("b", value);
                }
                for (int i = 14; i < 17; i++)
                {
                    Assert.IsFalse(document.TryGetAnnotationAtIndex(key, i, out value));
                    Assert.IsNull(value);
                }
            });
        }

        [Test]
        public void GetObjectAtIndex_WhenIndexNegative_Throws()
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetObjectAtIndex(-1));
        }

        [Test]
        public void GetObjectAtIndex_WhenIndexBeyondDocument_Throws()
        {
            var document = new SplashDocument();
            document.AppendText("some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetObjectAtIndex(12));
        }

        [Test]
        public void GetObjectAtIndex_WhenIndexValid_ReturnsEmbeddedObject()
        {
            var document = new SplashDocument();
            var obj = new EmbeddedImage(new Bitmap(16, 16));
            document.AppendText("obj");
            document.AppendObject(obj);
            document.AppendText("...");

            Assert.Multiple(() =>
            {
                for (int i = 0; i < 3; i++)
                    Assert.IsNull(document.GetObjectAtIndex(i));
                Assert.AreEqual(obj, document.GetObjectAtIndex(3));
                for (int i = 4; i < 7; i++)
                    Assert.IsNull(document.GetObjectAtIndex(i));
            });
        }

        [Test]
        public void AppendText_WhenTextIsNull_Throws()
        {
            var document = new SplashDocument();
            Assert.Throws<ArgumentNullException>(() => document.AppendText(null));
        }

        [Test]
        public void AppendObject_WhenObjectIsNull_Throws()
        {
            var document = new SplashDocument();
            Assert.Throws<ArgumentNullException>(() => document.AppendObject(null));
        }

        [Test]
        public unsafe void AppendStuff()
        {
            var style1 = new StyleBuilder() { Color = Color.Red }.ToStyle();
            var style2 = new StyleBuilder() { LeftMargin = 10, RightMargin = 10 }.ToStyle();
            var style3 = new StyleBuilder() { Font = SystemFonts.SmallCaptionFont, Color = Color.Blue }.ToStyle();
            var embeddedObject = new EmbeddedImage(new Bitmap(16, 16));

            var document = new SplashDocument();
            var changedParagraphIndices = new List<int>();

            document.ParagraphChanged += (sender, e) => changedParagraphIndices.Add(e.ParagraphIndex);

            using (document.BeginStyle(style1))
                document.AppendText("Some text, lalala.\nMore text.");

            using (document.BeginStyle(style2))
            {
                document.AppendText("Tab\t.\n");

                document.AppendText("\0\r"); // these control characters will be discarded

                using (document.BeginStyle(style3))
                {
                    document.AppendLine();
                    document.AppendText(""); // to verify that no change event is raised for empty text
                }

                document.AppendText("(");
                document.AppendObject(embeddedObject);
                document.AppendText(")");
            }

            Assert.Multiple(() =>
            {
                // Check char content.
                Assert.AreEqual("Some text, lalala.\nMore text.Tab\t.\n\n( )", document.ToString());

                // Check style table.
                Assert.AreEqual(4, document.StyleCount);
                Assert.AreEqual(Style.Default, document.LookupStyle(0));
                Assert.AreEqual(style1, document.LookupStyle(1));
                Assert.AreEqual(style2, document.LookupStyle(2));
                Assert.AreEqual(style3, document.LookupStyle(3));

                // Check object table.
                Assert.AreEqual(1, document.ObjectCount);
                Assert.AreEqual(embeddedObject, document.LookupObject(0));

                // Check paragraph table.
                Assert.AreEqual(4, document.ParagraphCount);
                Paragraph* paragraphs = document.GetParagraphZero();

                Assert.AreEqual(0, paragraphs[0].CharIndex); // "Some text, lalala.\n"
                Assert.AreEqual(19, paragraphs[0].CharCount);
                Assert.AreEqual(0, paragraphs[0].RunIndex);
                Assert.AreEqual(1, paragraphs[0].RunCount);

                Assert.AreEqual(19, paragraphs[1].CharIndex); // "More text.Tab\t.\n"
                Assert.AreEqual(16, paragraphs[1].CharCount);
                Assert.AreEqual(1, paragraphs[1].RunIndex);
                Assert.AreEqual(4, paragraphs[1].RunCount);

                Assert.AreEqual(35, paragraphs[2].CharIndex); // "\n"
                Assert.AreEqual(1, paragraphs[2].CharCount);
                Assert.AreEqual(5, paragraphs[2].RunIndex);
                Assert.AreEqual(1, paragraphs[2].RunCount);

                Assert.AreEqual(36, paragraphs[3].CharIndex); // "( )"
                Assert.AreEqual(3, paragraphs[3].CharCount);
                Assert.AreEqual(6, paragraphs[3].RunIndex);
                Assert.AreEqual(3, paragraphs[3].RunCount);

                // Check run table.
                Assert.AreEqual(9, document.RunCount);
                Run* runs = document.GetRunZero();

                Assert.AreEqual(RunKind.Text, runs[0].RunKind); // "Some text, lalala.\n"
                Assert.AreEqual(19, runs[0].CharCount);
                Assert.AreEqual(1, runs[0].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[1].RunKind); // "More text."
                Assert.AreEqual(10, runs[1].CharCount);
                Assert.AreEqual(1, runs[1].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[2].RunKind); // "Tab"
                Assert.AreEqual(3, runs[2].CharCount);
                Assert.AreEqual(2, runs[2].StyleIndex);

                Assert.AreEqual(RunKind.Tab, runs[3].RunKind); // "\t"
                Assert.AreEqual(1, runs[3].CharCount);
                Assert.AreEqual(2, runs[3].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[4].RunKind); // ".\n"
                Assert.AreEqual(2, runs[4].CharCount);
                Assert.AreEqual(2, runs[4].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[5].RunKind); // "\n"
                Assert.AreEqual(1, runs[5].CharCount);
                Assert.AreEqual(3, runs[5].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[6].RunKind); // "("
                Assert.AreEqual(1, runs[6].CharCount);
                Assert.AreEqual(2, runs[6].StyleIndex);

                Assert.AreEqual(RunKind.Object, runs[7].RunKind); // "("
                Assert.AreEqual(1, runs[7].CharCount);
                Assert.AreEqual(2, runs[7].StyleIndex);
                Assert.AreEqual(0, runs[7].ObjectIndex);

                Assert.AreEqual(RunKind.Text, runs[8].RunKind); // ")"
                Assert.AreEqual(1, runs[8].CharCount);
                Assert.AreEqual(2, runs[8].StyleIndex);

                // Check that paragraph changed notifications were raised as needed.
                Assert.AreElementsEqual(new[] { 0, 1, 2, 2, 3, 3, 3 }, changedParagraphIndices);
            });
        }

        [Test]
        [Row(SplashDocument.MaxCharsPerRun)]
        [Row(SplashDocument.MaxCharsPerRun + 1)]
        [Row(SplashDocument.MaxCharsPerRun * 2)]
        [Row(SplashDocument.MaxCharsPerRun * 2 + 1)]
        public unsafe void AppendText_WhenRunIsExtremelyLong_SplitsRun(int length)
        {
            var document = new SplashDocument();
            var content = new string(' ', length);

            document.AppendText(content);

            Assert.Multiple(() =>
            {
                // Check char content.
                Assert.AreEqual(content, document.ToString());

                // Check paragraph table.
                Assert.AreEqual(1, document.ParagraphCount);
                Paragraph* paragraphs = document.GetParagraphZero();

                int expectedRuns = (length + SplashDocument.MaxCharsPerRun - 1) / SplashDocument.MaxCharsPerRun;
                Assert.AreEqual(0, paragraphs[0].CharIndex);
                Assert.AreEqual(length, paragraphs[0].CharCount);
                Assert.AreEqual(0, paragraphs[0].RunIndex);
                Assert.AreEqual(expectedRuns, paragraphs[0].RunCount);

                // Check run table.
                Assert.AreEqual(expectedRuns, document.RunCount);
                Run* runs = document.GetRunZero();

                for (int i = 0; i < expectedRuns; i++)
                {
                    Assert.AreEqual(RunKind.Text, runs[i].RunKind);
                    Assert.AreEqual(Math.Min(length, SplashDocument.MaxCharsPerRun), runs[i].CharCount);
                    Assert.AreEqual(0, runs[i].StyleIndex);

                    length -= SplashDocument.MaxCharsPerRun;
                }
            });
        }
    }
}
