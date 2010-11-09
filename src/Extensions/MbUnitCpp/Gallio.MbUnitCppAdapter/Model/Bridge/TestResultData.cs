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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Gallio.Model;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// The standard test outcome of MbUnitCpp tests.
    /// </summary>
    public enum NativeOutcome
    {
        INCONCLUSIVE = 0,
        PASSED = 1,
        FAILED = 2,
    }

    /// <summary>
    /// A native structure that holds the results of the test step.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TestStepResult
    {
        public NativeOutcome NativeOutcome;
        public IntPtr MessagePtr;
        public int AssertCount;

        private static readonly IDictionary<NativeOutcome, TestOutcome> map = new Dictionary<NativeOutcome, TestOutcome>
        {
            {NativeOutcome.INCONCLUSIVE, TestOutcome.Inconclusive},
            {NativeOutcome.PASSED, TestOutcome.Passed},
            {NativeOutcome.FAILED, TestOutcome.Failed},
        };

        public TestOutcome TestOutcome
        {
            get
            {
                return map[NativeOutcome];
            }
        }

        public string Message
        {
            get
            {
                return Marshal.PtrToStringAnsi(MessagePtr);
            }
        }

    }
}