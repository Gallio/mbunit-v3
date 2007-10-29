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

using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Tests.Framework.Reflection
{
    public class SampleClass
    {
        #region Fields

#pragma warning disable 0414
        private int privateInt = 7;
#pragma warning restore 0414

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
