using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Tests.Reflection
{
    public class TestSample : BaseSample
    {
        public string publicString = "MbUnit Rocks!!!";
        private DateTime privateDateTime = DateTime.Today;
        internal static int staticNum = 7;

        public string PublicProperty
        {
            get { return publicString; }
            set { publicString = value; }
        }

        internal DateTime InternalProperty
        {
            get { return privateDateTime; }
            set { privateDateTime = value; }
        }

        protected static int StaticProperty
        {
            get { return staticNum; }
            set { staticNum = value; }
        }

        public int Pow(int x)
        {
            return Multiply(x, x);
        }

        public string PraiseMe()
        {
            return publicString;
        }

        private int Multiply(int x, int y)
        {
            return x * y;
        }

        public static int Add(int x, int y)
        {
            return x + y;
        }
    }
}