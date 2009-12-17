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
    internal struct SCRIPT_LOGATTR
    {
        public const byte fSoftBreakMask  = 0x01;
        public const byte fWhiteSpaceMask = 0x02;
        public const byte fCharStopMask   = 0x04;
        public const byte fWordStopMask   = 0x08;
        public const byte fInvalidMask    = 0x10;
        public const byte fReservedMask   = 0xe0;

        public byte BitFields;

        public bool fWhiteSpace
        {
            get { return (BitFields & fWhiteSpaceMask) != 0; }
        }

        public bool fSoftBreak
        {
            get { return (BitFields & fSoftBreakMask) != 0; }
        }

        public bool fSoftBreakOrfWhitespace
        {
            get { return (BitFields & (fSoftBreakMask | fWhiteSpaceMask)) != 0; }
        }

        public bool fCharStop
        {
            get { return (BitFields & fCharStopMask) != 0; }
        }

        public void SetfWhiteSpace()
        {
            BitFields |= fWhiteSpaceMask;
        }

        public void SetfSoftBreakfCharStopAndfWordStop()
        {
            BitFields |= fSoftBreakMask | fCharStopMask | fWordStopMask;
        }
    }
}
