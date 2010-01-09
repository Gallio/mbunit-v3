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

using System;
using System.Diagnostics;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Common.Markup;
using MbUnit.Framework;
using Gallio.Model;
using MbUnit.Samples.Properties;

namespace MbUnit.Samples.FeatureDemos
{
    /// <summary>
    /// This test fixture demonstrates some interesting features of the test log
    /// mechanism such as separating output into distinct "streams", creating
    /// nested sections, and embedding attachments.
    /// </summary>
    [TestFixture]
    [Description("Demonstrate the rich test logging features.")]
    public class TestLogDemo
    {
        /// <summary>
        /// This demo writes messages to the test log grouped into sections.
        /// </summary>
        [Test]
        public void Sections()
        {
            TestLog.WriteLine("This text appears outside of any section.");

            using (TestLog.BeginSection("A Section"))
            {
                TestLog.WriteLine("This text appears within a section called 'A Section'.");
                TestLog.WriteLine("Sections are useful for grouping related output to make it easier to read.");
            }
        }

        /// <summary>
        /// This demo uses markers to attach semantic content to messages written
        /// to the test log.  The test report formatter transforms this information
        /// into styles, links and other presentation artifacts.
        /// </summary>
        [Test]
        public void Markers()
        {
            TestLog.WriteLine("We can write text within markers to modify the presentation.");
            TestLog.WriteLine("The following text appears within a 'Highlight' marker and will be highlighted in the formatted test report.");

            // There is also a shortcut for the followin called TestLog.WriteHighlighted.
            using (TestLog.BeginMarker(Marker.Highlight))
                TestLog.WriteLine("This is important!");

            TestLog.WriteLine("The following text contains a 'Link' marker to create a link to a Url.");

            using (TestLog.BeginMarker(Marker.Link("http://www.mbunit.com")))
                TestLog.WriteLine("Click to go to mbunit.com");
        }

        /// <summary>
        /// This demo embeds an image into the report.
        /// </summary>
        [Test]
        public void EmbeddedImages()
        {
            TestLog.WriteLine("This log contains an embedded image.");

            TestLog.EmbedImage("MbUnit Logo", Resources.MbUnitLogo);
        }

        /// <summary>
        /// This demo shows how different parts of the log can be isolated into
        /// separate log streams.  Gallio sends some output to built-in log
        /// streams automatically.  You can write to these built-in log streams
        /// or create log streams of your own.
        /// </summary>
        [Test]
        public void MultipleStreams()
        {
            // aka. TestLog.ConsoleError
            Console.Error.WriteLine("Console error messages go into the 'ConsoleError' stream.");

            // aka. TestLog.ConsoleOutput
            Console.Out.WriteLine("Console output messages go into the 'ConsoleOutput' stream.");

            // aka. TestLog.DebugTrace
            Debug.WriteLine("Debug...");
            Trace.WriteLine("... and Trace messages go into the 'DebugTrace' stream.");

            TestLog.Warnings.WriteLine("Warnings go into the 'Warnings' stream.");

            TestLog.Failures.WriteLine("Failures go into the 'Failures' stream.");

            TestLog.WriteLine("Log messages go into the 'Log' stream.");

            TestLog.Writer["My Custom Log"].WriteLine("Log messages can also go into a custom stream of your choice.");
        }
    }
}
