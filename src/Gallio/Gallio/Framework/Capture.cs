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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Gallio.Common;
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
        /// Starts recording a video of the entire desktop.
        /// </summary>
        /// <returns>The recorder.</returns>
        public static ScreenRecorder StartRecording()
        {
            return StartRecording(new CaptureParameters(), 5);
        }

        /// <summary>
        /// Starts recording a video of the entire desktop.
        /// </summary>
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
    }
}
