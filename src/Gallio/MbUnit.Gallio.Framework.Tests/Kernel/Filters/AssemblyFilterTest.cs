// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit2::MbUnit.Framework;

using System;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Model;
using Rhino.Mocks;

namespace MbUnit._Framework.Tests.Kernel.Filters
{
    [TestFixture]
    [TestsOn(typeof(AssemblyFilter<IModelComponent>))]
    public class AssemblyFilterTest : BaseUnitTest, ITypeFilterTest
    {
        private IModelComponent component;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            CodeReference codeReference = CodeReference.CreateFromType(typeof(TypeFilterTest));
            component = Mocks.CreateMock<IModelComponent>();
            SetupResult.For(component.CodeReference).Return(codeReference);
            Mocks.ReplayAll();
        }

        [RowTest]
        [Row(true, typeof(TypeFilterTest))]
        [Row(false, typeof(Int32))]
        public void IsMatchWithFullName(bool expectedMatch, Type type)
        {
            Assert.AreEqual(expectedMatch, new AssemblyFilter<IModelComponent>(type.Assembly.FullName).IsMatch(component));
        }

        [RowTest]
        [Row(true, typeof(TypeFilterTest))]
        [Row(false, typeof(Int32))]
        public void IsMatchWithDisplayName(bool expectedMatch, Type type)
        {
            Assert.AreEqual(expectedMatch, new AssemblyFilter<IModelComponent>(type.Assembly.GetName().Name).IsMatch(component));
        }
    }
}