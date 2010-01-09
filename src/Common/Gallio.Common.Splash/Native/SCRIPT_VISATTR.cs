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
    internal struct SCRIPT_VISATTR
    {
        public const ushort uJustificationMask = 0x000f;
        public const ushort fClusterStartMask  = 0x0010;
        public const ushort fDiacriticMask     = 0x0020;
        public const ushort fZeroWidthMask     = 0x0040;
        public const ushort fReservedMask      = 0x0080;
        public const ushort fShapeReservedMask = 0xff00;

        public ushort BitFields;
    }
}
