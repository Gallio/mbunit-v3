using System;
using System.Collections.Generic;
using System.Text;

using MbUnit.Framework;
using System.Diagnostics;

namespace MbUnit.Framework.Tests
{
    [TestFixture]
    public class CombinatorialFixture
    {
        //[CombinatorialTest(CombinationType.AllPairs)]
        //public void AllPairs(
        //    [UsingLinear(0, 3)]int arg0,
        //    [UsingLinear(0, 3)]int arg1
        //    )
        //{
        //    Trace.WriteLine("arg0=" + arg0 + ", arg1=" + arg1);

        //    Assert.Between(arg0, 0, 3);
        //    Assert.Between(arg1, 0, 3);
        //}

        //[CombinatorialTest]
        //public void UsingLinears(
        //    [UsingLinear(0, 3)]int arg0,
        //    [UsingLinear(0, 3)]int arg1
        //    )
        //{
        //    Trace.WriteLine("arg0=" + arg0 + ", arg1=" + arg1);

        //    Assert.Between(arg0, 0, 3);
        //    Assert.Between(arg1, 0, 3);
        //}

        //[CombinatorialTest]
        //public void UsingLiterals(
        //    [UsingLiterals("a;b;c")]char a,
        //    [UsingLiterals("a;b;c")]char b
        //    )
        //{
        //    Trace.WriteLine("a=" + a + ", b=" + b);

        //    char[] cs = new char[] { 'a', 'b', 'c' };
        //    Assert.In(a, cs);
        //    Assert.In(b, cs);
        //}

        //public enum MyEnum
        //{
        //    One,
        //    Two,
        //    Three
        //}

        //[CombinatorialTest]
        //public void UsingEnums(
        //    [UsingEnum(typeof(MyEnum))]MyEnum a,
        //    [UsingEnum(typeof(MyEnum))]MyEnum b
        //    )
        //{
        //    Trace.WriteLine("a=" + a + ", b=" + b);

        //    Assert.In(a, Enum.GetValues(typeof(MyEnum)));
        //    Assert.In(b, Enum.GetValues(typeof(MyEnum)));
        //}

        [Factory]
        public int[] IntFactory()
        {
            return new int[] { 1, 2, 3 };
        }

        [Factory(typeof(int))]
        public Array WeakIntFactory()
        {
            return new int[] { 1, 2, 3 };
        }

        [CombinatorialTest]
        public void UsingFactories(
            [UsingFactories("IntFactory")]int a,
            [UsingFactories("WeakIntFactory")]int b
            )
        {
            Assert.Between(a, 1, 3);
            Assert.Between(b, 1, 3);
        }

        public class DomainFactory
        {
            [Factory]
            public int[] IntFactory()
            {
                return new int[] { 1, 2, 3 };
            }

            [Factory(typeof(int))]
            public Array WeakIntFactory()
            {
                return new int[] { 1, 2, 3 };
            }
        }

        [CombinatorialTest]
        public void UsingMultipleFactories(
            [UsingFactories("WeakIntFactory;IntFactory")]int a,
            [UsingFactories("WeakIntFactory")]int b
            )
        {
            Assert.Between(a, 1, 3);
            Assert.Between(b, 1, 3);
        }

        [CombinatorialTest]
        public void UsingTypeFactories(
            [UsingFactories(typeof(DomainFactory))]int a,
           [UsingFactories(typeof(DomainFactory))]int b
           )
        {
            Assert.Between(a, 1, 3);
            Assert.Between(b, 1, 3);
        }
    }
}
