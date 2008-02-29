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
