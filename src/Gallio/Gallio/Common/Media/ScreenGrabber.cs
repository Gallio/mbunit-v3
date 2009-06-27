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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gallio.Common.Platform;

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
        /// Returns true if screenshots can be captured.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method may return false if the application is running as a service which has
        /// not been granted the right to interact with the desktop.
        /// </para>
        /// </remarks>
        /// <returns>True if the screen can be captured</returns>
        public static bool CanCaptureScreenshot()
        {
            if (DotNetRuntimeSupport.IsUsingMono)
                return false;

            try
            {
                IntPtr hWnd = IntPtr.Zero;
                IntPtr hDC = IntPtr.Zero;

                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    hDC = GetDC(hWnd);
                    return hDC != IntPtr.Zero;
                }
                finally
                {
                    if (hDC != IntPtr.Zero)
                        ReleaseDC(hWnd, hDC);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Throws an exception if a screenshot cannot be captured at this time.
        /// </summary>
        /// <exception cref="ScreenshotNotAvailableException">Thrown if a screenshot cannot be captured at this time.</exception>
        public static void ThrowIfScreenshotNotAvailable()
        {
            if (! CanCaptureScreenshot())
                throw new ScreenshotNotAvailableException("Cannot capture screenshots at this time.  The application may be running as a service that has not been granted the right to interact with the desktop.");
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
        /// <remarks>
        /// <para>
        /// Captures a screenshot and returns the bitmap.  The caller is responsible for disposing
        /// the bitmap when no longer needed.
        /// </para>
        /// </remarks>
        /// <param name="bitmap">If not null writes the screenshot into the specified bitmap,
        /// otherwise creates and returns a new bitmap.</param>
        /// <returns>The screenshot.</returns>
        /// <exception cref="ArgumentException">Thrown if the provided <paramref name="bitmap"/> is
        /// not of the expected size.</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the object has been disposed.</exception>
        /// <exception cref="ScreenshotNotAvailableException">Thrown if a screenshot cannot be captured at this time.</exception>
        public virtual Bitmap CaptureScreenshot(Bitmap bitmap)
        {
            ThrowIfDisposed();

            if (bitmap != null && (bitmap.Width != screenshotWidth || bitmap.Height != screenshotHeight))
                throw new ArgumentException("The bitmap dimensions must exactly match the screenshot dimensions.");

            if (DotNetRuntimeSupport.IsUsingMono)
                throw new ScreenshotNotAvailableException("Cannot capture screenshots when running under Mono.");

            bool allocatedBitmap = false;
            try
            {
                if (bitmap == null)
                {
                    try
                    {
                        allocatedBitmap = true;
                        bitmap = new Bitmap(screenshotWidth, screenshotHeight);
                    }
                    catch (Exception ex)
                    {
                        throw new ScreenshotNotAvailableException("Could not allocate bitmap for screenshot.", ex);
                    }
                }

                if (xyScale != 1.0)
                {
                    if (screenBuffer == null)
                    {
                        try
                        {
                            screenBuffer = new Bitmap(screenWidth, screenHeight);
                        }
                        catch (Exception ex)
                        {
                            throw new ScreenshotNotAvailableException(
                                "Could not allocate bitmap for internal screenshot buffer.", ex);
                        }
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
            catch
            {
                if (allocatedBitmap && bitmap != null)
                    bitmap.Dispose();
                throw;
            }
        }

        private void CaptureScreenToBitmap(Bitmap bitmap)
        {
            try
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight),
                        CopyPixelOperation.SourceCopy);

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
            catch (Exception ex)
            {
                throw new ScreenshotNotAvailableException("Could not capture screenshot.  The application may be running as a service that has not been granted the right to interact with the desktop.", ex);
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

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

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
