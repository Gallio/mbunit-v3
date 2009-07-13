// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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

using System;
using System.Reflection;
using Gallio.Model.Filters;
using Gallio.Model;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Model.Filters
{
    [TestFixture]
    [TestsOn(typeof(MemberFilter<ITestDescriptor>))]
    public class MemberFilterTest : BaseTestWithMocks
    {
        [Test]
        [Row(true, "A")]
        [Row(false, "B")]
        [Row(false, null)]
        public void IsMatchCombinations(bool expectedMatch, string memberName)
        {
            ICodeElementInfo codeElement = memberName != null
                ? Reflector.Wrap((MemberInfo) GetType().GetMethod(memberName, BindingFlags.Static | BindingFlags.NonPublic))
                : null;

            ITestDescriptor component = Mocks.StrictMock<ITestDescriptor>();
            SetupResult.For(component.CodeElement).Return(codeElement);
            Mocks.ReplayAll();

            Assert.AreEqual(expectedMatch,
                new MemberFilter<ITestDescriptor>(new EqualityFilter<string>("A")).IsMatch(component));
        }

        [Test]
        [Row("Member1")]
        [Row("Member2")]
        public void ToStringTest(string member)
        {
            MemberFilter<ITestDescriptor> filter = new MemberFilter<ITestDescriptor>(new EqualityFilter<string>(member));
            Assert.AreEqual("Member(Equality('" + member + "'))", filter.ToString());
        }

        internal static void A()
        {
        }

        internal static void B()
        {
        }
    }
}