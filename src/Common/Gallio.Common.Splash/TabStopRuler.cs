using System;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the strategy for advancing to tab stops.
    /// </summary>
    public abstract class TabStopRuler
    {
        /// <summary>
        /// Advances the X position to the next tab stop.
        /// </summary>
        /// <param name="xPosition">The X position to advance.</param>
        /// <returns>The advanced X position.</returns>
        public abstract int AdvanceToNextTabStop(int xPosition);

        /// <summary>
        /// Creates a tab stop ruler based on per-pixel offsets.
        /// </summary>
        /// <param name="pixelsPerTabStop">The number of pixels per tab stop.</param>
        /// <returns>The ruler.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="pixelsPerTabStop"/> is less than 0.</exception>
        public static TabStopRuler CreatePixelRuler(int pixelsPerTabStop)
        {
            if (pixelsPerTabStop < 0)
                throw new ArgumentOutOfRangeException("pixelsPerTabStop");

            return new PixelRuler(pixelsPerTabStop);
        }

        private sealed class PixelRuler : TabStopRuler
        {
            private readonly int pixelsPerTabStop;

            public PixelRuler(int pixelsPerTabStop)
            {
                this.pixelsPerTabStop = pixelsPerTabStop;
            }

            public override int AdvanceToNextTabStop(int xPosition)
            {
                int advance = pixelsPerTabStop - xPosition % pixelsPerTabStop;
                return xPosition + advance;
            }
        }
    }
}
