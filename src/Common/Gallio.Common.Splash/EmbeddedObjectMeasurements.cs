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
using System.Drawing;
using System.Windows.Forms;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Describes the measurements of an embedded object.
    /// </summary>
    public struct EmbeddedObjectMeasurements
    {
        private Size size;
        private Padding margin;
        private int descent;

        /// <summary>
        /// Initializes the measurements.
        /// </summary>
        /// <param name="size">The size of the embedded object.</param>
        public EmbeddedObjectMeasurements(Size size)
        {
            this.size = size;
            margin = Padding.Empty;
            descent = 0;
        }

        /// <summary>
        /// Gets or sets the size of the embedded object.
        /// </summary>
        public Size Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Gets or sets the margin around the embedded object.
        /// </summary>
        public Padding Margin
        {
            get { return margin; }
            set { margin = value; }
        }

        /// <summary>
        /// Gets or sets the descent height of the object in pixels.
        /// </summary>
        /// <remarks>
        /// The descent is the number of pixels from the baseline of the object
        /// to its bottom.  It used to align the baseline of the object with the baseline
        /// of the text that surrounds it on the line.
        /// </remarks>
        public int Descent
        {
            get { return descent; }
            set { descent = value; }
        }
    }
}
