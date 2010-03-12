// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

using Gallio.Icarus.Specifications;
using MbUnit.Framework;

namespace Gallio.Icarus.Tests.Specifications
{
    public class SpecificationFactoryTest
    {
        private SpecificationFactory factory;

        [SetUp]
        public void SetUp()
        {
            factory = new SpecificationFactory();
        }

        [Test]
        public void Create_should_return_a_name_spec_for_that_metadata_type()
        {
            var specification = factory.Create("Name", "text");

            Assert.IsInstanceOfType<NameSpecification>(specification);
        }

        [Test]
        public void Create_should_return_a_name_spec_with_the_correct_match_text()
        {
            const string searchText = "text";

            var specification = (NameSpecification)factory.Create("Name",
                searchText);

            Assert.AreEqual(searchText, specification.Name);
        }

        [Test]
        public void Create_should_return_a_namespace_spec_for_that_metadata_type()
        {    
            var specification = factory.Create("Namespace", "text");
            
            Assert.IsInstanceOfType<NamespaceSpecification>(specification);
        }

        [Test]
        public void Create_should_return_a_namespace_spec_with_the_correct_match_text()
        {
            const string searchText = "text";

            var specification = (NamespaceSpecification)factory.Create("Namespace", 
                searchText);

            Assert.AreEqual(searchText, specification.MatchText);
        }

        [Test]
        public void Create_should_return_a_metadata_spec_otherwise()
        {
            var specification = factory.Create("key", "value");

            Assert.IsInstanceOfType<MetadataSpecification>(specification);
        }

        [Test]
        public void Create_should_return_a_metadata_spec_with_the_correct_key()
        {
            const string metadataType = "key";

            var specification = (MetadataSpecification)factory.Create(metadataType,
                "value");

            Assert.AreEqual(metadataType, specification.MetadataKey);
        }

        [Test]
        public void Create_should_return_a_metadata_spec_with_the_correct_value()
        {
            const string metadataValue = "value";

            var specification = (MetadataSpecification)factory.Create("key",
                metadataValue);

            Assert.AreEqual(metadataValue, specification.MetadataValue);
        }
    }
}
