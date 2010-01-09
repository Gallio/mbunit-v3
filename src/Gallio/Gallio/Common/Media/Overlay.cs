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
