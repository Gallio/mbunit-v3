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
    internal struct ABC
    {
        /// <summary>
        /// The A spacing of the character. The A spacing is the distance to add to the current position before drawing the character glyph.
        /// </summary>
        public int abcA;

        /// <summary>
        /// The B spacing of the character. The B spacing is the width of the drawn portion of the character glyph.
        /// </summary>
        public int abcB;

        /// <summary>
        /// The C spacing of the character. The C spacing is the distance to add to the current position to provide white space to the right of the character glyph.
        /// </summary>
        public int abcC;

        public int TotalWidth
        {
            get { return abcA + abcB + abcC; }
        }
    }
}
