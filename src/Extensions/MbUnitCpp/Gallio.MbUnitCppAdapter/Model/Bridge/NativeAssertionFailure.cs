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
    public struct NativeAssertionFailure
    {
        public IntPtr DescriptionPtr;
        public IntPtr MessagePtr;
        public IntPtr ActualValuePtr;
        public IntPtr ExpectedValuePtr;

        public string Description
        {
            get { return Marshal.PtrToStringAnsi(DescriptionPtr); }
        }

        public string Message
        {
            get { return HasMessage ? Marshal.PtrToStringAnsi(MessagePtr) : String.Empty; }
        }

        public string ActualValue
        {
            get { return HasActualValue ? Marshal.PtrToStringAnsi(ActualValuePtr) : String.Empty; }
        }

        public string ExpectedValue
        {
            get { return HasExpectedValue ? Marshal.PtrToStringAnsi(ExpectedValuePtr) : String.Empty; }
        }

        public bool HasMessage
        {
            get { return MessagePtr != IntPtr.Zero; }
        }

        public bool HasExpectedValue
        {
            get { return ExpectedValuePtr != IntPtr.Zero; }
        }

        public bool HasActualValue
        {
            get { return ActualValuePtr != IntPtr.Zero; }
        }
    }
}
