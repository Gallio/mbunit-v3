using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the measurements of an embedded object.
    /// </summary>
    public struct EmbeddedObjectMeasurements
    {
        private readonly Size size;
        private readonly Padding margin;

        /// <summary>
        /// Initializes the measurements.
        /// </summary>
        /// <param name="size">The size of the embedded object.</param>
        /// <param name="margin">The margin around the embedded object.</param>
        public EmbeddedObjectMeasurements(Size size, Padding margin)
        {
            this.size = size;
            this.margin = margin;
        }

        /// <summary>
        /// Gets the size of the embedded object.
        /// </summary>
        public Size Size
        {
            get { return size; }
        }

        /// <summary>
        /// Gets the margin around the embedded object.
        /// </summary>
        public Padding Margin
        {
            get { return margin; }
        }
    }
}
