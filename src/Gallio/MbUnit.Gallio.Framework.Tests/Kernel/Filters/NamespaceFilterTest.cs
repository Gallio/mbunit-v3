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
    [TestsOn(typeof(NamespaceFilter<IModelComponent>))]
    public class NamespaceFilterTest : BaseUnitTest
    {
        [RowTest]
        [Row(true, typeof(NamespaceFilterTest))]
        [Row(false, typeof(NamespaceFilter<IModelComponent>))]
        [Row(false, null)]
        public void IsMatchCombinations(bool expectedMatch, Type type)
        {
            CodeReference codeReference = type != null ? CodeReference.CreateFromType(type) : CodeReference.Unknown;

            IModelComponent component = Mocks.CreateMock<IModelComponent>();
            SetupResult.For(component.CodeReference).Return(codeReference);
            Mocks.ReplayAll();

            Assert.AreEqual(expectedMatch, new NamespaceFilter<IModelComponent>(typeof(NamespaceFilterTest).Namespace).IsMatch(component));
        }
    }
}