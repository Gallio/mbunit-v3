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

using Gallio.Reflection;
using Gallio.Tests;
using MbUnit.Framework;

using System;
using Gallio.Model.Filters;
using Gallio.Model;
using Rhino.Mocks;
using ITestComponent=Gallio.Model.ITestComponent;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(AssemblyFilter<ITestComponent>))]
    public class AssemblyFilterTest : BaseTestWithMocks, ITypeFilterTest
    {
        private ITestComponent component;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            ICodeElementInfo codeElement = Reflector.Wrap(typeof(TypeFilterTest));
            component = Mocks.CreateMock<ITestComponent>();
            SetupResult.For(component.CodeElement).Return(codeElement);
            Mocks.ReplayAll();
        }

        [Test]
        [Row(true, typeof(TypeFilterTest))]
        [Row(false, typeof(Int32))]
        public void IsMatchWithFullName(bool expectedMatch, Type type)
        {
            Assert.AreEqual(expectedMatch, new AssemblyFilter<ITestComponent>(
                new EqualityFilter<string>(type.Assembly.FullName)).IsMatch(component));
        }

        [Test]
        [Row(true, typeof(TypeFilterTest))]
        [Row(false, typeof(Int32))]
        public void IsMatchWithDisplayName(bool expectedMatch, Type type)
        {
            Assert.AreEqual(expectedMatch, new AssemblyFilter<ITestComponent>(
                new EqualityFilter<string>(type.Assembly.GetName().Name)).IsMatch(component));
        }

        [Test]
        [Row(true, typeof(TypeFilterTest))]
        [Row(false, typeof(Int32))]
        public void ToStringTest(bool expectedMatch, Type type)
        {
            string assemblyName = type.Assembly.GetName().Name;
            AssemblyFilter<ITestComponent> filter = new AssemblyFilter<ITestComponent>(
                new EqualityFilter<string>(assemblyName));
            Assert.AreEqual("Assembly(Equality('" + assemblyName + "'))", filter.ToString());
        }
    }
}