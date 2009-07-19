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
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Collections;
using Gallio.Common.Normalization;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace Gallio.Model.Schema
{
    /// <summary>
    /// Describes a test parameter in a portable manner for serialization.
    /// </summary>
    /// <seealso cref="TestParameter"/>
    [Serializable]
    [XmlType(Namespace=SchemaConstants.XmlNamespace)]
    public sealed class TestParameterData : TestComponentData, INormalizable<TestParameterData>
    {
        /// <summary>
        /// Creates an uninitialized instance for Xml deserialization.
        /// </summary>
        private TestParameterData()
        {
        }

        /// <summary>
        /// Creates a parameter data object.
        /// </summary>
        /// <param name="id">The component id.</param>
        /// <param name="name">The component name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or
        /// <paramref name="name"/> is null.</exception>
        public TestParameterData(string id, string name)
            : base(id, name)
        {
        }

        /// <summary>
        /// Copies the contents of a test parameter.
        /// </summary>
        /// <param name="source">The source test parameter.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null.</exception>
        public TestParameterData(TestParameter source)
            : base(source)
        {
        }

        /// <summary>
        /// Recreates a <see cref="TestParameter" /> object from the test data.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Because a test parameter data object does not contain all of the details of the original
        /// test parameter some information may be lost in the round trip.
        /// </para>
        /// </remarks>
        /// <returns>The test parameter.</returns>
        public TestParameter ToTestParameter()
        {
            var testParameter = new TestParameter(Name, CodeElement)
            {
                Id = Id                
            };

            testParameter.Metadata.Clear();
            testParameter.Metadata.AddAll(Metadata);
            return testParameter;
        }

        /// <inheritdoc />
        public TestParameterData Normalize()
        {
            string normalizedId = ModelNormalizationUtils.NormalizeTestComponentId(Id);
            string normalizedName = ModelNormalizationUtils.NormalizeTestComponentName(Name);
            CodeLocation normalizedCodeLocation = CodeLocation.Normalize();
            CodeReference normalizedCodeReference = CodeReference.Normalize();
            PropertyBag normalizedMetadata = ModelNormalizationUtils.NormalizeMetadata(Metadata);

            if (ReferenceEquals(Id, normalizedId)
                && ReferenceEquals(Name, normalizedName)
                && CodeLocation == normalizedCodeLocation
                && CodeReference == normalizedCodeReference
                && ReferenceEquals(Metadata, normalizedMetadata))
                return this;

            return new TestParameterData(normalizedId, normalizedName)
            {
                CodeElement = CodeElement,
                CodeLocation = normalizedCodeLocation,
                CodeReference = normalizedCodeReference,
                Metadata = normalizedMetadata
            };
        }
    }
}