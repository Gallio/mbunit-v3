using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.UI.ControlPanel
{
    /// <summary>
    /// Describes the traits of a <see cref="IControlPanelTabProvider" />.
    /// </summary>
    public class ControlPanelTabProviderTraits : Traits
    {
        private readonly string name;

        /// <summary>
        /// Creates a traits object for a control panel tab provider.
        /// </summary>
        /// <param name="name">The control panel tab name</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty</exception>
        public ControlPanelTabProviderTraits(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("The control panel tab name must not be empty.", "name");

            this.name = name;
        }

        /// <summary>
        /// Gets the control panel tab name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets or sets an integer index used to sort control panel tabs in ascending order.
        /// </summary>
        public int Order { get; set; }
    }
}
