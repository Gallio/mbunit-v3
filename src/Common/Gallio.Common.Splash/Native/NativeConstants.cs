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
using System.Text;

namespace Gallio.Common.Splash.Native
{
    internal static class NativeConstants
    {
        public const int S_OK = 0;
        public const int S_FALSE = 1;

        public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
        public const int USP_E_SCRIPT_NOT_IN_FONT = unchecked((int)0x80040200);

        public const int SCRIPT_UNDEFINED = 0;

        public const int TRANSPARENT = 1;
        public const int OPAQUE = 2;

        public const int DC_BRUSH = 18;
        public const int DC_PEN = 19;

        public const int RGN_XOR = 3;
    }
}
