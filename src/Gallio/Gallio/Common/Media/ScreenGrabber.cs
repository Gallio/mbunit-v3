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
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Grabs screenshots.
    /// </summary>
    public class ScreenGrabber : IDisposable
    {
        private readonly CaptureParameters parameters;
        private readonly int screenWidth;
        private readonly int screenHeight;
        private readonly int screenshotWidth;
        private readonly int screenshotHeight;
        private readonly double xyScale;

        private bool isDisposed;
        private Bitmap screenBuffer;

        /// <summary>
        /// Creates a screen grabber.
        /// </summary>
        /// <param name="parameters">The capture parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        public ScreenGrabber(CaptureParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            this.parameters = parameters;

            Size screenSize = GetScreenSize();
            screenWidth = screenSize.Width;
            screenHeight = screenSize.Height;

            xyScale = Math.Sqrt(parameters.Zoom);
            screenshotWidth = (int) Math.Round(screenWidth * xyScale);
            screenshotHeight = (int) Math.Round(screenHeight * xyScale);
        }

        /// <summary>
        /// Gets the size of the screen.
        /// </summary>
        /// <returns>The screen size.</returns>
        public static Size GetScreenSize()
        {
            return new Size(GetSystemMetrics(SM_CXSCREEN), GetSystemMetrics(SM_CYSCREEN));
        }

        /// <summary>
        /// Gets the capture parameters.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        public CaptureParameters Parameters
        {
            get
            {
                ThrowIfDisposed();
                return parameters;
            }
        }

        /// <summary>
        /// Gets the width of the screenshots that will be taken.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        public int ScreenshotWidth
        {
            get
            {
                ThrowIfDisposed();
                return screenshotWidth;
            }
        }

        /// <summary>
        /// Gets the width of the screenshots that will be taken.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        public int ScreenshotHeight
        {
            get
            {
                ThrowIfDisposed();
                return screenshotHeight;
            }
        }

        /// <summary>
        /// Disposes the screen grabber.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Captures a screenshot.
        /// </summary>
        /// <param name="bitmap">If not null writes the screenshot into the specified bitmap,
        /// otherwise creates and returns a new bitmap.</param>
        /// <returns>The screenshot.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided <paramref name="bitmap"/> is
        /// not of the expected size.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        public virtual Bitmap CaptureScreenshot(Bitmap bitmap)
        {
            ThrowIfDisposed();

            if (bitmap != null)
            {
                if (bitmap.Width != screenshotWidth || bitmap.Height != screenshotHeight)
                    throw new ArgumentException("The bitmap dimensions must exactly match the screenshot dimensions.");
            }
            else
            {
                bitmap = new Bitmap(screenshotWidth, screenshotHeight);
            }

            if (xyScale != 1.0)
            {
                if (screenBuffer == null)
                {
                    screenBuffer = new Bitmap(screenWidth, screenHeight);
                }

                CaptureScreenToBitmap(screenBuffer);

                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(screenBuffer, 0, 0, screenshotWidth, screenshotHeight);
                }
            }
            else
            {
                CaptureScreenToBitmap(bitmap);
            }

            return bitmap;
        }

        private void CaptureScreenToBitmap(Bitmap bitmap)
        {
            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight), CopyPixelOperation.SourceCopy);

                CURSORINFO cursorInfo = new CURSORINFO();
                cursorInfo.cbSize = Marshal.SizeOf(cursorInfo);

                if (GetCursorInfo(ref cursorInfo))
                {
                    if (cursorInfo.flags == CURSOR_SHOWING)
                    {
                        Cursor cursor = new Cursor(cursorInfo.hCursor);
                        cursor.Draw(graphics, new Rectangle(
                            cursorInfo.ptScreenPos.x - cursor.HotSpot.X,
                            cursorInfo.ptScreenPos.y - cursor.HotSpot.Y,
                            cursor.Size.Width, cursor.Size.Height));
                    }
                }
            }
        }

        /// <summary>
        /// Disposes the screen grabber.
        /// </summary>
        /// <param name="disposing">True if <see cref="Dispose()"/> was called directly.</param>
        protected virtual void Dispose(bool disposing)
        {
            isDisposed = true;

            if (screenBuffer != null)
            {
                screenBuffer.Dispose();
                screenBuffer = null;
            }
        }

        /// <summary>
        /// Throws <see cref="ObjectDisposedException" /> if the object has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException("The screen recorder has been disposed.");
        }

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int GetSystemMetrics(int nIndex);

        // From PInvoke.Net.

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CURSORINFO
        {
            public Int32 cbSize;        // Specifies the size, in bytes, of the structure.
                                        // The caller must set this to Marshal.SizeOf(typeof(CURSORINFO)).
            public Int32 flags;         // Specifies the cursor state. This parameter can be one of the following values:
                                        //    0             The cursor is hidden.
                                        //    CURSOR_SHOWING    The cursor is showing.
            public IntPtr hCursor;      // Handle to the cursor.
            public POINT ptScreenPos;   // A POINT structure that receives the screen coordinates of the cursor.
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(ref CURSORINFO pci);

        private const Int32 CURSOR_SHOWING = 0x00000001;
    }
}
