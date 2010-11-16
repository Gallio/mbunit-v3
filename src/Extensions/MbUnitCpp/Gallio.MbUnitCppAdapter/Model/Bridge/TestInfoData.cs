// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Diagnostics;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// The structure that warps native information about a MbUnitCpp test or a MbUnitCpp fixture.
    /// </summary>
    public class TestInfoData
    {
        private readonly NativeTestInfoData native;
        private string name;
        private string fileName;

        public TestInfoData(NativeTestInfoData native)
        {
            this.native = native;
        }

        /// <summary>
        /// Returns a unique key identifying the MbUnitCpp test/fixture that may be used to build the model tree.
        /// </summary>
        /// <returns>A unique key name.</returns>
        public string GetId()
        {
            return "MbUnitCpp_" + ((native.Position.pTest != IntPtr.Zero) ? native.Position.pTest : native.Position.pTestFixture);
        }

        /// <summary>
        /// Marshals the test/fixture name.
        /// </summary>
        public string Name
        {
            get
            {
                if (name == null)
                    name = Marshal.PtrToStringUni(native.NamePtr);

                return name;
            }
        }

        /// <summary>
        /// Marshals the source file name where the test/fixture is defined.
        /// </summary>
        public string FileName
        {
            get
            {
                if (fileName == null)
                    fileName = Marshal.PtrToStringUni(native.FileNamePtr);

                return fileName;
            }
        }

        /// <summary>
        /// Determines whether the current info describes a test fixture or a test case.
        /// </summary>
        public bool IsTestFixture
        {
            get
            {
                return native.IsTestFixture;
            }
        }

        /// <summary>
        /// Gets the native unmanaged structure.
        /// </summary>
        public NativeTestInfoData Native
        {
            get
            {
                return native;
            }
        }

        /// <summary>
        /// Builds and returns an artifical stack trace that points to the current test/fixture.
        /// </summary>
        /// <returns></returns>
        public StackTraceData GetStackTraceData()
        {
            return new StackTraceData(String.Format("   at {0} in {1}:line {2}\r\n", Name, FileName, native.LineNumber));
        }
    }
}
