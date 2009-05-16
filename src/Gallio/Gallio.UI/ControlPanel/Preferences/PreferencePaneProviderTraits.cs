using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.UI.ControlPanel.Preferences
{
    /// <summary>
    /// Describes the traits of a <see cref="IPreferencePaneProvider" />.
    /// </summary>
    public class PreferencePaneProviderTraits : Traits
    {
        private readonly string path;

        /// <summary>
        /// Creates a traits object for a preference pane provider.
        /// </summary>
        /// <param name="path">The preference pane path consisting of slash-delimited name segments
        /// specifying tree nodes</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is empty</exception>
        public PreferencePaneProviderTraits(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentException("The preference pane path must not be empty.", "path");

            this.path = path;
        }

        /// <summary>
        /// Gets the preference pane path consisting of slash-delimited name segments
        /// specifying tree nodes.
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Gets or sets an icon (16x16) for the preference pane, or null if none.
        /// </summary>
        public Icon Icon { get; set; }

        /// <summary>
        /// Gets or sets an integer index used to sort control panel tabs in ascending order.
        /// </summary>
        public int Order { get; set; }
    }
}
