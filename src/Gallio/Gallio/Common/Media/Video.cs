using System;
using System.IO;
using Gallio.Common.Markup;

namespace Gallio.Common.Media
{
    /// <summary>
    /// A video is an abstraction of an encoded video stream (possibly incorporating audio).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The primary use of this class is to author videos that can be embedded in test
    /// reports.  It is not intended to be used for playback.
    /// </para>
    /// <para>
    /// Subclasses define specific encoding formats.
    /// </para>
    /// <para>
    /// While a video is being created, its contents may be buffered in memory or on disk
    /// in a temporary location.
    /// </para>
    /// </remarks>
    public abstract class Video
    {
        private readonly VideoParameters parameters;

        /// <summary>
        /// Creates a new video.
        /// </summary>
        /// <param name="parameters">The video parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        protected Video(VideoParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            this.parameters = parameters;
        }

        /// <summary>
        /// Gets the mime-type of the video format.
        /// </summary>
        /// <seealso cref="MimeTypes"/>
        public abstract string MimeType { get; }

        /// <summary>
        /// Gets the video parameters.
        /// </summary>
        public VideoParameters Parameters
        {
            get { return parameters; }
        }

        /// <summary>
        /// Adds a frame to the video.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The implementation makes a copy of the frame or encodes it immediately so that
        /// the caller can reuse the same frame buffers for subsequent frames.
        /// </para>
        /// </remarks>
        /// <param name="frame">The video frame to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="frame"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="frame"/> dimensions are not
        /// exactly equal to those of the video.</exception>
        public void AddFrame(VideoFrame frame)
        {
            if (frame == null)
                throw new ArgumentNullException("frame");
            if (frame.Width != parameters.Width || frame.Height != parameters.Height)
                throw new ArgumentException("The frame dimensions must exactly equal those of the video.", "frame");

            AddFrameImpl(frame);
        }

        /// <summary>
        /// Saves the video to a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="stream"/> is null</exception>
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("path");

            SaveImpl(stream);
        }

        /// <summary>
        /// Adds a frame to the video.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The implementation makes a copy of the frame or encodes it immediately so that
        /// the caller can reuse the same frame buffers for subsequent frames.
        /// </para>
        /// <para>
        /// The frame dimensions have already been checked by the caller.
        /// </para>
        /// </remarks>
        /// <param name="frame">The video frame to add, not null.</param>
        protected abstract void AddFrameImpl(VideoFrame frame);

        /// <summary>
        /// Saves the video to a stream.
        /// </summary>
        /// <param name="stream">The stream, not null.</param>
        protected abstract void SaveImpl(Stream stream);
    }
}
