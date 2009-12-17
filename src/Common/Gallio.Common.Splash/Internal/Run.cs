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

namespace Gallio.Common.Splash.Internal
{
    /// <summary>
    /// Internal structure describing a run of text or an embedded object in the document.
    /// </summary>
    /// <remarks>
    /// Packed into 4 bytes per Run.  Keep it that way.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct Run
    {
        private const int RunKindMask = 0x03;

        private byte styleIndex;
        private byte bitFields;
        private ushort miscShort;

        public RunKind RunKind
        {
            get { return (RunKind)(bitFields & RunKindMask); }
        }

        public int StyleIndex
        {
            get { return styleIndex; }
        }

        public int CharCount
        {
            get
            {
                switch (RunKind)
                {
                    case RunKind.Text:
                        return miscShort;
                    case RunKind.Object:
                    case RunKind.Tab:
                        return 1;
                    default:
                        throw new NotSupportedException();
                }
            }
            set
            {
                switch (RunKind)
                {
                    case RunKind.Text:
                        miscShort = (ushort)value;
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public int ObjectIndex
        {
            get
            {
                switch (RunKind)
                {
                    case RunKind.Object:
                        return miscShort;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public void InitializeTextRun(int styleIndex)
        {
            this.styleIndex = (byte)styleIndex;
            bitFields = (byte)RunKind.Text;
            miscShort = 0;
        }

        public void InitializeObjectRun(int styleIndex, int objectIndex)
        {
            this.styleIndex = (byte)styleIndex;
            bitFields = (byte)RunKind.Object;
            miscShort = (ushort)objectIndex;
        }

        public void InitializeTabRun(int styleIndex)
        {
            this.styleIndex = (byte)styleIndex;
            bitFields = (byte)RunKind.Tab;
            miscShort = 0;
        }
    }
}
