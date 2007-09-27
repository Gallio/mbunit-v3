using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Tests.Reflection
{
    public class SampleClass
    {
        #region Fields

        private int privateInt = 7;

        #endregion

        #region Properties

        private string PrivateStringProperty
        {
            get { return "Test"; }
        }

        #endregion

        #region Methods
        private int PrivateIntWithNoParam()
        {
            return 5;
        }

        private int PrivateIntWithIntParam(int n)
        {
            return n;
        }

        private string PrivateOverloaded()
        {
            return "Test";
        }

        private string PrivateOverloaded(string s)
        {
            return s;
        }

        private void PrivateVoidWithNoParam()
        {
        }

        private int PrivateIntWithDoubleParam(double d)
        {
            return (int)d;
        }


        private static bool PrivateStaticBoolWithBoolParam(bool b)
        {
            return b;
        }

        internal int InternalIntWithIntParam(int n)
        {
            return n;
        }
        #endregion
    }
}
