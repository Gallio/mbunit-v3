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
    /// The native structure that holds information about a MbUnitCpp test or a MbUnitCpp fixture.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeTestInfoData
    {
        /// <summary>
        /// A pointer to the unmanaged unicode string that holds the fixture/test name.
        /// </summary>
        public IntPtr NamePtr;

        /// <summary>
        /// The index of the test or the fixture.
        /// </summary>
        public int Index;

        /// <summary>
        /// Determines whether the current info describes a test fixture or a test case.
        /// </summary>
        public bool IsTestFixture;

        /// <summary>
        /// A pointer to the unmanaged unicode string that holds the source file name where the fixture/test is defined.
        /// </summary>
        public IntPtr FileNamePtr;

        /// <summary>
        /// The line number in the source file where the fixture/test is declared.
        /// </summary>
        public int LineNumber;

        /// <summary>
        /// Represents the position of the current fixture/test.
        /// </summary>
        public Position Position;

        /// <summary>
        /// 
        /// </summary>
        public int MetadataId;
    }
}
