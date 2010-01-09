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
using System.Text;

namespace Gallio.Common.Splash.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SCRIPT_ANALYSIS
    {
        public const ushort eScriptMask       = 0x03ff;
        public const ushort fRTLMask          = 0x0400;
        public const ushort fLayoutRTLMask    = 0x0800;
        public const ushort fLinkBeforeMask   = 0x1000;
        public const ushort fLinkAfterMask    = 0x2000;
        public const ushort fLogicalOrderMask = 0x4000;
        public const ushort fNoGlyphIndexMask = 0x8000;

        public ushort BitFields;
        public SCRIPT_STATE s;

        public int eScript
        {
            get { return BitFields & eScriptMask; }
            set { BitFields = (ushort)(BitFields & ~eScriptMask | value); }
        }

        public bool fRTL
        {
            get { return (BitFields & fRTLMask) != 0; }
        }
    }
}
