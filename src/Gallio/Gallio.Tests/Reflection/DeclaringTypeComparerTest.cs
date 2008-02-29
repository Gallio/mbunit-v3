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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gallio.Reflection;
using MbUnit.Framework;

namespace Gallio.Tests.Reflection
{
    [TestFixture]
    [TestsOn(typeof(DeclaringTypeComparer<>))]
    public class DeclaringTypeComparerTest
    {
        [Test]
        [Row(typeof(A), typeof(A), 0)]
        [Row(typeof(A), typeof(B), -1)]
        [Row(typeof(A), typeof(C), -1)]
        [Row(typeof(B), typeof(A), 1)]
        [Row(typeof(B), typeof(B), 0)]
        [Row(typeof(B), typeof(C), -1)]
        [Row(typeof(C), typeof(A), 1)]
        [Row(typeof(C), typeof(B), 1)]
        [Row(typeof(C), typeof(C), 0)]
        public void Compare(Type x, Type y, int expectedResult)
        {
            IMethodInfo xm = Reflector.Wrap(x.GetMethod("Member" + x.Name));
            IMethodInfo ym = Reflector.Wrap(y.GetMethod("Member" + y.Name));

            Assert.AreEqual(expectedResult, DeclaringTypeComparer<IMethodInfo>.Instance.Compare(xm, ym));
        }

        [Test]
        [Row(typeof(DeclaringTypeComparerTest), typeof(A), -1)]
        [Row(typeof(DeclaringTypeComparerTest), typeof(DeclaringTypeComparerTest), 0)]
        [Row(typeof(A), typeof(DeclaringTypeComparerTest), 1)]
        public void CompareSortsNullDeclaringTypesBeforeOtherTypes(Type x, Type y, int expectedResult)
        {
            ITypeInfo xm = Reflector.Wrap(x);
            ITypeInfo ym = Reflector.Wrap(y);

            // Comparing types and nested types can produce null declaring types.
            Assert.AreEqual(expectedResult, DeclaringTypeComparer<ITypeInfo>.Instance.Compare(xm, ym));
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
