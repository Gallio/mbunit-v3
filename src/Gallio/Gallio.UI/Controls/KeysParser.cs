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