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
using MbUnit.Framework.Kernel.Attributes;
using MbUnit.Framework.Kernel.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// Associates custom metadata with a test fixture, test method, test parameter
    /// or other test component.  The metadata can be used for documentation, classification
    /// or dynamic customization of tests.
    /// </summary>
    public class MetadataAttribute : MetadataPatternAttribute
    {
        private readonly string metadataKey;
        private readonly string metadataValue;

        /// <summary>
        /// Associates custom metadata with a test component.
        /// </summary>
        /// <param name="metadataKey">The metadata key</param>
        /// <param name="metadataValue">The metadata value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="metadataKey"/>
        /// or <paramref name="metadataValue"/> is null</exception>
        public MetadataAttribute(string metadataKey, string metadataValue)
        {
            if (metadataKey == null)
                throw new ArgumentNullException(@"metadataKey");
            if (metadataValue == null)
                throw new ArgumentNullException(@"metadataValue");

            this.metadataKey = metadataKey;
            this.metadataValue = metadataValue;
        }

        /// <summary>
        /// Gets the metadata key.
        /// </summary>
        public string MetadataKey
        {
            get { return metadataKey; }
        }

        /// <summary>
        /// Gets the metadata value.
        /// </summary>
        public string MetadataValue
        {
            get { return metadataValue; }
        }

        /// <inheritdoc />
        public override void Apply(TemplateTreeBuilder builder, ITemplateComponent component)
        {
            component.Metadata.Add(metadataKey, metadataValue);
        }
    }
}