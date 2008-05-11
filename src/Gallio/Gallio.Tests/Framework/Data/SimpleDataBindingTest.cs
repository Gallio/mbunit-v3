// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(SimpleDataBinding))]
    public class SimpleDataBindingTest
    {
        [Test]
        public void ConstructorWithPathAndIndex()
        {
            SimpleDataBinding binding = new SimpleDataBinding(42, "path");

            Assert.AreEqual("path", binding.Path);
            Assert.AreEqual(42, binding.Index);
        }

        [Test]
        public void ReplaceIndexCreatesANewInstanceWithTheNewIndex()
        {
            SimpleDataBinding oldBinding = new SimpleDataBinding(42, "path");
            DataBinding newBinding = oldBinding.ReplaceIndex(23);

            Assert.AreNotSame(oldBinding, newBinding);

            Assert.AreEqual("path", newBinding.Path);
            Assert.AreEqual(23, newBinding.Index);
        }

        [Test]
        new public void ToString()
        {
            Assert.AreEqual("Binding Index: <null>, Path: <null>",
                new SimpleDataBinding(null, null).ToString());
            Assert.AreEqual("Binding Index: 42, Path: 'foo'",
                new SimpleDataBinding(42, "foo").ToString());
        }

        [Test]
        public void EqualsAndHashCodeAreEqualForEqualBindings()
        {
            Assert.AreEqual(
                new SimpleDataBinding(1, "path"),
                new SimpleDataBinding(1, "path"));
            Assert.AreEqual(
                new SimpleDataBinding(1, "path").GetHashCode(),
                new SimpleDataBinding(1, "path").GetHashCode());
        }

        [Test]
        public void EqualsAndHashCodeAreDistinctForDifferentBindings()
        {
            InterimAssert.AreDistinct(
                new SimpleDataBinding(null, null),
                new SimpleDataBinding(0, null),
                new SimpleDataBinding(1, null),
                new SimpleDataBinding(null, "path"),
                new SimpleDataBinding(null, "path2"),
                new SimpleDataBinding(0, "path"),
                new SimpleDataBinding(1, "path2"),
                null
            );
        }
    }
}