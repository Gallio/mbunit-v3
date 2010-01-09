// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Drawing;
using System.Text;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Describes a request to paint overlays.
    /// </summary>
    public class OverlayPaintRequest : EventArgs
    {
        private readonly Graphics graphics;
        private readonly Rectangle frameBounds;
        private readonly int frameNumber;
        private readonly double framesPerSecond;

        /// <summary>
        /// Creates an overlay paint request.
        /// </summary>
        /// <param name="graphics">The graphics object to use for painting the overlay.</param>
        /// <param name="frameBounds">The bounds of the frame that is being rendered into the graphics context.</param>
        /// <param name="frameNumber">The zero-based monotonically increasing number of the frame that is being rendered.</param>
        /// <param name="framesPerSecond">The frame rate expressed as the number of frames per second, or zero if the frame is static.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphics"/> is null.</exception>
        public OverlayPaintRequest(Graphics graphics, Rectangle frameBounds, int frameNumber, double framesPerSecond)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            this.graphics = graphics;
            this.frameBounds = frameBounds;
            this.frameNumber = frameNumber;
            this.framesPerSecond = framesPerSecond;
        }

        /// <summary>
        /// Gets the graphics object to use for painting the overlay.
        /// </summary>
        public Graphics Graphics
        {
            get { return graphics; }
        }

        /// <summary>
        /// Gets the bounds of the frame that is being rendered into the graphics context.
        /// </summary>
        public Rectangle FramesBounds
        {
            get { return frameBounds; }
        }

        /// <summary>
        /// Gets the zero-based monotonically increasing number of the frame that is being rendered.
        /// </summary>
        public int FrameNumber
        {
            get { return frameNumber; }
        }

        /// <summary>
        /// Gets the frame rate expressed as the number of frames per second, or zero if the frame is static.
        /// </summary>
        public double FramesPerSecond
        {
            get { return framesPerSecond; }
        }
    }
}
