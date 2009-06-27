using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Represents a single frame of video content.
    /// </summary>
    public abstract class VideoFrame
    {
        /// <summary>
        /// Gets the width of the video frame.
        /// </summary>
        public abstract int Width { get; }

        /// <summary>
        /// Gets the height of the video frame.
        /// </summary>
        public abstract int Height { get; }

        /// <summary>
        /// Copies pixels from the frame into a buffer.
        /// </summary>
        /// <param name="sourceRect">The source rectangle of pixels to copy.</param>
        /// <param name="pixelBuffer">The buffer into which to copy the pixels as 32bit RGB with the upper 8 bits zeroed out.</param>
        /// <param name="startOffset">The start offset where the first pixel should be written.</param>
        /// <param name="stride">The incremental offset between subsequent rows of pixels.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pixelBuffer"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="sourceRect"/> components,
        /// <paramref name="startOffset"/> or <paramref name="stride"/> are negative or if they would cause pixels
        /// to be written outside the bounds of the pixel buffer.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="sourceRect"/> components
        /// are negative, <paramref name="startOffset"/> is outside the bounds of the pixel buffer, or <paramref name="stride"/>
        /// is less than the width of the rectange.</exception>
        /// <exception cref="ArgumentException">Thrown if the combined <paramref name="sourceRect"/> dimensions,
        /// <paramref name="startOffset"/> and <paramref name="stride"/> would cause pixels to be written out
        /// of bounds of the pixel buffer.</exception>
        public void CopyPixels(Rectangle sourceRect, int[] pixelBuffer, int startOffset, int stride)
        {
            if (pixelBuffer == null)
                throw new ArgumentNullException("pixelBuffer");
            if (sourceRect.X < 0 || sourceRect.Y < 0 || sourceRect.Width < 0 || sourceRect.Height < 0 || sourceRect.Right > Width || sourceRect.Bottom > Height)
                throw new ArgumentOutOfRangeException("sourceRect", "Rectangle position and dimensions must be within the bounds of the frame.");
            if (startOffset < 0 || startOffset >= pixelBuffer.Length)
                throw new ArgumentOutOfRangeException("startOffset", "Start offset must be within the bounds of the pixel buffer.");
            if (stride < sourceRect.Width)
                throw new ArgumentOutOfRangeException("stride", "Stride must be at least as large as the width of the rectangle.");
            if (sourceRect.Height == 0 || sourceRect.Width == 0)
                return;
            if (startOffset + (sourceRect.Height - 1) * stride + sourceRect.Width > pixelBuffer.Length)
                throw new ArgumentException("The combined rectangle dimensions, start offset and stride would cause pixels to be written out of bounds of the pixel buffer.");

            CopyPixelsImpl(sourceRect, pixelBuffer, startOffset, stride);
        }

        /// <summary>
        /// Copies pixels from the frame into a buffer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The arguments have already been checked to ensure that all pixels in the requested
        /// range will fit in the pixel buffer.  The implementation can assume that it will be safe to
        /// write all requested pixels into the buffer (so it may use unmanaged code to do so
        /// if desired).
        /// </para>
        /// </remarks>
        /// <param name="sourceRect">The source rectangle of pixels to copy.</param>
        /// <param name="pixelBuffer">The buffer into which to copy the pixels as 32bit RGB with the upper 8 bits zeroed out.</param>
        /// <param name="startOffset">The start offset where the first pixel should be written.</param>
        /// <param name="stride">The incremental offset between subsequent rows of pixels.</param>
        protected abstract void CopyPixelsImpl(Rectangle sourceRect, int[] pixelBuffer, int startOffset, int stride);
    }
}
