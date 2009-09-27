using System;
using WatiN.Core;

namespace MbUnit.Samples.WebTestingWithWatiN.Framework
{
    /// <summary>
    /// Base test fixture for WatiN tests.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is intended to be used as a superclass for browser based test fixtures.
    /// It provides easy access to the <see cref="BrowserContext"/> that is
    /// introduced by the <see cref="BrowserAttribute"/>, especially the <see cref="Browser"/>
    /// property.
    /// </para>
    /// </remarks>
    /// <seealso cref="BrowserAttribute"/>
    public abstract class BrowserTestFixture
    {
        /// <summary>
        /// Gets the browser for the current test.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the browser is not available.</exception>
        public static Browser Browser
        {
            get { return BrowserContext.Browser; }
        }

        /// <summary>
        /// Gets the browser context for the current test.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if there is no browser context.</exception>
        public static BrowserContext BrowserContext
        {
            get { return BrowserContext.CurrentBrowserContext; }
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
            BrowserContext.EmbedBrowserSnapshot(attachmentName);
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
            BrowserContext.EmbedBrowserSnapshot(attachmentName, browser);
        }
    }
}
