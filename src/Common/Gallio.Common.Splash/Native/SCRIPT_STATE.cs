// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPT_STATE
    {
        public const ushort uBidiLevelMask         = 0x001f;
        public const ushort fOverrideDirectionMask = 0x0020;
        public const ushort fInhibitSymSwapMask    = 0x0040;
        public const ushort fCharShapeMask         = 0x0080;
        public const ushort fDigitSubstituteMask   = 0x0100;
        public const ushort fInhibitLigateMask     = 0x0200;
        public const ushort fDisplayZWGMask        = 0x0400;
        public const ushort fArabicNumContextMask  = 0x0800;
        public const ushort fGcpClustersMask       = 0x1000;
        public const ushort fReservedMask          = 0x2000;
        public const ushort fEngineReservedMask    = 0xc000;

        public ushort BitFields;

        public int uBidiLevel
        {
            get { return BitFields & uBidiLevelMask; }
            set { BitFields = (ushort) (BitFields & ~uBidiLevelMask | value); }
        }
    }
}
