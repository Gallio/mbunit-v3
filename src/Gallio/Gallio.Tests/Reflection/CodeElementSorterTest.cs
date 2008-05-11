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

using System;
using System.Collections.Generic;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(CodeElementSorter))]
    public class CodeElementSorterTest
    {
        [Test]
        public void SortMembersByDeclaringType()
        {
            List<IMethodInfo> members = new List<IMethodInfo>();
            members.Add(Reflector.Wrap(typeof(A).GetMethod("MemberA")));
            members.Add(Reflector.Wrap(typeof(C).GetMethod("MemberC")));
            members.Add(Reflector.Wrap(typeof(B).GetMethod("MemberB")));

            IList<IMethodInfo> sortedMembers = CodeElementSorter.SortMembersByDeclaringType(members);
            CollectionAssert.AreElementsEqual(new IMethodInfo[] { members[0], members[2], members[1] }, sortedMembers);
        }

        private class A
        {
            public void MemberA()
            {
            }
        }

        private class B : A
        {
            public void MemberB()
            {
            }
        }

        private class C : B
        {
            public void MemberC()
            {
            }
        }
    }
}
