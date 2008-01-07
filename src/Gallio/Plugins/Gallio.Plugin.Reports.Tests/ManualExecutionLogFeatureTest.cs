using Gallio.Logging;
using Gallio.Plugin.Reports.Tests.Properties;
using MbUnit.Framework;

namespace Gallio.Plugin.Reports.Tests
{
    /// <summary>
    /// This test generates interesting execution logs that may be used to
    /// manually inspect the report formatting.
    /// </summary>
    [TestFixture]
    public class ManualExecutionLogFeatureTest
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
            Log.Embed(new BinaryAttachment("Binary", "application/octet-stream", new byte[] { 67, 65, 66, 66, 65, 70, 69 }));

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
            Log.WriteLine("Some log output.");
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
