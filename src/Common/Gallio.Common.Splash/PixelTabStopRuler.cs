using System;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// A tab stop ruler based on per-pixel offsets.
    /// </summary>
    public class PixelTabStopRuler : TabStopRuler, IEquatable<PixelTabStopRuler>
    {
        private readonly int pixelsPerTabStop;
        private readonly int minimumTabWidth;

        /// <summary>
        /// Creates a tab stop ruler based on per-pixel offsets.
        /// </summary>
        /// <param name="pixelsPerTabStop">The number of pixels per tab stop, or 0 to always advance the minimum tab width.</param>
        /// <param name="minimumTabWidth">The minimum tab width, or 0 if there is no minimum.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pixelsPerTabStop"/> or
        /// <paramref name="minimumTabWidth"/> is less than 0.</exception>
        public PixelTabStopRuler(int pixelsPerTabStop, int minimumTabWidth)
        {
            if (pixelsPerTabStop < 0)
                throw new ArgumentOutOfRangeException("pixelsPerTabStop");
            if (minimumTabWidth < 0)
                throw new ArgumentOutOfRangeException("minimumTabWidth");

            this.pixelsPerTabStop = pixelsPerTabStop;
            this.minimumTabWidth = minimumTabWidth;
        }

        /// <summary>
        /// Gets the number of pixels per tab stop, or 0 to always advance the minimum tab width.
        /// </summary>
        public int PixelsPerTabStop
        {
            get { return pixelsPerTabStop; }
        }

        /// <summary>
        /// Gets the minimum tab width.
        /// </summary>
        public int MinimumTabWidth
        {
            get { return minimumTabWidth; }
        }

        /// <inheritdoc />
        public override int AdvanceToNextTabStop(int xPosition)
        {
            if (pixelsPerTabStop == 0)
                return xPosition + minimumTabWidth;

            int advance = xPosition >= 0
                ? pixelsPerTabStop - xPosition % pixelsPerTabStop
                : - xPosition % pixelsPerTabStop;

            if (advance < minimumTabWidth)
                advance += pixelsPerTabStop;

            return xPosition + advance;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as PixelTabStopRuler);
        }

        /// <inheritdoc />
        public override bool Equals(TabStopRuler other)
        {
            return Equals(other as PixelTabStopRuler);
        }

        /// <inheritdoc />
        public virtual bool Equals(PixelTabStopRuler other)
        {
            return other != null
                && pixelsPerTabStop == other.pixelsPerTabStop
                && minimumTabWidth == other.minimumTabWidth;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return typeof(PixelTabStopRuler).GetHashCode()
                ^ pixelsPerTabStop
                ^ (minimumTabWidth << 15);
        }
    }
}