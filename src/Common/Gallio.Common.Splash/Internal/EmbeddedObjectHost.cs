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
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gallio.Common.Splash.Internal
{
    internal sealed class EmbeddedObjectHost : IDisposable, IEmbeddedObjectSite
    {
        private readonly EmbeddedObject embeddedObject;
        private readonly Control parentControl;
        private readonly Style paragraphStyle;
        private readonly Style inlineStyle;
        private readonly int charIndex;
        private IEmbeddedObjectClient embeddedObjectClient;

        private EmbeddedObjectMeasurements? cachedMeasurements;
        private Rectangle? currentBounds;
        private bool currentRightToLeft;

        public EmbeddedObjectHost(EmbeddedObject embeddedObject, int charIndex,
            Control parentControl, Style paragraphStyle, Style inlineStyle)
        {
            this.embeddedObject = embeddedObject;
            this.charIndex = charIndex;
            this.parentControl = parentControl;
            this.paragraphStyle = paragraphStyle;
            this.inlineStyle = inlineStyle;
        }

        public EmbeddedObject EmbeddedObject
        {
            get { return embeddedObject; }
        }

        public int CharIndex
        {
            get { return charIndex; }
        }

        public Control ParentControl
        {
            get { return parentControl; }
        }

        public Style ParagraphStyle
        {
            get { return paragraphStyle; }
        }

        public Style InlineStyle
        {
            get { return inlineStyle; }
        }

        /// <summary>
        /// Gets or sets the layout bounds relative to the layout origin.
        /// This is set during the main layout operation and cached until relayout occurs.
        /// </summary>
        public Rectangle LayoutBounds { get; set; }

        public void CreateClient()
        {
            if (embeddedObjectClient != null)
                return;

            try
            {
                embeddedObjectClient = embeddedObject.CreateClient(this);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void Dispose()
        {
            if (embeddedObjectClient == null)
                return;

            try
            {
                embeddedObjectClient.Dispose();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                embeddedObjectClient = null;
            }
        }

        public bool RequiresPaint
        {
            get
            {
                if (embeddedObjectClient == null)
                    return false;

                try
                {
                    return embeddedObjectClient.RequiresPaint;
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                    return false;
                }
            }
        }

        public EmbeddedObjectMeasurements Measure()
        {
            if (embeddedObjectClient == null)
                CreateDefaultEmbeddedObjectMeasurements();

            if (!cachedMeasurements.HasValue)
            {
                try
                {
                    cachedMeasurements = embeddedObjectClient.Measure();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                    cachedMeasurements = CreateDefaultEmbeddedObjectMeasurements();
                }
            }

            return cachedMeasurements.Value;
        }

        public void Paint(Graphics g, PaintOptions paintOptions, Rectangle bounds, bool rightToLeft)
        {
            if (embeddedObjectClient == null)
                return;

            try
            {
                embeddedObjectClient.Paint(g, paintOptions, bounds, rightToLeft);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        public void Show(Rectangle bounds, bool rightToLeft)
        {
            if (embeddedObjectClient == null)
                return;

            if (currentRightToLeft != rightToLeft
                || !currentBounds.HasValue || currentBounds.Value != bounds)
            {
                currentBounds = bounds;
                currentRightToLeft = rightToLeft;

                try
                {
                    embeddedObjectClient.Show(bounds, rightToLeft);
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public void Hide()
        {
            if (embeddedObjectClient == null)
                return;

            if (currentBounds.HasValue)
            {
                currentBounds = null;

                try
                {
                    embeddedObjectClient.Hide();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        private static void HandleException(Exception ex)
        {
            Debug.WriteLine(string.Format("An exception occurred while interacting with an embedded object.\n{0}", ex));
        }

        public static EmbeddedObjectMeasurements CreateDefaultEmbeddedObjectMeasurements()
        {
            return new EmbeddedObjectMeasurements(new Size(0, 0));
        }
    }
}
