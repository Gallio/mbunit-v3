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
using System.Drawing;
using Gallio.Common.Collections;
using Gallio.Common.Media;

namespace Gallio.Framework
{
    /// <summary>
    /// Captures screenshot images and videos.
    /// </summary>
    /// <remarks>
    /// <para>
    /// These functions are intended to provide a simple interface for capturing,
    /// captioning, and embedding images and video.  If you desire more control over the manner
    /// in which the capture occurs, you may prefer to use the <see cref="ScreenGrabber"/>,
    /// <see cref="ScreenRecorder"/>, and <see cref="Overlay"/> classes directly.
    /// </para>
    /// </remarks>
    public static class Capture
    {
        private static readonly Key<OverlayManager> OverlayManagerKey = new Key<OverlayManager>("OverlayManager");

        /// <summary>
        /// Gets the size of the screen.
        /// </summary>
        /// <returns>The screen size.</returns>
        public static Size GetScreenSize()
        {
            return ScreenGrabber.GetScreenSize();
        }

        /// <summary>
        /// Returns true if screenshots can be captured.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may return false if the application is running as a service which has
        /// not been granted the right to interact with the desktop.
        /// </para>
        /// </remarks>
        /// <returns>True if the screen can be captured.</returns>
        public static bool CanCaptureScreenshot()
        {
            return ScreenGrabber.CanCaptureScreenshot();
        }

        /// <summary>
        /// Captures an image of the entire desktop.
        /// </summary>
        /// <returns>The screenshot.</returns>
        /// <exception cref="ScreenshotNotAvailableException">Thrown if a screenshot cannot be captured at this time.</exception>
        public static Bitmap Screenshot()
        {
            return Screenshot(new CaptureParameters());
        }

        /// <summary>
        /// Captures an image of the entire desktop.
        /// </summary>
        /// <param name="parameters">The capture parameters.</param>
        /// <returns>The screenshot.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        /// <exception cref="ScreenshotNotAvailableException">Thrown if a screenshot cannot be captured at this time.</exception>
        public static Bitmap Screenshot(CaptureParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            using (var grabber = new ScreenGrabber(parameters))
            {
                grabber.OverlayManager.AddOverlay(GetOverlayManager().ToOverlay());
                return grabber.CaptureScreenshot(null);
            }
        }

        /// <summary>
        /// Starts recording a screen capture video of the entire desktop.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Recording a screen capture video can be very CPU and space intensive particularly
        /// when running tests on a single-core CPU.  We recommend calling
        /// <see cref="StartRecording(CaptureParameters, double)" /> with
        /// a <see cref="CaptureParameters.Zoom" /> factor of 0.25 or less and a frame rate
        /// of no more than 5 to 10 frames per second.
        /// </para>
        /// </remarks>
        /// <returns>The recorder.</returns>
        /// <exception cref="ScreenshotNotAvailableException">Thrown if a screenshot cannot be captured at this time.</exception>
        public static ScreenRecorder StartRecording()
        {
            return StartRecording(new CaptureParameters(), 5);
        }

        /// <summary>
        /// Starts recording a screen capture video of the entire desktop.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Recording a screen capture video can be very CPU and space intensive particularly
        /// when running tests on a single-core CPU.  We recommend calling
        /// <see cref="StartRecording(CaptureParameters, double)" /> with
        /// a <see cref="CaptureParameters.Zoom" /> factor of 0.25 or less and a frame rate
        /// of no more than 5 to 10 frames per second.
        /// </para>
        /// </remarks>
        /// <param name="parameters">The capture parameters.</param>
        /// <param name="framesPerSecond">The number of frames per second to capture.</param>
        /// <returns>The recorder.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        /// <exception cref="ScreenshotNotAvailableException">Thrown if a screenshot cannot be captured at this time.</exception>
        public static ScreenRecorder StartRecording(CaptureParameters parameters, double framesPerSecond)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            ScreenGrabber.ThrowIfScreenshotNotAvailable();

            ScreenGrabber grabber = new ScreenGrabber(parameters);
            try
            {
                FlashScreenVideo video = new FlashScreenVideo(new FlashScreenVideoParameters(
                    grabber.ScreenshotWidth, grabber.ScreenshotHeight, framesPerSecond));

                ScreenRecorder recorder = new ScreenRecorder(grabber, video);
                try
                {
                    recorder.OverlayManager.AddOverlay(GetOverlayManager().ToOverlay());

                    recorder.Start();
                    return recorder;
                }
                catch
                {
                    recorder.Dispose();
                    throw;
                }
            }
            catch
            {
                grabber.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Automatically embeds a screenshot when a trigger event occurs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If screenshots cannot be captured, the method will embed a warning message to that effect.
        /// </para>
        /// </remarks>
        /// <param name="triggerEvent">The trigger event.</param>
        /// <param name="attachmentName">The name to give the image attachment, or null to assign one automatically.</param>
        /// <seealso cref="TestContext.AutoExecute(TriggerEvent, Gallio.Common.Action)"/>
        public static void AutoEmbedScreenshot(TriggerEvent triggerEvent, string attachmentName)
        {
            AutoEmbedScreenshot(triggerEvent, attachmentName, new CaptureParameters());
        }

        /// <summary>
        /// Automatically embeds a screenshot when a trigger event occurs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If screenshots cannot be captured, the method will embed a warning message to that effect.
        /// </para>
        /// </remarks>
        /// <param name="triggerEvent">The trigger event.</param>
        /// <param name="attachmentName">The name to give to the image attachment, or null to assign one automatically.</param>
        /// <param name="parameters">The capture parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        /// <seealso cref="TestContext.AutoExecute(TriggerEvent, Gallio.Common.Action)"/>
        public static void AutoEmbedScreenshot(TriggerEvent triggerEvent, string attachmentName, CaptureParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            TestContext context = TestContext.CurrentContext;
            if (context != null)
            {
                context.AutoExecute(triggerEvent, () =>
                {
                    try
                    {
                        Bitmap bitmap = Screenshot(parameters);
                        context.LogWriter.Default.EmbedImage(attachmentName, bitmap);
                    }
                    catch (ScreenshotNotAvailableException ex)
                    {
                        context.LogWriter.Default.WriteException(ex, "Screenshot not available.");
                    }
                });
            }
        }

        /// <summary>
        /// Automatically embeds a video of the test run from this point forward when a trigger event occurs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Recording a screen capture video can be very CPU and space intensive particularly
        /// when running tests on a single-core CPU.  We recommend calling
        /// <see cref="AutoEmbedRecording(TriggerEvent, string, CaptureParameters, double)" /> with
        /// a <see cref="CaptureParameters.Zoom" /> factor of 0.25 or less and a frame rate
        /// of no more than 5 to 10 frames per second.
        /// </para>
        /// <para>
        /// If screenshots cannot be captured, the method will embed a warning message to that effect.
        /// </para>
        /// </remarks>
        /// <param name="triggerEvent">The trigger event.</param>
        /// <param name="attachmentName">The name to give the video attachment, or null to assign one automatically.</param>
        /// <seealso cref="TestContext.AutoExecute(TriggerEvent, Gallio.Common.Action)"/>
        public static void AutoEmbedRecording(TriggerEvent triggerEvent, string attachmentName)
        {
            AutoEmbedRecording(triggerEvent, attachmentName, new CaptureParameters(), 5);
        }

        /// <summary>
        /// Automatically embeds a video of the test run from this point forward when a trigger event occurs.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Recording a screen capture video can be very CPU and space intensive particularly
        /// when running tests on a single-core CPU.  We recommend calling
        /// <see cref="AutoEmbedRecording(TriggerEvent, string, CaptureParameters, double)" /> with
        /// a <see cref="CaptureParameters.Zoom" /> factor of 0.25 or less and a frame rate
        /// of no more than 5 to 10 frames per second.
        /// </para>
        /// <para>
        /// If screenshots cannot be captured, the method will embed a warning message to that effect.
        /// </para>
        /// </remarks>
        /// <param name="triggerEvent">The trigger event.</param>
        /// <param name="attachmentName">The name to give the video attachment, or null to assign one automatically.</param>
        /// <param name="parameters">The capture parameters.</param>
        /// <param name="framesPerSecond">The number of frames per second to capture.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        /// <seealso cref="TestContext.AutoExecute(TriggerEvent, Gallio.Common.Action)"/>
        public static void AutoEmbedRecording(TriggerEvent triggerEvent, string attachmentName, CaptureParameters parameters, double framesPerSecond)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            TestContext context = TestContext.CurrentContext;
            if (context != null)
            {
                try
                {
                    ScreenRecorder recorder = StartRecording(parameters, framesPerSecond);

                    context.AutoExecute(triggerEvent, () =>
                    {
                        recorder.Stop();

                        if (recorder.Video.FrameCount != 0)
                            context.LogWriter.Default.EmbedVideo(attachmentName, recorder.Video);
                    }, recorder.Dispose);
                }
                catch (ScreenshotNotAvailableException ex)
                {
                    context.AutoExecute(triggerEvent, () =>
                    {
                        context.LogWriter.Default.WriteException(ex, "Recording not available.");
                    });
                }
            }
        }

        /// <summary>
        /// Gets the overlay manager for the current test context.
        /// </summary>
        /// <returns>The overlay manager.</returns>
        public static OverlayManager GetOverlayManager()
        {
            return GetOverlayManager(TestContext.CurrentContext);
        }

        /// <summary>
        /// Gets the overlay manager for the specified test context.
        /// </summary>
        /// <param name="context">The test context.</param>
        /// <returns>The overlay manager.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null.</exception>
        public static OverlayManager GetOverlayManager(TestContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            lock (context.Data)
            {
                OverlayManager overlayManager;
                if (! context.Data.TryGetValue(OverlayManagerKey, out overlayManager))
                {
                    overlayManager = new OverlayManager();
                    context.Data.SetValue(OverlayManagerKey, overlayManager);
                }

                return overlayManager;
            }
        }

        /// <summary>
        /// Adds an overlay to display over screenshot images and videos.
        /// </summary>
        /// <param name="overlay">The overlay to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
        public static void AddOverlay(Overlay overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("overlay");

            GetOverlayManager().AddOverlay(overlay);
        }

        /// <summary>
        /// Sets a caption to display over screenshots images and videos. 
        /// </summary>
        /// <param name="text">The caption text, an empty string to remove it.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="text"/> is null.</exception>
        public static void SetCaption(string text)
        {
            GetCaptionOverlay().Text = text;
        }

        /// <summary>
        /// Sets the font size of the caption to display over screenshots images and videos. 
        /// </summary>
        /// <param name="fontSize">The caption font size.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="fontSize"/> is less than 1.</exception>
        public static void SetCaptionFontSize(int fontSize)
        {
            GetCaptionOverlay().FontSize = fontSize;
        }

        /// <summary>
        /// Sets the alignment of the caption to display over screenshots images and videos. 
        /// </summary>
        /// <param name="horizontalAlignment">The horizontal alignment.</param>
        /// <param name="verticalAlignment">The vertical alignment.</param>
        public static void SetCaptionAlignment(HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            CaptionOverlay captionOverlay = GetCaptionOverlay();
            captionOverlay.HorizontalAlignment = horizontalAlignment;
            captionOverlay.VerticalAlignment = verticalAlignment;
        }

        /// <summary>
        /// Gets the caption overlay to display over of screenshots images and videos.
        /// </summary>
        /// <returns>The caption overlay.</returns>
        public static CaptionOverlay GetCaptionOverlay()
        {
            OverlayManager overlayManager = GetOverlayManager();

            DefaultCaptionOverlay defaultCaptionOverlay = (DefaultCaptionOverlay)
                GenericCollectionUtils.Find(overlayManager.Overlays, x => x is DefaultCaptionOverlay);
            if (defaultCaptionOverlay == null)
            {
                defaultCaptionOverlay = new DefaultCaptionOverlay();
                overlayManager.AddOverlay(defaultCaptionOverlay);
            }

            return defaultCaptionOverlay;
        }

        private sealed class DefaultCaptionOverlay : CaptionOverlay
        {
        }
    }
}
