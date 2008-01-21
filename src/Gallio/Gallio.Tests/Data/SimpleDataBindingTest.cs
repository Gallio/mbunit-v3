// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Text;
using Gallio.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Data
{
    [TestFixture]
    [TestsOn(typeof(SimpleDataBinding))]
    public class SimpleDataBindingTest
    {
        [Test]
        public void ConstructorWithValueTypeOnly()
        {
            SimpleDataBinding binding = new SimpleDataBinding(typeof(int));

            Assert.AreEqual(typeof(int), binding.ValueType);
            Assert.IsNull(binding.Path);
            Assert.IsNull(binding.Index);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorWithValueTypeOnly_ThrowsIfValueTypeIsNull()
        {
            new SimpleDataBinding(null);
        }

        [Test]
        public void ConstructorWithPathAndIndex()
        {
            SimpleDataBinding binding = new SimpleDataBinding(typeof(int), "path", 42);

            Assert.AreEqual(typeof(int), binding.ValueType);
            Assert.AreEqual("path", binding.Path);
            Assert.AreEqual(42, binding.Index);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void ConstructorWithPathAndIndex_ThrowsIfValueTypeIsNull()
        {
            new SimpleDataBinding(null, "path", 42);
        }

        [Test]
        new public void ToString()
        {
            Assert.AreEqual("Binding ValueType: System.Int32, Path: <null>, Index: <null>",
                new SimpleDataBinding(typeof(int)).ToString());
            Assert.AreEqual("Binding ValueType: System.Int32, Path: 'foo', Index: 42",
                new SimpleDataBinding(typeof(int), "foo", 42).ToString());
        }
    }
}
