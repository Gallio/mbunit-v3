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

using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using Gallio.Tests.Common.Xml.Diffing;
using MbUnit.Framework;
using Gallio.Common.Xml;
using Gallio.Common;
using System.Collections.Generic;
using System;
using Rhino.Mocks;

namespace Gallio.Tests.Common.Xml
{
    [TestFixture]
    [TestsOn(typeof(NodeCollection))]
    public class NodeCollectionTest : DiffableTestBase
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_initializer_should_throw_exception()
        {
            new NodeCollection(null);
        }

        [Test]
        public void Default_empty()
        {
            var collection = NodeCollection.Empty;
            Assert.IsEmpty(collection);
            Assert.Count(0, collection);
        }

        [Test]
        public void Constructs_ok()
        {
            var mockMarkup1 = MockRepository.GenerateStub<INode>();
            var mockMarkup2 = MockRepository.GenerateStub<INode>();
            var mockMarkup3 = MockRepository.GenerateStub<INode>();
            var array = new[] { mockMarkup1, mockMarkup2, mockMarkup3 };
            var collection = new NodeCollection(array);
            Assert.Count(3, collection);
            Assert.AreElementsSame(array, collection);
        }


    
    
    }
}
