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
using System.IO;
using System.Reflection;
using System.Text;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// Provides basic information about an Assembly derived from its metadata.
    /// </summary>
    public class AssemblyMetadata
    {
        private readonly ushort majorRuntimeVersion;
        private readonly ushort minorRuntimeVersion;
        private readonly CorFlags corflags;
        private readonly PEFormat peFormat;

        private AssemblyMetadata(ushort majorRuntimeVersion, ushort minorRuntimeVersion, CorFlags corflags, PEFormat peFormat)
        {
            this.majorRuntimeVersion = majorRuntimeVersion;
            this.minorRuntimeVersion = minorRuntimeVersion;
            this.corflags = corflags;
            this.peFormat = peFormat;
        }

        /// <summary>
        /// Gets the major version of the CLI runtime required for the assembly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Typical runtime versions:
        /// <list type="bullet">
        /// <item>2.0: .Net 1.0 / .Net 1.1</item>
        /// <item>2.5: .Net 2.0 / .Net 3.0 / .Net 3.5</item>
        /// <item>4.0: .Net 4.0</item>
        /// </list>
        /// </para>
        /// </remarks>
        public int MajorRuntimeVersion
        {
            get { return majorRuntimeVersion; }
        }

        /// <summary>
        /// Gets the minor version of the CLI runtime required for the assembly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Typical runtime versions:
        /// <list type="bullet">
        /// <item>2.0: .Net 1.0 / .Net 1.1</item>
        /// <item>2.5: .Net 2.0 / .Net 3.0 / .Net 3.5</item>
        /// <item>4.0: .Net 4.0</item>
        /// </list>
        /// </para>
        /// </remarks>
        public int MinorRuntimeVersion
        {
            get { return minorRuntimeVersion; }
        }

        /// <summary>
        /// Gets the processor architecture required for the assembly.
        /// </summary>
        public ProcessorArchitecture ProcessorArchitecture
        {
            get
            {
                if (peFormat == PEFormat.PE32Plus)
                    return ProcessorArchitecture.Amd64;
                if ((corflags & CorFlags.F32BitsRequired) != 0)
                    return ProcessorArchitecture.X86;
                return ProcessorArchitecture.MSIL;
            }
        }

        internal static AssemblyMetadata ReadAssemblyMetadata(Stream stream)
        {
            long length = stream.Length;
            if (length < 0x40)
                return null;

            BinaryReader reader = new BinaryReader(stream);

            // Read the pointer to the PE header.
            stream.Position = 0x3c;
            uint peHeaderPtr = reader.ReadUInt32();
            if (peHeaderPtr == 0)
                peHeaderPtr = 0x80;

            // Ensure there is at least enough room for the following structures:
            //     24 byte PE Signature & Header
            //     28 byte Standard Fields         (24 bytes for PE32+)
            //     68 byte NT Fields               (88 bytes for PE32+)
            // >= 128 byte Data Dictionary Table
            if (peHeaderPtr > length - 256)
                return null;

            // Check the PE signature.  Should equal 'PE\0\0'.
            stream.Position = peHeaderPtr;
            uint peSignature = reader.ReadUInt32();
            if (peSignature != 0x00004550)
                return null;

            // Read PE header fields.
            ushort machine = reader.ReadUInt16();
            ushort numberOfSections = reader.ReadUInt16();
            uint timeStamp = reader.ReadUInt32();
            uint symbolTablePtr = reader.ReadUInt32();
            uint numberOfSymbols = reader.ReadUInt32();
            ushort optionalHeaderSize = reader.ReadUInt16();
            ushort characteristics = reader.ReadUInt16();

            // Read PE magic number from Standard Fields to determine format.
            PEFormat peFormat = (PEFormat) reader.ReadUInt16();
            if (peFormat != PEFormat.PE32 && peFormat != PEFormat.PE32Plus)
                return null;

            // Read the 15th Data Dictionary RVA field which contains the CLI header RVA.
            // When this is non-zero then the file contains CLI data otherwise not.
            stream.Position = peHeaderPtr + (peFormat == PEFormat.PE32 ? 232 : 248);
            uint cliHeaderRva = reader.ReadUInt32();
            if (cliHeaderRva == 0)
                return null;

            // Read section headers.  Each one is 40 bytes.
            //    8 byte Name
            //    4 byte Virtual Size
            //    4 byte Virtual Address
            //    4 byte Data Size
            //    4 byte Data Pointer
            //  ... total of 40 bytes
            uint sectionTablePtr = peHeaderPtr + 24 + optionalHeaderSize;
            Section[] sections = new Section[numberOfSections];
            for (int i = 0; i < numberOfSections; i++)
            {
                stream.Position = sectionTablePtr + i * 40 + 8;

                Section section = new Section();
                section.VirtualSize = reader.ReadUInt32();
                section.VirtualAddress = reader.ReadUInt32();
                reader.ReadUInt32();
                section.Pointer = reader.ReadUInt32();

                sections[i] = section;
            }

            // Read parts of the CLI header.
            uint cliHeaderPtr = ResolveRva(sections, cliHeaderRva);
            if (cliHeaderPtr == 0)
                return null;

            stream.Position = cliHeaderPtr + 4;
            ushort majorRuntimeVersion = reader.ReadUInt16();
            ushort minorRuntimeVersion = reader.ReadUInt16();

            stream.Position = cliHeaderPtr + 16;
            CorFlags corflags = (CorFlags)reader.ReadUInt32();

            return new AssemblyMetadata(majorRuntimeVersion, minorRuntimeVersion, corflags, peFormat);
        }

        private static uint ResolveRva(Section[] sections, uint rva)
        {
            foreach (Section section in sections)
            {
                if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.VirtualSize)
                    return rva - section.VirtualAddress + section.Pointer;
            }

            return 0;
        }

        private enum PEFormat : ushort
        {
            PE32 = 0x10b,
            PE32Plus = 0x20b
        }

        [Flags]
        private enum CorFlags : uint
        {
            F32BitsRequired = 2,
            ILOnly = 1,
            StrongNameSigned = 8,
            TrackDebugData = 0x10000
        }

        private class Section
        {
            public uint VirtualAddress;
            public uint VirtualSize;
            public uint Pointer;
        }
    }
}
