using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Abstract base class for video overlays.
    /// </summary>
    public abstract class Overlay
    {
        /// <summary>
        /// Paints the overlay.
        /// </summary>
        /// <param name="request">The paint request.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
        public void Paint(OverlayPaintRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            PaintImpl(request);
        }

        /// <summary>
        /// Paints the overlay.
        /// </summary>
        /// <param name="request">The paint request, not null.</param>
        protected abstract void PaintImpl(OverlayPaintRequest request);
    }
}
