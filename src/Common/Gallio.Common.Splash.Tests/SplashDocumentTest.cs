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
                Assert.AreEqual(0, document.StyleCount);
                Assert.AreEqual(0, document.ObjectCount);
                Assert.AreEqual(0, document.RunCount);
                Assert.AreEqual("", document.ToString());
            });
        }

        [Test]
        public void Clear_EmptiesTheDocumentAndRaisesEvent()
        {
            var document = new SplashDocument();
            document.AppendText(Style.DefaultStyle, "some text...");

            bool documentClearedEventRaised = false;
            document.DocumentCleared += (sender, e) => documentClearedEventRaised = true;
            document.Clear();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(0, document.CharCount);
                Assert.AreEqual(1, document.ParagraphCount);
                Assert.AreEqual(0, document.StyleCount);
                Assert.AreEqual(0, document.ObjectCount);
                Assert.AreEqual(0, document.RunCount);
                Assert.AreEqual("", document.ToString());

                Assert.IsTrue(documentClearedEventRaised);
            });
        }

        [Test]
        public void GetText_WhenStartIndexNegative_Throws()
        {
            var document = new SplashDocument();
            document.AppendText(Style.DefaultStyle, "some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetText(-1, 1));
        }

        [Test]
        public void GetText_WhenLengthNegative_Throws()
        {
            var document = new SplashDocument();
            document.AppendText(Style.DefaultStyle, "some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetText(0, -1));
        }

        [Test]
        public void GetText_WhenStartIndexPlusLengthExceedsDocument_Throws()
        {
            var document = new SplashDocument();
            document.AppendText(Style.DefaultStyle, "some text...");

            Assert.Throws<ArgumentOutOfRangeException>(() => document.GetText(10, 3));
        }

        [Test]
        [Row(0, 12)]
        [Row(10, 2)]
        [Row(0, 4)]
        [Row(3, 7)]
        public void GetText_WhenRangeValid_ReturnsRange(int startIndex, int length)
        {
            var document = new SplashDocument();
            document.AppendText(Style.DefaultStyle, "some text...");

            Assert.AreEqual("some text...".Substring(startIndex, length), document.GetText(startIndex, length));
        }

        [Test]
        public void AppendText_WhenStyleIsNull_Throws()
        {
            var document = new SplashDocument();
            Assert.Throws<ArgumentNullException>(() => document.AppendText(null, ""));
        }

        [Test]
        public void AppendText_WhenTextIsNull_Throws()
        {
            var document = new SplashDocument();
            Assert.Throws<ArgumentNullException>(() => document.AppendText(Style.DefaultStyle, null));
        }

        [Test]
        public void AppendLine_WhenStyleIsNull_Throws()
        {
            var document = new SplashDocument();
            Assert.Throws<ArgumentNullException>(() => document.AppendLine(null));
        }

        [Test]
        public void AppendObject_WhenStyleIsNull_Throws()
        {
            var document = new SplashDocument();
            var embeddedObject = new EmbeddedImage(new Bitmap(16, 16));
            Assert.Throws<ArgumentNullException>(() => document.AppendObject(null, embeddedObject));
        }

        [Test]
        public void AppendObject_WhenObjectIsNull_Throws()
        {
            var document = new SplashDocument();
            Assert.Throws<ArgumentNullException>(() => document.AppendObject(Style.DefaultStyle, null));
        }

        [Test]
        public unsafe void AppendStuff()
        {
            var style1 = new StyleBuilder() { Color = Color.Red }.ToStyle();
            var style2 = new StyleBuilder() { LeftMargin = 10, RightMargin = 10 }.ToStyle();
            var style3 = new StyleBuilder() { Font = SystemFonts.DefaultFont }.ToStyle();
            var embeddedObject = new EmbeddedImage(new Bitmap(16, 16));

            var document = new SplashDocument();
            var changedParagraphIndices = new List<int>();

            document.ParagraphChanged += (sender, e) => changedParagraphIndices.Add(e.ParagraphIndex);

            document.AppendText(style1, "Some text, lalala.\nMore text.");
            document.AppendText(style2, "Tab\t.\n");
            document.AppendLine(style3);
            document.AppendText(style3, ""); // to verify that no change event is raised for empty text
            document.AppendText(style2, "(");
            document.AppendObject(style2, embeddedObject);
            document.AppendText(style2, ")");

            Assert.Multiple(() =>
            {
                // Check char content.
                Assert.AreEqual("Some text, lalala.\nMore text.Tab\t.\n\n( )", document.ToString());

                // Check style table.
                Assert.AreEqual(3, document.StyleCount);
                Assert.AreEqual(style1, document.LookupStyle(0));
                Assert.AreEqual(style2, document.LookupStyle(1));
                Assert.AreEqual(style3, document.LookupStyle(2));

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
                Assert.AreEqual(0, runs[0].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[1].RunKind); // "More text."
                Assert.AreEqual(10, runs[1].CharCount);
                Assert.AreEqual(0, runs[1].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[2].RunKind); // "Tab"
                Assert.AreEqual(3, runs[2].CharCount);
                Assert.AreEqual(1, runs[2].StyleIndex);

                Assert.AreEqual(RunKind.Tab, runs[3].RunKind); // "\t"
                Assert.AreEqual(1, runs[3].CharCount);
                Assert.AreEqual(1, runs[3].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[4].RunKind); // ".\n"
                Assert.AreEqual(2, runs[4].CharCount);
                Assert.AreEqual(1, runs[4].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[5].RunKind); // "\n"
                Assert.AreEqual(1, runs[5].CharCount);
                Assert.AreEqual(2, runs[5].StyleIndex);

                Assert.AreEqual(RunKind.Text, runs[6].RunKind); // "("
                Assert.AreEqual(1, runs[6].CharCount);
                Assert.AreEqual(1, runs[6].StyleIndex);

                Assert.AreEqual(RunKind.Object, runs[7].RunKind); // "("
                Assert.AreEqual(1, runs[7].CharCount);
                Assert.AreEqual(1, runs[7].StyleIndex);
                Assert.AreEqual(0, runs[7].ObjectIndex);

                Assert.AreEqual(RunKind.Text, runs[8].RunKind); // ")"
                Assert.AreEqual(1, runs[8].CharCount);
                Assert.AreEqual(1, runs[8].StyleIndex);

                // Check that paragraph changed notifications were raised as needed.
                Assert.AreElementsEqual(new[] { 0, 1, 2, 3, 3, 3 }, changedParagraphIndices);
            });
        }
    }
}
