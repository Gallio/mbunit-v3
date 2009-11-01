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
