// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using System.Xml.Serialization;

namespace Gallio.Model.Serialization
{
    /// <summary>
    /// Describes a test package in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="TestPackage"/>
    [Serializable]
    [XmlRoot("testPackage", Namespace = SerializationUtils.XmlNamespace)]
    [XmlType(Namespace = SerializationUtils.XmlNamespace)]
    public sealed class TestPackageData : TestComponentData
    {
        private readonly List<string> assemblyFullNames;
        private TestPackageConfig config;

        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestPackageData()
        {
            assemblyFullNames = new List<string>();
        }

        /// <summary>
        /// Copies the contents of a test package.
        /// </summary>
        /// <param name="source">The source test package</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TestPackageData(TestPackage source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            config = source.Config;
            assemblyFullNames = new List<string>();
        }

        /// <summary>
        /// Gets or sets the test package configuration.
        /// </summary>
        [XmlElement("config", IsNullable=false)]
        public TestPackageConfig Config
        {
            get { return config; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                config = value;
            }
        }

        /// <summary>
        /// Gets the full names of the assemblies that belong to the test package.
        /// </summary>
        [XmlArray("assemblyFullNames", IsNullable = false)]
        [XmlArrayItem("assemblyFullNames", typeof(string), IsNullable = false)]
        public List<string> AssemblyFullNames
        {
            get { return assemblyFullNames; }
        }
    }
}
