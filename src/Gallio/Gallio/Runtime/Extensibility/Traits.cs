using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// The base class for plugin, service or component traits.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Subclasses should include properties with getters and setters for binding
    /// configuration values associated with services (aka. traits).  They may also
    /// include methods and other service-specific functionality based on the traits.
    /// </para>
    /// <para>
    /// Traits objects are instantiated in the same way as other components.  The container
    /// injects required dependencies (on services or configuration values) in the constructor
    /// and injects optional dependencies into settable properties.
    /// </para>
    /// </remarks>
    public class Traits
    {
    }
}
