using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Specifies parameters for screenshot captures.
    /// </summary>
    public class CaptureParameters
    {
        private double zoom;

        /// <summary>
        /// Creates a capture parameters object.
        /// </summary>
        public CaptureParameters()
        {
            zoom = 1.0;
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The zoom factor specifies the degree of magnification or reduction desired.
        /// For example, a zoom factor of 1.0 (the default) is normal size, 0.25 reduces to one quarter the
        /// original size and 2.0 magnifies to twice the original size.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 1/16 or more than 16.</exception>
        public double Zoom
        {
            get { return zoom; }
            set
            {
                if (value < 1.0 / 16.0 || value > 16.0)
                    throw new ArgumentOutOfRangeException("zoom", "The zoom factor must be between 1/16 and 16.");
                zoom = value;
            }
        }
    }
}
