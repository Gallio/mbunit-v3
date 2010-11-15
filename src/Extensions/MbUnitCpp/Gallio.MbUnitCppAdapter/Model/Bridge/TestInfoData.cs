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
    [StructLayout(LayoutKind.Sequential)]
    public struct TestInfoData
    {
        public IntPtr NamePtr;
        public int Index;
        public bool IsTestFixture;
        public IntPtr FileNamePtr;
        public int LineNumber;
        public Position Position;

        public string GetId()
        {
            return "MbUnitCpp_" + ((Position.pTest != IntPtr.Zero) ? Position.pTest : Position.pTestFixture);
        }

        public string Name
        {
            get
            {
                return Marshal.PtrToStringUni(NamePtr);
            }
        }

        public string FileName
        {
            get
            {
                return Marshal.PtrToStringUni(FileNamePtr);
            }
        }

        public StackTraceData GetStackTraceData()
        {
            return new StackTraceData(String.Format("   at {0} in {1}:line {2}\r\n", Name, FileName, LineNumber));
        }
    }
}
