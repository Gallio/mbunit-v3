// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Gallio.Common.Media;
using Gallio.Framework;
using Gallio.Common.Markup;
using Gallio.Tests.Properties;
using MbUnit.Framework;

namespace Gallio.Tests.Reports
{
    /// <summary>
    /// This test generates interesting test logs that may be used to
    /// manually inspect the report formatting.
    /// </summary>
    [TestFixture]
    public class ManualTestLogFeatureTest
    {
        private Video video;

        [FixtureSetUp]
        public void CreateVideo()
        {
            const int width = 640;
            const int height = 480;
            const int frames = 30;

            Stopwatch stopwatch = Stopwatch.StartNew();
            var parameters = new FlashScreenVideoParameters(width, height, 5)
            {
                BlockWidth = 256,
                BlockHeight = 256
            };
            video = new FlashScreenVideo(parameters);
            stopwatch.Stop();

            Bitmap bitmap = new Bitmap(width, height);
            BitmapVideoFrame bitmapVideoFrame = new BitmapVideoFrame(bitmap);
            for (int frame = 0; frame < frames; frame++)
            {
                BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height),
                    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    int offset = 0;
                    for (int y = 0; y < height; y++)
                    {
                        int scanOffset = offset;

                        for (int x = 0; x < width; x++)
                        {
                            int color = (x * 255 / width) << 16 | (y * 255 / height) << 8 | frame * 255 / frames;
                            Marshal.WriteInt32(bitmapData.Scan0, scanOffset, color);

                            scanOffset += 4;
                        }

                        offset += bitmapData.Stride;
                    }
                }
                finally
                {
                    bitmap.UnlockBits(bitmapData);
                }

                stopwatch.Start();
                video.AddFrame(bitmapVideoFrame);
                stopwatch.Stop();
            }

            TestLog.WriteLine("Video encoding {2} frames at {0}x{1} took {3}s", width, height, frames, stopwatch.Elapsed.TotalSeconds);
        }

        [Test]
        public void EmbeddedResources()
        {
            TestLog.Write("Embedded image:");
            TestLog.EmbedImage("Image", Resources.MbUnitLogo);

            TestLog.Write("Embedded plain text:");
            TestLog.EmbedPlainText("Plain Text", "This is some plain text.\nLalalala...\n\tIndented with TAB.\nThis should all appear to be preformatted.");

            TestLog.Write("Embedded XML:");
            TestLog.EmbedXml("XML", "<life><universe><everything>42</everything></universe></life>");

            TestLog.Write("Embedded XHTML:");
            TestLog.EmbedXHtml("XHtml", "<p>Some <b>XHTML</b> markup.<br/>With a line break.</p>");

            TestLog.Write("Embedded HTML:");
            TestLog.EmbedHtml("Html", "<p>Some <b>HTML</b> markup.<br>With a line break.</p>");

            TestLog.Write("Embedded binary data:");
            TestLog.Embed(new BinaryAttachment("Binary", "application/octet-stream", new byte[] { 67, 65, 66, 66, 65, 71, 69 }));

            TestLog.Write("Embedded video:");
            TestLog.EmbedVideo("Video", video);

            TestLog.Write("The same embedded image as above:");
            TestLog.EmbedExisting("Image");
        }

        [Test]
        public void AttachedResources()
        {
            TestLog.WriteLine("Attached image.");
            TestLog.AttachImage("Image", Resources.MbUnitLogo);

            TestLog.WriteLine("Attached plain text.");
            TestLog.AttachPlainText("Plain Text", "This is some plain text.\nLalalala...");

            TestLog.WriteLine("Attached XML.");
            TestLog.AttachXml("XML", "<life><universe><everything>42</everything></universe></life>");

            TestLog.WriteLine("Attached XHTML.");
            TestLog.AttachXHtml("XHtml", "<p>Some <b>XHTML</b> markup.<br/>With a line break.</p>");

            TestLog.WriteLine("Attached HTML.");
            TestLog.AttachHtml("Html", "<p>Some <b>HTML</b> markup.<br>With a line break.</p>");

            TestLog.WriteLine("Attached binary data.");
            TestLog.Attach(new BinaryAttachment("Binary", "application/octet-stream", new byte[] { 67, 65, 66, 66, 65, 71, 69 }));

            TestLog.Write("Attached video.");
            TestLog.AttachVideo("Video", video);
        }

        [Test]
        public void ReportStreams()
        {
            TestLog.Failures.WriteLine("A failure.");
            TestLog.Warnings.WriteLine("A warning.");
            TestLog.ConsoleInput.WriteLine("Console input.");
            TestLog.ConsoleOutput.WriteLine("Console output.");
            TestLog.ConsoleError.WriteLine("Console error.");
            TestLog.DebugTrace.WriteLine("Debug / trace.");
            TestLog.WriteLine("Default log stream.");
        }

        [Test]
        public void Sections()
        {
            TestLog.Write("Some text with no newline.");

            using (TestLog.BeginSection("A section."))
            {
                TestLog.WriteLine("Some text.");
                TestLog.WriteLine("More text.");

                TestLog.EmbedImage("An image", Resources.MbUnitLogo);

                using (TestLog.BeginSection("Another section."))
                {
                    TestLog.Write("Same image as above.");
                    TestLog.EmbedExisting("An image");
                }
            }
        }
    }
}