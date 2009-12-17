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
    /// Provides style parameters for painting.
    /// </summary>
    public class PaintOptions
    {
        /// <summary>
        /// Initializes paint options to system defaults.
        /// </summary>
        public PaintOptions()
        {
            BackgroundColor = SystemColors.Window;
            SelectedTextColor = SystemColors.HighlightText;
            SelectedBackgroundColor = SystemColors.Highlight;
        }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Color BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the selected text color.
        /// </summary>
        public Color SelectedTextColor { get; set; }

        /// <summary>
        /// Gets or sets the selected background color.
        /// </summary>
        public Color SelectedBackgroundColor { get; set; }
    }
}
