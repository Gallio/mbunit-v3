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
using System.Drawing;
using Gallio.Common.Media;

namespace Gallio.Framework
{
    /// <summary>
    /// Captures screen shots and screen videos.
    /// </summary>
    public static class Capture
    {
        /// <summary>
        /// Gets the size of the screen.
        /// </summary>
        /// <returns>The screen size.</returns>
        public static Size GetScreenSize()
        {
            return ScreenGrabber.GetScreenSize();
        }

        /// <summary>
        /// Captures an image of the entire desktop.
        /// </summary>
        /// <returns>The screenshot.</returns>
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
        public static Bitmap Screenshot(CaptureParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            using (var grabber = new ScreenGrabber(parameters))
                return grabber.CaptureScreenshot(null);
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
        public static ScreenRecorder StartRecording(CaptureParameters parameters, double framesPerSecond)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            ScreenGrabber grabber = new ScreenGrabber(parameters);
            try
            {
                FlashScreenVideo video = new FlashScreenVideo(new FlashScreenVideoParameters(
                    grabber.ScreenshotWidth, grabber.ScreenshotHeight, framesPerSecond));

                ScreenRecorder recorder = new ScreenRecorder(grabber, video);
                try
                {
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
        /// <param name="triggerEvent">The trigger event.</param>
        /// <param name="attachmentName">The name to give the image attachment, or null to assign one automatically.</param>
        public static void AutoEmbedScreenshot(TriggerEvent triggerEvent, string attachmentName)
        {
            AutoEmbedScreenshot(triggerEvent, attachmentName, new CaptureParameters());
        }

        /// <summary>
        /// Automatically embeds a screenshot when a trigger event occurs.
        /// </summary>
        /// <param name="triggerEvent">The trigger event.</param>
        /// <param name="attachmentName">The name to give the image attachment, or null to assign one automatically.</param>
        /// <param name="parameters">The capture parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        public static void AutoEmbedScreenshot(TriggerEvent triggerEvent, string attachmentName, CaptureParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            TestContext context = TestContext.CurrentContext;
            if (context != null)
            {
                context.AutoExecute(triggerEvent, () =>
                {
                    Bitmap bitmap = Screenshot(parameters);
                    context.LogWriter.Default.EmbedImage(attachmentName, bitmap);
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
        /// </remarks>
        /// <param name="triggerEvent">The trigger event.</param>
        /// <param name="attachmentName">The name to give the video attachment, or null to assign one automatically.</param>
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
        /// </remarks>
        /// <param name="triggerEvent">The trigger event.</param>
        /// <param name="attachmentName">The name to give the video attachment, or null to assign one automatically.</param>
        /// <param name="parameters">The capture parameters.</param>
        /// <param name="framesPerSecond">The number of frames per second to capture.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        public static void AutoEmbedRecording(TriggerEvent triggerEvent, string attachmentName, CaptureParameters parameters, double framesPerSecond)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            TestContext context = TestContext.CurrentContext;
            if (context != null)
            {
                ScreenRecorder recorder = StartRecording(parameters, framesPerSecond);

                context.AutoExecute(triggerEvent, () =>
                {
                    recorder.Stop();
                    context.LogWriter.Default.EmbedVideo(attachmentName, recorder.Video);
                }, recorder.Dispose);
            }
        }
    }
}
