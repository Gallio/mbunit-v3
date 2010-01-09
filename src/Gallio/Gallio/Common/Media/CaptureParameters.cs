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
