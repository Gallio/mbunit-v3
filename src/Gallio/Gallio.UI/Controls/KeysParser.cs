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
using System.Windows.Forms;

namespace Gallio.UI.Controls
{
    /// <inheritdoc />
    public class KeysParser : IKeysParser
    {
        /// <inheritdoc />
        public Keys Parse(string shortcut)
        {
            if (String.IsNullOrEmpty(shortcut))
                return Keys.None;

            if (HasModifier(shortcut))
            {
                var index = shortcut.IndexOf("+");
                var left = Parse(shortcut.Substring(0, index));
                var right = Parse(shortcut.Substring(index + 1));
                return left | right;
            }

            return ParseShortcut(shortcut);
        }

        private static bool HasModifier(string shortcut)
        {
            return shortcut.Contains("+");
        }

        private static Keys ParseShortcut(string shortcut)
        {
            if (shortcut.ToLower() == "ctrl")
            {
                return Keys.Control;
            }

            Keys key;
            try
            {
                key = ParseEnum<Keys>(shortcut);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Failed to parse shortcut string: {0}", 
                                                  shortcut), ex);
            }
            return key;
        }

        private static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof (T), value, true);
        }
    }
}
