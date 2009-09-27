using System;
using Gallio.Framework;

namespace MbUnit.Samples.WebTestingWithWatiN.Framework
{
    /// <summary>
    /// Specifies the browser configuration for a browser-based test.
    /// </summary>
    public interface IBrowserConfiguration
    {
        /// <summary>
        /// Gets the browser type for the test.
        /// </summary>
        BrowserType BrowserType { get; }

        /// <summary>
        /// Gets when a browser snapshot should be captured and embedded.
        /// </summary>
        TriggerEvent BrowserSnapshotTriggerEvent { get; }

        /// <summary>
        /// Gets the zoom factor for browser snapshots.
        /// </summary>
        double BrowserSnapshotZoom { get; }

        /// <summary>
        /// Gets when a screen recording should be captured and embedded.
        /// </summary>
        TriggerEvent ScreenRecordingTriggerEvent { get; }

        /// <summary>
        /// Gets the zoom factor for screen recordings.
        /// </summary>
        double ScreenRecordingZoom { get; }

        /// <summary>
        /// Gets the number of frames per second for screen recordings.
        /// </summary>
        double ScreenRecordingFramesPerSecond { get; }

        /// <summary>
        /// Gets a label that is used to identify the browser configuration as part
        /// of the name of the data-driven test.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Creates a browser context for the configuration.
        /// </summary>
        /// <returns>The browser context.</returns>
        BrowserContext CreateBrowserContext();
    }
}
