using System;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the strategy for advancing to tab stops.
    /// </summary>
    public abstract class TabStopRuler : IEquatable<TabStopRuler>
    {
        /// <summary>
        /// Advances the X position to the next tab stop.
        /// </summary>
        /// <param name="xPosition">The X position to advance.</param>
        /// <returns>The advanced X position.</returns>
        public abstract int AdvanceToNextTabStop(int xPosition);

        /// <inheritdoc />
        public abstract bool Equals(TabStopRuler other);
    }
}
