// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Text;
using System.Text.RegularExpressions;
using MbUnit.Framework;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Logging;
using WatiN.Core;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;

namespace MbUnit.Samples
{
    [TestFixture]
    public class WatiNDemo
    {
        private IE ie;

        [SetUp]
        public void CreateBrowser()
        {
            ie = new IE();

            Logger.LogWriter = new WatiNStreamLogger();
        }

        [TearDown]
        public void DisposeBrowser()
        {
            if (Context.CurrentContext.Outcome == TestOutcome.Failed)
                Snapshot("Final screen when failure occurred.", LogStreamNames.Failures);

            if (ie != null)
                ie.Dispose();
        }

        [Test]
        public void DemoCaptureOnFailure()
        {
            IE.Settings.WaitForCompleteTimeOut = 5;

            using (Log.BeginSection("Go to Google, enter MbUnit as a search term and click I'm Feeling Lucky"))
            {
                ie.GoTo("http://www.google.com");

                ie.TextField(Find.ByName("q")).TypeText("MbUnit");
                ie.Button(Find.ByName("btnI")).Click();
            }

            // Of course this is ridiculous, we'll be on the MbUnit homepage...
            Assert.IsTrue(ie.ContainsText("NUnit"), "Expected to find NUnit on the page.");
        }

        [Test]
        public void DemoNoCaptureOnSuccess()
        {
            IE.Settings.WaitForCompleteTimeOut = 5;

            using (Log.BeginSection("Go to Google, enter MbUnit as a search term and click I'm Feeling Lucky"))
            {
                ie.GoTo("http://www.google.com");

                ie.TextField(Find.ByName("q")).TypeText("MbUnit");
                ie.Button(Find.ByName("btnI")).Click();
            }

            using (Log.BeginSection("Click on About."))
            {
                Assert.IsTrue(ie.ContainsText("MbUnit"));
                ie.Link(Find.ByUrl(new Regex(@"About\.aspx"))).Click();
            }

            Snapshot("About the MbUnit project.");
        }

        private void Snapshot(string caption)
        {
            Snapshot(caption, LogStreamNames.Default);
        }

        private void Snapshot(string caption, string logStreamName)
        {
            using (Log.Writer[logStreamName].BeginSection(caption))
            {
                Log.Writer[logStreamName].WriteLine("Url: {0}", ie.Url);
                Log.Writer[logStreamName].EmbedImage(null, new CaptureWebPage(ie).CaptureWebPageImage(false, false, 100));
            }
        }

        private class WatiNStreamLogger : ILogWriter
        {
            public void LogAction(string message)
            {
                Log.WriteLine(message);
            }
        }
    }
}
