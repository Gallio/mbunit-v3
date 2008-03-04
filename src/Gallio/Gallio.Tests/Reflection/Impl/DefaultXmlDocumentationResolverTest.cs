// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Castle.Core.Logging;
using Gallio.Hosting;
using Gallio.Reflection.Impl;
using Gallio.Tests;
using MbUnit.TestResources;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection.Impl
{
    [TestFixture]
    [Author("Jeff")]
    [TestsOn(typeof(DefaultXmlDocumentationResolver))]
    public class DefaultXmlDocumentationResolverTest : BaseUnitTest
    {
        private DefaultXmlDocumentationResolver resolver;

        public override void SetUp()
        {
            base.SetUp();

            resolver = new DefaultXmlDocumentationResolver();
        }

        [Test, ExpectedArgumentNullException]
        public void GetXmlDocumentation_ThrowsIfAssemblyPathIsNull()
        {
            resolver.GetXmlDocumentation(null, "foo");
        }

        [Test, ExpectedArgumentNullException]
        public void GetXmlDocumentation_ThrowsIfMemberIdIsNull()
        {
            resolver.GetXmlDocumentation("foo", null);
        }

        [Test]
        public void GetXmlDocumentation_ReturnsNullIfAssemblyDoesNotExist()
        {
            Assert.IsNull(resolver.GetXmlDocumentation("NoSuchAssembly", "T:AType"));
        }

        [Test]
        public void GetXmlDocumentation_GetsDocumentationForDocumentedMember()
        {
            Type type = typeof(DocumentedClass);
            Assert.AreEqual("<summary>\nA documented class.\n</summary>\n<remarks>\nThe XML documentation of this test is significant.\n  Including the leading whitespace on this line.\n    And the extra 8 trailing spaces on this line!\n</remarks>",
                resolver.GetXmlDocumentation(Loader.GetAssemblyLocalPath(type.Assembly), "T:" + type.FullName));
        }

        [Test]
        public void GetXmlDocumentation_ReturnsNullForUndocumentedMember()
        {
            Type type = typeof(DocumentedClass.UndocumentedNestedClass);
            Assert.IsNull(resolver.GetXmlDocumentation(Loader.GetAssemblyLocalPath(type.Assembly), "T:" + type.FullName));
        }
    }
}
