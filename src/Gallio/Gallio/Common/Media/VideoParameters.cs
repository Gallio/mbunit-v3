using System;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Specifies the parameters for a video.
    /// </summary>
    /// <seealso cref="Video"/>
    public abstract class VideoParameters
    {
        private readonly int width;
        private readonly int height;
        private readonly double framesPerSecond;

        /// <summary>
        /// Creates a video parameters object.
        /// </summary>
        /// <param name="width">The width of the video.</param>
        /// <param name="height">The height of the video.</param>
        /// <param name="framesPerSecond">The number of frames per second of the video.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="width"/> or <paramref name="height"/>,
        /// or <paramref name="framesPerSecond"/> are zero or negative.</exception>
        public VideoParameters(int width, int height, double framesPerSecond)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException("width", "The width must be at least 1.");
            if (height < 1)
                throw new ArgumentOutOfRangeException("height", "The height must be at least 1.");
            if (framesPerSecond <= 0.0)
                throw new ArgumentOutOfRangeException("framesPerSecond", "Frames per second must be non-zero and positive.");

            this.width = width;
            this.height = height;
            this.framesPerSecond = framesPerSecond;
        }

        /// <summary>
        /// Gets the width of the video.
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Gets the height of the video.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Gets the number of frames per second of the video.
        /// </summary>
        public double FramesPerSecond
        {
            get { return framesPerSecond; }
        }
    }
}
