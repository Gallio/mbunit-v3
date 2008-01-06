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

extern alias MbUnit2;
using Gallio.Reflection;
using Gallio.Tests;
using MbUnit2::MbUnit.Framework;

using System;
using System.Reflection;
using Gallio.Model.Filters;
using Gallio.Model;
using Rhino.Mocks;
using ITestComponent=Gallio.Model.ITestComponent;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(TypeFilter<ITestComponent>))]
    public class TypeFilterTest : BaseUnitTest, ITypeFilterTest
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

        [RowTest]
        [Row(true, typeof(TypeFilterTest), false)]
        [Row(true, typeof(TypeFilterTest), true)]
        [Row(false, typeof(BaseUnitTest), false)]
        [Row(true, typeof(BaseUnitTest), true)]
        [Row(false, typeof(ITypeFilterTest), false)]
        [Row(true, typeof(ITypeFilterTest), true)]
        public void IsMatchWithAssemblyQualifiedName(bool expectedMatch, Type type, bool includeDerivedTypes)
        {
            Assert.AreEqual(expectedMatch,
                new TypeFilter<ITestComponent>(new EqualityFilter<string>(type.AssemblyQualifiedName), includeDerivedTypes).IsMatch(component));
        }

        [RowTest]
        [Row(true, typeof(TypeFilterTest), false)]
        [Row(true, typeof(TypeFilterTest), true)]
        [Row(false, typeof(BaseUnitTest), false)]
        [Row(true, typeof(BaseUnitTest), true)]
        [Row(false, typeof(ITypeFilterTest), false)]
        [Row(true, typeof(ITypeFilterTest), true)]
        public void IsMatchWithFullName(bool expectedMatch, Type type, bool includeDerivedTypes)
        {
            Assert.AreEqual(expectedMatch,
                new TypeFilter<ITestComponent>(new EqualityFilter<string>(type.FullName), includeDerivedTypes).IsMatch(component));
        }

        [RowTest]
        [Row(true, typeof(TypeFilterTest), false)]
        [Row(true, typeof(TypeFilterTest), true)]
        [Row(false, typeof(BaseUnitTest), false)]
        [Row(true, typeof(BaseUnitTest), true)]
        [Row(false, typeof(ITypeFilterTest), false)]
        [Row(true, typeof(ITypeFilterTest), true)]
        public void IsMatchWithName(bool expectedMatch, Type type, bool includeDerivedTypes)
        {
            Assert.AreEqual(expectedMatch,
                new TypeFilter<ITestComponent>(new EqualityFilter<string>(type.Name), includeDerivedTypes).IsMatch(component));
        }
    }

    /// <summary>
    /// This interface is implemented to assist with checking whether the option
    /// to include derived types correctly handles interfaces.
    /// </summary>
    internal interface ITypeFilterTest
    {
    }
}