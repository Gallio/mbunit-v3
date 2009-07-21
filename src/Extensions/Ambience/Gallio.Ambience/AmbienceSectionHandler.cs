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
using System.Configuration;
using System.Xml;
using Gallio.Ambience.Impl;

namespace Gallio.Ambience
{
    /// <summary>
    /// Recognizes and processes the &lt;ambience&gt; configuration section.
    /// </summary>
    /// <example>
    /// Example configuration:
    /// <code><![CDATA[
    /// <?xml version="1.0" encoding="utf-8" ?>
    /// <configuration>
    ///   <configSections>
    ///     <section name="ambience" type="Gallio.Ambience.AmbienceSectionHandler, Gallio.Ambience" />
    ///   </configSections>
    ///
    ///   <ambience>
    ///     <defaultClient hostName="localhost" port="65436" userName="Test" password="Password" />
    ///   </ambience>
    /// </configuration>
    /// ]]></code>
    /// </example>
    public class AmbienceSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// The name of the Ambience section: "ambience".
        /// </summary>
        public const string SectionName = "ambience";

        /// <inheritdoc />
        public object Create(object parent, object configContext, XmlNode section)
        {
            return AmbienceConfigurationSection.ParseFromXml(section);
        }

        /// <summary>
        /// Gets the configuration section contents, or a default instance if none available.
        /// </summary>
        /// <returns>The configuration section.</returns>
        internal static AmbienceConfigurationSection GetSection()
        {
            return (AmbienceConfigurationSection)ConfigurationManager.GetSection(SectionName) ?? AmbienceConfigurationSection.CreateDefault();
        }
    }
}
