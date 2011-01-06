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
using Gallio.Common.Collections;

namespace Gallio.MbUnitCppAdapter.Model.PE
{
    /// <summary>
    /// A reader that gets useful information from the PE image of a target DLL.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The reader does not lock the DLL, nor it causes its dependencies to be loaded.
    /// </para>
    /// <para>
    /// Information found in the PE image might be useful for determining whether
    /// a DLL is suitable to a given unmanaged test framework adapter. We might look
    /// if the DLL references a particular DLL (e.g. "boost_unit_test_framework.dll") or
    /// if it exports a specific function (e.g. "MbUnitCpp_RunTests").
    /// </para>
    /// </remarks>
    public class PEImageReader : IDisposable
    {
        private readonly PEImageDataReader reader;
        private bool disposed;
        private long offsetFileHeader;
        private long offsetOptionalHeader;
        private long offsetImports;
        private long offsetExports;
        private ushort sizeOfOptionalHeader;
        private int numberOfSections;

        private class Magic : IEquatable<Magic>
        {
            private readonly ushort code;
            private readonly long offsetNumberOfRvaAndSizes;
            private readonly long offsetExports;
            private readonly long offsetImports;

            public bool IsValid
            {
                get { return code != 0; }
            }

            public long OffsetNumberOfRvaAndSizes
            {
                get { return offsetNumberOfRvaAndSizes; }
            }

            public long OffsetExports
            {
                get { return offsetExports; }
            }

            public long OffsetImports
            {
                get { return offsetImports; }
            }

            private Magic(ushort code, long offsetNumberOfRvaAndSizes, long offsetExports, long offsetImports)
            {
                this.code = code;
                this.offsetNumberOfRvaAndSizes = offsetNumberOfRvaAndSizes;
                this.offsetExports = offsetExports;
                this.offsetImports = offsetImports;
            }

            public readonly static Magic Unknown = new Magic(0, 0, 0, 0);
            public readonly static Magic PE32 = new Magic(0x10b, 92, 96, 104);
            public readonly static Magic PE32Plus = new Magic(0x20b, 108, 112, 120);

            public bool Equals(Magic other)
            {
                return (other != null) && (code == other.code);
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as Magic);
            }

            public override int GetHashCode()
            {
                return code;
            }

            public static Magic FromCode(ushort code)
            {
                if (code == PE32.code)
                    return PE32;

                if (code == PE32Plus.code)
                    return PE32Plus;

                return Unknown;
            }
        }

        /// <summary>
        /// Constructs a reader from the specified file.
        /// </summary>
        /// <param name="file">The file to read data from.</param>
        public PEImageReader(string file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            reader = new PEImageDataReader(file);
        }

        /// <summary>
        /// Constructs a reader from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to read data from.</param>
        /// <remarks>
        /// <para>
        /// The stream is not disposed when the reader is disposed.
        /// </para>
        /// </remarks>
        public PEImageReader(Stream stream)
        {
            reader = new PEImageDataReader(stream);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                reader.Dispose();
            }

            disposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Reads the PE image.
        /// </summary>
        /// <returns>Information found in the PE image, or a null reference if not found or not applicable.</returns>
        public PEImageInfo Read()
        {
            if (!VerifySignature())
                return null;

            ProcessorArchitecture architecture = ReadArchitecture();

            if (!FindSections())
                return new PEImageInfo(architecture, null, null);

            return new PEImageInfo(architecture, ReadImports(), ReadExports());
        }

        private bool VerifySignature()
        {
            int address = reader.ReadInt32(0x3c);
            uint signature = reader.ReadUInt32(address);
            offsetFileHeader = address + 4;
            return (signature == 0x00004550); // = "PE\0\0"
        }

        private static readonly IDictionary<ushort, ProcessorArchitecture> ProcessorArchitectureMap = new Dictionary<ushort, ProcessorArchitecture>
        {
            { 0x8664, ProcessorArchitecture.Amd64 },
            { 0x0200, ProcessorArchitecture.IA64 },
            { 0x014c, ProcessorArchitecture.X86 },
        };

        private ProcessorArchitecture ReadArchitecture()
        {
            ushort machine = reader.ReadUInt16(offsetFileHeader + 0);
            ProcessorArchitecture result;
            bool found = ProcessorArchitectureMap.TryGetValue(machine, out result);
            return found ? result : ProcessorArchitecture.None;
        }

        private bool FindSections()
        {
            sizeOfOptionalHeader = reader.ReadUInt16(offsetFileHeader + 16);
            numberOfSections = reader.ReadUInt16(offsetFileHeader + 2);

            if (sizeOfOptionalHeader == 0) // No optional header.
                return false;

            offsetOptionalHeader = offsetFileHeader + 20;
            Magic magic = Magic.FromCode(reader.ReadUInt16(offsetOptionalHeader));

            if (!magic.IsValid)
                return false;

            uint numberOfNvaAndSizes = reader.ReadUInt32(offsetOptionalHeader + magic.OffsetNumberOfRvaAndSizes);

            if (numberOfNvaAndSizes < 2) // Too small optional header size: import table is not present.
                return false;

            offsetExports = RvaToOffset(reader.ReadUInt32(offsetOptionalHeader + magic.OffsetExports));
            offsetImports = RvaToOffset(reader.ReadUInt32(offsetOptionalHeader + magic.OffsetImports));
            return true;
        }

        private long RvaToOffset(uint rva)
        {
            long current = offsetOptionalHeader + sizeOfOptionalHeader;

            for (int i = 0; i < numberOfSections; i++, current += 40)
            {
                uint virtualSize = reader.ReadUInt32(current + 8);
                uint virtualAddress = reader.ReadUInt32(current + 12);
                uint pointerToRawData = reader.ReadUInt32(current + 20);

                if (virtualAddress <= rva && virtualAddress + virtualSize > rva)
                    return rva - virtualAddress + pointerToRawData;
            }

            throw new InvalidOperationException("Section not found!");
        }

        private IEnumerable<string> ReadImports()
        {
            long offset = offsetImports;

            while (true)
            {
                uint rva = reader.ReadUInt32(offset + 12);

                if (rva == 0)
                    yield break;

                yield return reader.ReadAsciiString(RvaToOffset(rva));
                offset += 20;
            }
        }

        private IEnumerable<string> ReadExports()
        {
            uint count = reader.ReadUInt32(offsetExports + 24);
            long offset = RvaToOffset(reader.ReadUInt32(offsetExports + 32));

            for (uint i = 0; i < count; i++, offset += 4)
            {
                long pointer = RvaToOffset(reader.ReadUInt32(offset));
                yield return reader.ReadAsciiString(pointer);
            }
        }
    }
}