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

using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;
using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Common.Xml;
using System;
using Rhino.Mocks;

namespace Gallio.Tests.Common.Diffing.Xml
{
    [TestFixture]
    [TestsOn(typeof(Diff))]
    public class DiffTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_diffType_should_throw_exception()
        {
            var mockPath = MockRepository.GenerateStub<IXmlPathStrict>();
            new Diff(null, mockPath, DiffTargets.Actual);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_path_should_throw_exception()
        {
            new Diff(DiffType.MissingElement, null, DiffTargets.Actual);
        }

        [Test]
        [ExpectedArgumentException]
        public void Constructs_with_empty_message_should_throw_exception()
        {
            var mockPath = MockRepository.GenerateStub<IXmlPathStrict>();
            new Diff(DiffType.MissingElement, mockPath, DiffTargets.Actual);
        }

        [Test]
        public void Constructs_ok()
        {
            var mockPath = MockRepository.GenerateStub<IXmlPathStrict>();
            var diff = new Diff(DiffType.MissingElement, mockPath, DiffTargets.Both);
            Assert.AreEqual(DiffType.MissingElement, diff.Type);
            Assert.AreSame(mockPath, diff.Path);
            Assert.AreEqual(DiffTargets.Both, diff.Targets);
        }

        [Test]
        public void Formats()
        {
            var mockPath = MockRepository.GenerateStub<IXmlPathStrict>();
            mockPath.Stub(x => x.ToString()).Return("All paths lead to Roma");
            var diff = new Diff(DiffType.MissingElement, mockPath, DiffTargets.Both);
            Assert.AreEqual("Missing element at 'All paths lead to Roma'.", diff.ToString());
        }
    }
}
