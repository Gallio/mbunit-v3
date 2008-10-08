using System;
using System.Configuration;
using System.Xml;

namespace Gallio.Ambience
{
    /// <summary>
    /// <para>
    /// Recognizes and processes the &lt;ambience&gt; configuration section.
    /// </para>
    /// </summary>
    public class AmbienceConfigurationSectionHandler : IConfigurationSectionHandler
    {
        /// <inheritdoc />
        public object Create(object parent, object configContext, XmlNode section)
        {
            throw new NotImplementedException();
        }
    }
}
