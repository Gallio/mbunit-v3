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
using System.Drawing;
using System.Text;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// Represents an instance of an embedded object that has been attached to a particular site.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When the client is disposed, it should release all attachments to its site.
    /// </para>
    /// </remarks>
    public interface IEmbeddedObjectClient : IDisposable
    {
        /// <summary>
        /// Returns true if the <see cref="Paint"/> method should be called or false if the
        /// client paints itself by other means.
        /// </summary>
        bool RequiresPaint { get; }

        /// <summary>
        /// Measure the client.
        /// </summary>
        /// <returns>The embedded object measurements.</returns>
        EmbeddedObjectMeasurements Measure();

        /// <summary>
        /// Shows the embedded object and sets its bounds.
        /// </summary>
        /// <param name="bounds">The bounds of the embedded object.</param>
        /// <param name="rightToLeft">True if the current reading order is right to left.</param>
        void Show(Rectangle bounds, bool rightToLeft);

        /// <summary>
        /// Hides the embedded object.
        /// </summary>
        void Hide();

        /// <summary>
        /// Paints the embedded object.
        /// </summary>
        /// <param name="g">The graphics context.  Not null.</param>
        /// <param name="paintOptions">The paint options.  Not null.</param>
        /// <param name="bounds">The bounds of the embedded object.</param>
        /// <param name="rightToLeft">True if the current reading order is right to left.</param>
        void Paint(Graphics g, PaintOptions paintOptions, Rectangle bounds, bool rightToLeft);
    }
}
