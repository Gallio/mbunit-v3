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
