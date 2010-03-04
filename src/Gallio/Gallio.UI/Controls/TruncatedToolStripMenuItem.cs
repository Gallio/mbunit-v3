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

using System.Text;

namespace Gallio.UI.Controls
{
    /// <summary>
    /// A menu item that truncates the text, using ellipses (...), if necessary.
    /// </summary>
    public class TruncatedToolStripMenuItem : ToolStripMenuItem
    {
        ///<summary>
        /// Ctor.
        ///</summary>
        ///<param name="text">The text for the menu item to display. 
        /// The full text will be the tooltip, but the display text
        /// will be truncated if longer than specified.</param>
        ///<param name="width">The maximum length text to display.</param>
        public TruncatedToolStripMenuItem(string text, int width)
        {
            AutoToolTip = false;
            ToolTipText = text;
            Text = text.Length > width ? TruncateText(text, width) : text;
        }

        private static string TruncateText(string text, int length)
        {
            // NOTE: You need to create the builder with the required capacity before calling function.
            // See http://msdn.microsoft.com/en-us/library/aa446536.aspx
            var stringBuilder = new StringBuilder(length + 1);
            Native.PathCompactPathEx(stringBuilder, text, length + 1, 0); // + 1 for null terminator included in StringBuilder capacity
            return stringBuilder.ToString();
        }
    }
}