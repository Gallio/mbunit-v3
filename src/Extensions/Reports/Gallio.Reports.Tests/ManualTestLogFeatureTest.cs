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

using Gallio.Framework;
using Gallio.Reports.Tests.Properties;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Reports.Tests
{
    /// <summary>
    /// This test generates interesting test logs that may be used to
    /// manually inspect the report formatting.
    /// </summary>
    [TestFixture]
    public class ManualTestLogFeatureTest
    {
        [Test]
        public void EmbeddedResources()
        {
            Log.Write("Embedded image:");
            Log.EmbedImage("Image", Resources.MbUnitLogo);

            Log.Write("Embedded plain text:");
            Log.EmbedPlainText("Plain Text", "This is some plain text.\nLalalala...");

            Log.Write("Embedded XML:");
            Log.EmbedXml("XML", "<life><universe><everything>42</everything></universe></life>");

            Log.Write("Embedded XHTML:");
            Log.EmbedXHtml("XHtml", "<p>Some <b>XHTML</b> markup.<br/>With a line break.</p>");

            Log.Write("Embedded HTML:");
            Log.EmbedHtml("Html", "<p>Some <b>HTML</b> markup.<br>With a line break.</p>");

            Log.Write("Embedded binary data:");
            Log.Embed(new BinaryAttachment("Binary", "application/octet-stream", new byte[] { 67, 65, 66, 66, 65, 71, 69 }));

            Log.Write("The same embedded image as above:");
            Log.EmbedExisting("Image");
        }

        [Test]
        public void AttachedResources()
        {
            Log.WriteLine("Attached image.");
            Log.AttachImage("Image", Resources.MbUnitLogo);

            Log.WriteLine("Attached plain text.");
            Log.AttachPlainText("Plain Text", "This is some plain text.\nLalalala...");

            Log.WriteLine("Attached XML.");
            Log.AttachXml("XML", "<life><universe><everything>42</everything></universe></life>");

            Log.WriteLine("Attached XHTML.");
            Log.AttachXHtml("XHtml", "<p>Some <b>XHTML</b> markup.<br/>With a line break.</p>");

            Log.WriteLine("Attached HTML.");
            Log.AttachHtml("Html", "<p>Some <b>HTML</b> markup.<br>With a line break.</p>");

            Log.WriteLine("Attached binary data.");
            Log.Attach(new BinaryAttachment("Binary", "application/octet-stream", new byte[] { 67, 65, 66, 66, 65, 71, 69 }));
        }

        [Test]
        public void ReportStreams()
        {
            Log.Failures.WriteLine("A failure.");
            Log.Warnings.WriteLine("A warning.");
            Log.ConsoleInput.WriteLine("Console input.");
            Log.ConsoleOutput.WriteLine("Console output.");
            Log.ConsoleError.WriteLine("Console error.");
            Log.DebugTrace.WriteLine("Debug / trace.");
            Log.WriteLine("Default log stream.");
        }

        [Test]
        public void Sections()
        {
            Log.Write("Some text with no newline.");

            using (Log.BeginSection("A section."))
            {
                Log.WriteLine("Some text.");
                Log.WriteLine("More text.");

                Log.EmbedImage("An image", Resources.MbUnitLogo);

                using (Log.BeginSection("Another section."))
                {
                    Log.Write("Same image as above.");
                    Log.EmbedExisting("An image");
                }
            }
        }
    }
}
