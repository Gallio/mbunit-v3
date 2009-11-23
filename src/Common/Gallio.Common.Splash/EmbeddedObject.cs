using System;
using System.Drawing;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// An embedded object that can be drawn into a <see cref="SplashView" />.
    /// </summary>
    public abstract class EmbeddedObject
    {
        /// <summary>
        /// Measures the embedded object.
        /// </summary>
        /// <param name="style">The style of the text run that surrounds the embedded object.  Not null.</param>
        /// <returns>The embedded object measurements.</returns>
        public abstract EmbeddedObjectMeasurements Measure(Style style);

        /// <summary>
        /// Paints the embedded object in the specified area.
        /// </summary>
        /// <param name="style">The style of the text run that surrounds the embedded object.  Not null.</param>
        /// <param name="g">The graphics context.</param>
        /// <param name="area">The area into which the embedded object should be painted.</param>
        public abstract void Paint(Style style, Graphics g, Rectangle area);
    }
}
