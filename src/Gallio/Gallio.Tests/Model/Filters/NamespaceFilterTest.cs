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

using Gallio.Common.Reflection;
using Gallio.Tests;
using MbUnit.Framework;

using System;
using Gallio.Model.Filters;
using Gallio.Model;
using Rhino.Mocks;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(NamespaceFilter<ITestDescriptor>))]
    public class NamespaceFilterTest : BaseTestWithMocks
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullArgument()
        {
            new NamespaceFilter<ITestDescriptor>(null);
        }

        [Test]
        [Row(true, typeof(NamespaceFilterTest))]
        [Row(false, typeof(NamespaceFilter<ITestDescriptor>))]
        [Row(false, null)]
        public void IsMatchCombinations(bool expectedMatch, Type type)
        {
            ICodeElementInfo codeElement = type != null ? Reflector.Wrap(type) : null;

            ITestDescriptor component = Mocks.StrictMock<ITestDescriptor>();
            SetupResult.For(component.CodeElement).Return(codeElement);
            Mocks.ReplayAll();

            Assert.AreEqual(expectedMatch, new NamespaceFilter<ITestDescriptor>(
                new EqualityFilter<string>(typeof(NamespaceFilterTest).Namespace)).IsMatch(component));
        }

        [Test]
        [Row(typeof(NamespaceFilterTest))]
        [Row(typeof(NamespaceFilter<ITestDescriptor>))]
        //[Row(null)]
        public void ToStringTest(Type type)
        {
            string namespaceName = type.Namespace;
            NamespaceFilter<ITestDescriptor> filter = new NamespaceFilter<ITestDescriptor>(
                new EqualityFilter<string>(namespaceName));
            Assert.AreEqual("Namespace(Equality('" + namespaceName + "'))", filter.ToString());
        }
    }
}