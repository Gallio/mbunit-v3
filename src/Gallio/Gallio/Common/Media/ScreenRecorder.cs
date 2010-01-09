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
using System.Drawing.Drawing2D;
using System.Threading;
using Gallio.Common.Policies;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Records screenshots as a video.
    /// </summary>
    public class ScreenRecorder : IDisposable
    {
        private ScreenGrabber grabber;
        private Video video;
        private readonly OverlayManager overlayManager;

        private readonly object timerLock = new object();
        private Timer timer;
        private Bitmap lastBitmap;

        /// <summary>
        /// Creates a screen recorder.
        /// </summary>
        /// <param name="grabber">The screen grabber.</param>
        /// <param name="video">The video to which frames are to be added.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="grabber"/> or
        /// <paramref name="video"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="video"/> size
        /// is not exactly equal to the <paramref name="grabber"/> screenshot size.</exception>
        public ScreenRecorder(ScreenGrabber grabber, Video video)
        {
            if (grabber == null)
                throw new ArgumentNullException("grabber");
            if (video == null)
                throw new ArgumentNullException("video");
            if (video.Parameters.Width != grabber.ScreenshotWidth ||
                video.Parameters.Height != grabber.ScreenshotHeight)
                throw new ArgumentException("The video dimensions must be exactly the same as the screenshots obtained by the grabber.");

            this.grabber = grabber;
            this.video = video;

            overlayManager = new OverlayManager();
        }

        /// <summary>
        /// Gets the overlay manager.
        /// </summary>
        public OverlayManager OverlayManager
        {
            get
            {
                ThrowIfDisposed();
                return overlayManager;
            }
        }

        /// <summary>
        /// Gets the current frame number.
        /// </summary>
        protected int FrameNumber
        {
            get { return video.FrameCount; }
        }

        /// <summary>
        /// Gets the number of frames per second.
        /// </summary>
        protected double FramesPerSecond
        {
            get { return video.Parameters.FramesPerSecond; }
        }

        /// <summary>
        /// Gets the screen grabber in use.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        public ScreenGrabber Grabber
        {
            get
            {
                ThrowIfDisposed();
                return grabber;
            }
        }

        /// <summary>
        /// Gets the video being recorded.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        public Video Video
        {
            get
            {
                ThrowIfDisposed();
                return video;
            }
        }

        /// <summary>
        /// Disposes the screen recorder.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Starts or resumes recording.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        public void Start()
        {
            lock (timerLock)
            {
                ThrowIfDisposed();

                if (timer != null)
                    return;

                int period = (int) Math.Round(1000 / video.Parameters.FramesPerSecond);
                timer = new Timer(TimerElapsed, null, 0, period);
            }
        }

        /// <summary>
        /// Stops recorded.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        public void Stop()
        {
            Stop(true);
        }

        private void Stop(bool throwIfDisposed)
        {
            lock (timerLock)
            {
                if (throwIfDisposed)
                    ThrowIfDisposed();

                if (timer != null)
                {
                    timer.Dispose();
                    timer = null;
                }
            }
        }

        /// <summary>
        /// Paints overlays onto a bitmap.
        /// </summary>
        /// <param name="bitmap">The bitmap, not null.</param>
        protected virtual void PaintOverlays(Bitmap bitmap)
        {
            overlayManager.PaintOverlaysOnImage(bitmap, FrameNumber, FramesPerSecond);
        }

        /// <summary>
        /// Adds a captured screenshot to the video.
        /// </summary>
        /// <param name="bitmap">The bitmap, not null.</param>
        protected virtual void AddFrame(Bitmap bitmap)
        {
            PaintOverlays(bitmap);

            Video.AddFrame(new BitmapVideoFrame(bitmap));
        }

        /// <summary>
        /// Disposes the screen recorder.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()"/> was called directly.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop(false);

                if (grabber != null)
                    grabber.Dispose();
            }

            grabber = null;
            lastBitmap = null;
            video = null;
        }

        /// <summary>
        /// Throws <see cref="ObjectDisposedException" /> if the object has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (grabber == null)
                throw new ObjectDisposedException("The screen recorder has been disposed.");
        }

        private void CaptureFrame()
        {
            try
            {
                lastBitmap = grabber.CaptureScreenshot(lastBitmap);
                AddFrame(lastBitmap);
            }
            catch (ScreenshotNotAvailableException)
            {
                // Ignore the exception.
            }
            catch (Exception ex)
            {
                UnhandledExceptionPolicy.Report("Could not capture screenshot.", ex);
            }
        }

        private void TimerElapsed(object dummy)
        {
            lock (timerLock)
            {
                if (timer == null || grabber == null)
                    return;

                CaptureFrame();
            }
        }
    }
}
