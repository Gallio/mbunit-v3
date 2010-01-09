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
using System.Drawing;
using System.Windows.Forms;

namespace Gallio.Common.Splash
{
    /// <summary>
    /// An embedded image wraps a <see cref="Control"/> so that it can be embedded in
    /// a Splash document.
    /// </summary>
    public class EmbeddedControl : EmbeddedObject
    {
        private readonly Control control;

        /// <summary>
        /// Creates an embedded control.
        /// </summary>
        /// <param name="control">The control to draw into the client area.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="control"/> is null.</exception>
        public EmbeddedControl(Control control)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            this.control = control;
        }

        /// <summary>
        /// Gets the control.
        /// </summary>
        public Control Control
        {
            get { return control; }
        }

        /// <summary>
        /// Gets or sets the margin around the control.
        /// </summary>
        public Padding Margin { get; set; }

        /// <summary>
        /// Gets or sets the baseline of the control.
        /// </summary>
        /// <remarks>
        /// The default value is 0 which positions the bottom of the control in line with the
        /// baseline of surrounding text.  Use a positive value to raise the control above the
        /// text baseline or a negative value to lower it below the text baseline.
        /// </remarks>
        public int Baseline { get; set; }

        /// <inheritdoc />
        public override IEmbeddedObjectClient CreateClient(IEmbeddedObjectSite site)
        {
            return new Client(this, site.ParentControl);
        }

        private sealed class Client : IEmbeddedObjectClient
        {
            private readonly EmbeddedControl embeddedControl;
            private readonly Control parentControl;
            private Rectangle bounds;

            public Client(EmbeddedControl embeddedControl, Control parentControl)
            {
                this.embeddedControl = embeddedControl;
                this.parentControl = parentControl;

                if (embeddedControl.Control.Parent != null)
                    throw new InvalidOperationException("The embedded control already belongs to a different control container.");

                bounds = embeddedControl.Control.Bounds;
            }

            public void Dispose()
            {
                Hide();
            }

            public bool RequiresPaint
            {
                get { return false; }
            }

            public EmbeddedObjectMeasurements Measure()
            {
                return new EmbeddedObjectMeasurements(embeddedControl.control.Size)
                {
                    Margin = embeddedControl.Margin,
                    Descent = embeddedControl.Baseline
                };
            }

            public void Show(Rectangle bounds, bool rightToLeft)
            {
                if (this.bounds != bounds)
                {
                    this.bounds = bounds;
                    embeddedControl.control.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                }

                parentControl.Controls.Add(embeddedControl.Control);
            }

            public void Hide()
            {
                parentControl.Controls.Remove(embeddedControl.Control);
            }

            public void Paint(Graphics g, PaintOptions paintOptions, Rectangle bounds, bool rightToLeft)
            {
            }
        }
    }
}
