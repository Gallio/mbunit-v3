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
using Gallio.Common.Collections;
using Gallio.Common.Media;
using Gallio.Framework;
using WatiN.Core;
using WatiN.Core.Interfaces;
using WatiN.Core.Logging;
using WatiN.Core.UtilityClasses;

namespace MbUnit.Samples.WebTestingWithWatiN.Framework
{
    /// <summary>
    /// The browser context provides access to the browser instance and options for
    /// the executing test.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Some properties of the browser context are only available during specific
    /// phases of the test lifecycle.  For example, the browser instance cannot be
    /// accessed before the test has been set up or after it has been torn down.
    /// </para>
    /// </remarks>
    public class BrowserContext
    {
        private static readonly Key<BrowserContext> BrowserContextKey = new Key<BrowserContext>("BrowserContext");

        private readonly IBrowserConfiguration browserConfiguration;
        private Browser browser;

        private bool isBrowserAvailable;
        private ScreenRecorder screenRecorder;

        /// <summary>
        /// Creates a new browser context.
        /// </summary>
        /// <param name="browserConfiguration">The browser configuration.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="browserConfiguration"/> is null.</exception>
        public BrowserContext(IBrowserConfiguration browserConfiguration)
        {
            if (browserConfiguration == null)
                throw new ArgumentNullException("browserConfiguration");

            this.browserConfiguration = browserConfiguration;
        }

        /// <summary>
        /// Gets the browser context from a test context.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <returns>The browser context, or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testContext"/> is null.</exception>
        public static BrowserContext GetBrowserContext(TestContext testContext)
        {
            if (testContext == null)
                throw new ArgumentNullException("testContext");

            return testContext.Data.GetValueOrDefault(BrowserContextKey, null);
        }

        /// <summary>
        /// Sets the browser context for a test context.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="browserContext">The browser context, or null if none.</param>
        /// <returns>The browser test context or null if none.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="testContext"/> is null.</exception>
        public static void SetBrowserContext(TestContext testContext, BrowserContext browserContext)
        {
            if (testContext == null)
                throw new ArgumentNullException("testContext");

            if (browserContext == null)
                testContext.Data.RemoveValue(BrowserContextKey);
            else
                testContext.Data.SetValue(BrowserContextKey, browserContext);
        }

        /// <summary>
        /// Returns true if there is a current browser context.
        /// </summary>
        public static bool HasCurrentBrowserContext
        {
            get { return GetBrowserContext(TestContext.CurrentContext) != null; }
        }

        /// <summary>
        /// Gets the current browser context.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no current browser context.</exception>
        public static BrowserContext CurrentBrowserContext
        {
            get
            {
                BrowserContext browserContext = GetBrowserContext(TestContext.CurrentContext);
                if (browserContext == null)
                    throw new InvalidOperationException("There is no current browser context.  Does the test have a [Browser] attribute?");
                return browserContext;
            }
        }

        /// <summary>
        /// Returns true if there is a browser object available.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Might return false if the test has not been set up or has already been torn down.
        /// </para>
        /// </remarks>
        public bool IsBrowserAvailable
        {
            get { return isBrowserAvailable; }
        }

        /// <summary>
        /// Returns true if there is a browser object already open.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Might return false if the test has not been set up or has already been torn down
        /// or if the browser object has not been created.
        /// </para>
        /// </remarks>
        public bool IsBrowserOpen
        {
            get { return isBrowserAvailable && browser != null; }
        }

        /// <summary>
        /// Gets the browser instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the browser is not available.</exception>
        public Browser Browser
        {
            get
            {
                if (! isBrowserAvailable)
                    throw new InvalidOperationException(
                        "The browser is not available.  The test might not have been set up or has already been torn down.");

                if (browser == null)
                    browser = CreateBrowser();

                return browser;
            }
        }

        /// <summary>
        /// Gets the browser configuration.
        /// </summary>
        public IBrowserConfiguration BrowserConfiguration
        {
            get { return browserConfiguration; }
        }

        /// <summary>
        /// Creates an instance of a WatiN browser configured as needed.
        /// </summary>
        /// <returns>The new browser instance.</returns>
        public virtual Browser CreateBrowser()
        {
            switch (browserConfiguration.BrowserType)
            {
                case BrowserType.IE:
                    var ie = new IE();
                    ie.ShowWindow(WatiN.Core.Native.Windows.NativeMethods.WindowShowStyle.Maximize);
                    return ie;

                case BrowserType.FireFox:
                    var fireFox = new FireFox();
                    return fireFox;

                case BrowserType.Chrome:
                    var chrome = new Chrome();
                    return chrome;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Performs set up tasks before running a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called automatically by the infrastructure and should not be called from your test.
        /// </para>
        /// </remarks>
        public virtual void SetUp()
        {
            isBrowserAvailable = true;

            StartScreenRecordingIfNeeded();

            ConfigureWatiNSettings();
        }

        /// <summary>
        /// Performs tear down tasks after running a test.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called automatically by the infrastructure and should not be called from your test.
        /// </para>
        /// </remarks>
        public virtual void TearDown()
        {
            try
            {
                if (browser != null)
                {
                    StopScreenRecordingAndEmbedIfNeeded();
                    EmbedFinalBrowserSnapshotIfNeeded();

                    try
                    {
                        browser.Close();
                    }
                    catch
                    {
                        // Ignore problems closing the browser.  It's possible that the user forcibly
                        // closed the browser before the test was finished in which case attempting
                        // to close it again could yield an error.
                    }

                    browser = null;
                }
            }
            finally
            {
                isBrowserAvailable = false;
            }
        }

        /// <summary>
        /// Configures screen recording and screen shot capture settings.
        /// </summary>
        /// <seealso cref="Capture"/>
        public virtual void ConfigureCaptureSettings()
        {
            Capture.SetCaptionFontSize(24);
            Capture.SetCaptionAlignment(HorizontalAlignment.Center, VerticalAlignment.Bottom);
        }

        /// <summary>
        /// Configures WatiN global settings.
        /// </summary>
        public virtual void ConfigureWatiNSettings()
        {
            if (!(Logger.LogWriter is GallioLogger))
                Logger.LogWriter = new GallioLogger();

            Settings.AutoMoveMousePointerToTopLeft = false;
            Settings.MakeNewIeInstanceVisible = true;
        }

        /// <summary>
        /// Embeds a browser snapshot from the current browser.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does nothing if the browser is not open or if the snapshot could not be captured
        /// due to an error.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The attachment name, or null to construct one automatically.</param>
        public void EmbedBrowserSnapshot(string attachmentName)
        {
            if (IsBrowserOpen)
                EmbedBrowserSnapshot(attachmentName, browser);
        }

        /// <summary>
        /// Captures and embed a screenshot from the specified browser.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does nothing if the browser is not open or if the snapshot could not be captured
        /// due to an error.
        /// </para>
        /// </remarks>
        /// <param name="attachmentName">The attachment name, or null to construct one automatically.</param>
        /// <param name="browser">The browser.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="browser"/> is null.</exception>
        public void EmbedBrowserSnapshot(string attachmentName, Browser browser)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            try
            {
                var capture = new CaptureWebPage(browser);
                System.Drawing.Image image = capture.CaptureWebPageImage(true, false, 100);

                using (TestLog.BeginSection(browser.Url))
                    TestLog.EmbedImage(attachmentName, image);
            }
            catch
            {
                // Ignore the failure since the snapshot is for diagnostic purposes only.
                // If we can't capture it then too bad.
            }
        }

        private void EmbedFinalBrowserSnapshotIfNeeded()
        {
            if (ShouldEmbedFinalBrowserSnapshotGivenCurrentTestContext())
            {
                EmbedBrowserSnapshot("Final browser contents.");
            }
        }

        private void StartScreenRecordingIfNeeded()
        {
            if (ShouldStartScreenRecordingGivenCurrentTestContext())
            {
                screenRecorder = Capture.StartRecording(new CaptureParameters()
                {
                    Zoom = browserConfiguration.ScreenRecordingZoom
                }, browserConfiguration.ScreenRecordingFramesPerSecond);
            }
        }

        private void StopScreenRecordingAndEmbedIfNeeded()
        {
            if (screenRecorder != null)
            {
                screenRecorder.Stop();
                Video video = screenRecorder.Video;
                screenRecorder = null;

                if (ShouldEmbedScreenRecordingGivenCurrentTestContext())
                {
                    TestLog.EmbedVideo("Screen Recording", video);
                }
            }
        }

        private bool ShouldStartScreenRecordingGivenCurrentTestContext()
        {
            return browserConfiguration.ScreenRecordingTriggerEvent != TriggerEvent.Never;
        }

        private bool ShouldEmbedScreenRecordingGivenCurrentTestContext()
        {
            return TestContext.CurrentContext.IsTriggerEventSatisfied(browserConfiguration.ScreenRecordingTriggerEvent);
        }

        private bool ShouldEmbedFinalBrowserSnapshotGivenCurrentTestContext()
        {
            return TestContext.CurrentContext.IsTriggerEventSatisfied(browserConfiguration.BrowserSnapshotTriggerEvent);
        }

        private sealed class GallioLogger : ILogWriter
        {
            public void LogAction(string message)
            {
                TestLog.WriteLine(message);
                Capture.SetCaption(message);
            }

            public void LogDebug(string message)
            {
                // Ignore debug messages.
            }

            public void LogInfo(string message)
            {
                TestLog.WriteLine(message);
            }
        }
    }
}
