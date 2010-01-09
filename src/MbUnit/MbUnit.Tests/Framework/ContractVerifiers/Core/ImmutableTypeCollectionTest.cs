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

using System;
using System.Collections.Generic;
using System.Linq;
using Gallio.Model;
using Gallio.Runner.Reports;
using Gallio.Tests;
using Gallio.Tests.Integration;
using MbUnit.Framework;
using MbUnit.Framework.ContractVerifiers;
using System.Collections.ObjectModel;
using MbUnit.Framework.ContractVerifiers.Core;

namespace MbUnit.Tests.Framework.ContractVerifiers.Core
{
    [TestFixture]
    public class ImmutableTypeCollectionTest
    {
        public class Sample
        {
        }

        public class GenericSample<T>
        {
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Adds_null_type_should_throw_exception()
        {
            var collection = new ImmutableTypeCollection();
            collection.Add(null);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Contains_null_type_should_throw_exception()
        {
            var collection = new ImmutableTypeCollection();
            collection.Contains(null);
        }

        [Test]
        public void Contains_some_default_type()
        {
            var collection = new ImmutableTypeCollection();
            Assert.IsTrue(collection.Contains(typeof(string)));
        }

        [Test]
        public void Adds_type_ok()
        {
            var collection = new ImmutableTypeCollection();
            Assert.IsFalse(collection.Contains(typeof(Sample)));
            collection.Add(typeof(Sample));
            Assert.IsTrue(collection.Contains(typeof(Sample)));
        }

        [Test]
        public void Adds_generic_type_ok()
        {
            var collection = new ImmutableTypeCollection();
            Assert.IsFalse(collection.Contains(typeof(GenericSample<int>)));
            collection.Add(typeof(GenericSample<string>));
            Assert.IsTrue(collection.Contains(typeof(GenericSample<double>)));
        }
    }
}
