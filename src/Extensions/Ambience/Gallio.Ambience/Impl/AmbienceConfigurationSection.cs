using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gallio.Ambience.Impl
{
    /// <summary>
    /// Provides information from the Ambience configuration section.
    /// </summary>
    /// <seealso cref="AmbienceSectionHandler"/>
    internal sealed class AmbienceConfigurationSection
    {
        private AmbienceConfigurationSection()
        {
            DefaultClientConfiguration = new AmbienceClientConfiguration();
        }

        public static AmbienceConfigurationSection CreateDefault()
        {
            return new AmbienceConfigurationSection();
        }

        public static AmbienceConfigurationSection ParseFromXml(XmlNode section)
        {
            AmbienceConfigurationSection result = new AmbienceConfigurationSection();

            var rootElement = (XmlElement) section;
            var defaultClientElement = (XmlElement)rootElement.SelectSingleNode("defaultClient");
            if (defaultClientElement != null)
            {
                string hostName = defaultClientElement.GetAttribute("hostName");
                if (!string.IsNullOrEmpty(hostName))
                    result.DefaultClientConfiguration.HostName = hostName;

                string port = defaultClientElement.GetAttribute("port");
                if (!string.IsNullOrEmpty(port))
                    result.DefaultClientConfiguration.Port = int.Parse(port, CultureInfo.InvariantCulture);

                string userName = defaultClientElement.GetAttribute("userName");
                if (!string.IsNullOrEmpty(userName))
                    result.DefaultClientConfiguration.Credential.UserName = userName;

                string password = defaultClientElement.GetAttribute("password");
                if (!string.IsNullOrEmpty(password))
                    result.DefaultClientConfiguration.Credential.Password = password;
            }

            return result;
        }

        public AmbienceClientConfiguration DefaultClientConfiguration { get; private set; }
    }
}
