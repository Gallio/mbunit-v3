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
using System.Reflection;
using MbUnit.Framework.Kernel.Filters;
using MbUnit.Framework.Kernel.Model;
using Rhino.Mocks;

namespace MbUnit._Framework.Tests.Kernel.Filters
{
    [TestFixture]
    [TestsOn(typeof(MemberFilter<IModelComponent>))]
    public class MemberFilterTest : BaseUnitTest
    {
        [RowTest]
        [Row(true, "A")]
        [Row(false, "B")]
        [Row(false, null)]
        public void IsMatchCombinations(bool expectedMatch, string memberName)
        {
            CodeReference codeReference = memberName != null
                ? CodeReference.CreateFromMember(GetType().GetMethod(memberName, BindingFlags.Static | BindingFlags.NonPublic))
                : CodeReference.Unknown;

            IModelComponent component = Mocks.CreateMock<IModelComponent>();
            Expect.Call(component.CodeReference).Return(codeReference);
            Mocks.ReplayAll();

            Assert.AreEqual(expectedMatch, new MemberFilter<IModelComponent>("A").IsMatch(component));
        }

        internal static void A()
        {
        }

        internal static void B()
        {
        }
    }
}