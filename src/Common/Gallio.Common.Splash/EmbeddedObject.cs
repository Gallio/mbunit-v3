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
        /// <param name="site">The site of the embedded object.  Not null.</param>
        /// <returns>The embedded object measurements.</returns>
        public abstract EmbeddedObjectMeasurements Measure(IEmbeddedObjectSite site);

        /// <summary>
        /// Paints the embedded object in the specified area.
        /// </summary>
        /// <param name="site">The site of the embedded object.  Not null.</param>
        /// <param name="g">The graphics context.  Not null.</param>
        /// <param name="area">The area into which the embedded object should be painted.</param>
        /// <param name="paintOptions">The paint options.</param>
        public abstract void Paint(IEmbeddedObjectSite site, Graphics g, Rectangle area, PaintOptions paintOptions);
    }
}
