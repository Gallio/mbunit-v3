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
    public enum NativeValueType
    {
        Raw,
        String,
        Boolean,
        Char,
        Byte,
        Int16,
        UInt16,
        Int32,
        UInt32,
        Int64,
        Single,
        Double,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeAssertionFailure
    {
        public int DescriptionId;
        public int MessageId;
        public int ActualValueId;
        public NativeValueType ActualValueType;
        public int ExpectedValueId;
        public NativeValueType ExpectedValueType;
    }
}
