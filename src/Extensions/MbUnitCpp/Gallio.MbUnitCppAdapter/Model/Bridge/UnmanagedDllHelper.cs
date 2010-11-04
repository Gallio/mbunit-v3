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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Model;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    public static class UnmanagedDllHelper
    {
        public static ProcessorArchitecture GetArchitecture(string file)
        {
            using (var fileStream = new FileStream(file, FileMode.Open))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                fileStream.Seek(0x3c, SeekOrigin.Begin);
                Int32 peOffset = binaryReader.ReadInt32();
                fileStream.Seek(peOffset, SeekOrigin.Begin);
                UInt32 peHead = binaryReader.ReadUInt32();

                if (peHead != 0x00004550)
                    return ProcessorArchitecture.None;

                UInt16 machineType = binaryReader.ReadUInt16();

                switch (machineType)
                {
                    case 0x8664: // AMD64
                        return ProcessorArchitecture.Amd64;

                    case 0x200: // IA64
                        return ProcessorArchitecture.IA64;

                    case 0x14c: // X86
                        return ProcessorArchitecture.X86;

                    default:
                        return ProcessorArchitecture.None;
                }
            }
        }
    }
}
