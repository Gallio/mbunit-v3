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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using MbUnit.Framework;
using MbUnit.Samples.WebTestingWithWatiN.Framework;

namespace MbUnit.Samples.WebTestingWithWatiN
{
    /// <summary>
    /// Demonstrates how to integrate WatiN and MbUnit so as to produce reports
    /// with rich content such as screenshots.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This demonstration only shows some of the features of the Browser attribute.
    /// We recommend reading the code of <see cref="BrowserAttribute" /> and customizing
    /// it as needed for your purposes.
    /// </para>
    /// </remarks>
    public class GoogleSearchDemo : BrowserTestFixture
    {
        /// <summary>
        /// Performs a successful search.  Because the test passed, by default
        /// no browser snapshots or screen recordings will be included in the report.
        /// This behavior can be customized by setting properties of the Browser attribute.
        /// </summary>
        [Test]
        [Browser(BrowserType.IE)]
        public void SuccessfulSearch()
        {
            GoogleSearchPage.GoTo(Browser);

            Browser.Page<GoogleSearchPage>().Search("Fiddlesticks");

            Assert.Contains(Browser.Text, "Fiddlesticks");
        }

        /// <summary>
        /// Performs a series of searches then fails.  Because the test
        /// failed, by default a screen recording will be included in the report.
        /// This behavior can be customized by setting properties of the Browser attribute.
        /// </summary>
        [Test]
        [Browser(BrowserType.IE)]
        public void FailedSearch()
        {
            GoogleSearchPage.GoTo(Browser);

            Browser.Page<GoogleSearchPage>().Search("Gallio");
            Browser.Page<GoogleSearchPage>().Search("MbUnit");
            Browser.Page<GoogleSearchPage>().Search("WatiN");

            Assert.Fail("Cause the test to fail for demonstration purposes.");
        }

        /// <summary>
        /// Performs a series of searches then fails.  This time we capture a
        /// browser snapshot instead of a full screen recording.
        /// </summary>
        [Test]
        [Browser(BrowserType.IE,
            BrowserSnapshotTriggerEvent = TriggerEvent.TestFailedOrInconclusive,
            ScreenRecordingTriggerEvent = TriggerEvent.Never)]
        public void FailedSearch_UseBrowserSnapshotInsteadOfScreenRecording()
        {
            GoogleSearchPage.GoTo(Browser);

            Browser.Page<GoogleSearchPage>().Search("Gallio");
            Browser.Page<GoogleSearchPage>().Search("MbUnit");
            Browser.Page<GoogleSearchPage>().Search("WatiN");

            Assert.Fail("Cause the test to fail for demonstration purposes.");
        }

        /// <summary>
        /// Runs the same test repeatedly with different browsers.
        /// </summary>
        [Test]
        [Browser(BrowserType.IE)]
        [Browser(BrowserType.FireFox)]
        [Browser(BrowserType.Chrome)]
        public void MultipleBrowsers()
        {
            GoogleSearchPage.GoTo(Browser);

            Browser.Page<GoogleSearchPage>().Search("Fiddlesticks");
        }

        /// <summary>
        /// Captures snapshots periodically after interesting operations.
        /// Please note that current versions of WatiN only support snapshots with IE.
        /// </summary>
        [Test]
        [Browser(BrowserType.IE)]
        public void Snapshots()
        {
            GoogleSearchPage.GoTo(Browser);

            Browser.Page<GoogleSearchPage>().Search("Gallio");
            EmbedBrowserSnapshot("Gallio Search Results");

            Browser.Page<GoogleSearchPage>().Search("MbUnit");
            EmbedBrowserSnapshot("MbUnit Search Results");

            Browser.Page<GoogleSearchPage>().Search("WatiN");
            EmbedBrowserSnapshot("WatiN Search Results");
        }
    }
}
