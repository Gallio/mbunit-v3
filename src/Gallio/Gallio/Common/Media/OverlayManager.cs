// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using Gallio.Common.Policies;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Manages video source overlays.
    /// </summary>
    public class OverlayManager
    {
        private readonly List<Overlay> overlays;

        /// <summary>
        /// Creates an overlay manager with no overlays.
        /// </summary>
        public OverlayManager()
        {
            overlays = new List<Overlay>();
        }

        /// <summary>
        /// Gets the read-only list of registered overlays.
        /// </summary>
        public IList<Overlay> Overlays
        {
            get { return new ReadOnlyCollection<Overlay>(overlays); }
        }

        /// <summary>
        /// Adds an overlay.
        /// </summary>
        /// <param name="overlay">The overlay to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
        public void AddOverlay(Overlay overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("overlay");

            overlays.Add(overlay);
        }

        /// <summary>
        /// Removes an overlay.
        /// </summary>
        /// <param name="overlay">The overlay to remove.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="overlay"/> is null.</exception>
        public void RemoveOverlay(Overlay overlay)
        {
            if (overlay == null)
                throw new ArgumentNullException("overlay");

            overlays.Remove(overlay);
        }

        /// <summary>
        /// Asks all overlays to paint themselves.
        /// </summary>
        /// <param name="request">The paint request.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
        public void PaintOverlays(OverlayPaintRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            foreach (Overlay overlay in overlays)
            {
                GraphicsState originalState = request.Graphics.Save();
                try
                {
                    overlay.Paint(request);
                }
                finally
                {
                    request.Graphics.Restore(originalState);
                }
            }
        }

        /// <summary>
        /// Asks all overlays to paint themselves over the specified image.
        /// </summary>
        /// <param name="image">The image to paint on.</param>
        /// <param name="frameNumber">The frame number as in <see cref="OverlayPaintRequest.FrameNumber" />.</param>
        /// <param name="framesPerSecond">The number of frames per second as in <see cref="OverlayPaintRequest.FramesPerSecond" />.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="image"/> is null.</exception>
        public void PaintOverlaysOnImage(Image image, int frameNumber, double framesPerSecond)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (overlays.Count != 0)
            {
                using (Graphics graphics = Graphics.FromImage(image))
                {
                    PaintOverlays(new OverlayPaintRequest(graphics,
                        new Rectangle(0, 0, image.Width, image.Height),
                        frameNumber, framesPerSecond));
                }
            }
        }

        /// <summary>
        /// Returns a wrapper for the overlay manager as a composite overlay.
        /// </summary>
        /// <returns>The composite overlay.</returns>
        public Overlay ToOverlay()
        {
            return new CompositeOverlay(this);
        }

        private sealed class CompositeOverlay : Overlay
        {
            private readonly OverlayManager overlayManager;

            public CompositeOverlay(OverlayManager overlayManager)
            {
                this.overlayManager = overlayManager;
            }

            protected override void PaintImpl(OverlayPaintRequest request)
            {
                overlayManager.PaintOverlays(request);
            }
        }
    }
}
