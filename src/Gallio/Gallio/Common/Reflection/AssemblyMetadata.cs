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
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Metadata;
using Mono.Cecil.PE;

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
        private readonly AssemblyName assemblyName;
        private readonly IList<AssemblyName> assemblyReferences;
        private readonly string runtimeVersion;

        private AssemblyMetadata(ushort majorRuntimeVersion, ushort minorRuntimeVersion, CorFlags corflags, PEFormat peFormat,
            AssemblyName assemblyName, IList<AssemblyName> assemblyReferences, string runtimeVersion)
        {
            this.majorRuntimeVersion = majorRuntimeVersion;
            this.minorRuntimeVersion = minorRuntimeVersion;
            this.corflags = corflags;
            this.peFormat = peFormat;
            this.assemblyName = assemblyName;
            this.assemblyReferences = assemblyReferences;
            this.runtimeVersion = runtimeVersion;
        }

        /// <summary>
        /// Gets the major version of the CLI runtime required for the assembly.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Typical runtime versions:
        /// <list type="bullet">
        /// <item>2.0: .Net 1.0 / .Net 1.1</item>
        /// <item>2.5: .Net 2.0 / .Net 3.0 / .Net 3.5 / .Net 4.0</item>
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
        /// <item>2.5: .Net 2.0 / .Net 3.0 / .Net 3.5 / .Net 4.0</item>
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

                // Issue 704 - If ILOnly is false, we must run x86
                if ((corflags & CorFlags.F32BitsRequired) != 0 || (corflags & CorFlags.ILOnly) == 0)
                    return ProcessorArchitecture.X86;

                return ProcessorArchitecture.MSIL;
            }
        }

        /// <summary>
        /// Gets the assembly references.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field is only populated when the <see cref="AssemblyMetadataFields.AssemblyName"/> flag is specified.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the assembly name was not populated.</exception>
        public AssemblyName AssemblyName
        {
            get
            {
                if (assemblyName == null)
                    throw new InvalidOperationException("The assembly name was not populated.");
                return assemblyName;
            }
        }

        /// <summary>
        /// Gets the assembly references.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field is only populated when the <see cref="AssemblyMetadataFields.AssemblyReferences"/> flag is specified.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the assembly references were not populated.</exception>
        public IList<AssemblyName> AssemblyReferences
        {
            get
            {
                if (assemblyReferences == null)
                    throw new InvalidOperationException("The assembly references were not populated.");
                return assemblyReferences;
            }
        }

        /// <summary>
        /// Gets the runtime version in the form "vX.Y.ZZZZ".
        /// </summary>
        /// <remarks>
        /// <para>
        /// This field is only populated when the <see cref="AssemblyMetadataFields.RuntimeVersion"/> flag is specified.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the runtime version were not populated.</exception>
        public string RuntimeVersion
        {
            get
            {
                if (runtimeVersion == null)
                    throw new InvalidOperationException("The runtime version was not populated.");
                return runtimeVersion;
            }
        }

        internal static AssemblyMetadata ReadAssemblyMetadata(Stream stream, AssemblyMetadataFields fields)
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
            PEFormat peFormat = (PEFormat)reader.ReadUInt16();
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
            uint metadataRva = reader.ReadUInt32();
            uint metadataSize = reader.ReadUInt32();
            CorFlags corflags = (CorFlags)reader.ReadUInt32();

            // Read optional fields.
            AssemblyName assemblyName = null;
            IList<AssemblyName> assemblyReferences = null;
            string runtimeVersion = null;

            if ((fields & AssemblyMetadataFields.RuntimeVersion) != 0)
            {
                uint metadataPtr = ResolveRva(sections, metadataRva);
                stream.Position = metadataPtr + 12;

                int paddedRuntimeVersionLength = reader.ReadInt32();
                byte[] runtimeVersionBytes = reader.ReadBytes(paddedRuntimeVersionLength);

                int runtimeVersionLength = 0;
                while (runtimeVersionLength < paddedRuntimeVersionLength
                    && runtimeVersionBytes[runtimeVersionLength] != 0)
                    runtimeVersionLength += 1;

                runtimeVersion = Encoding.UTF8.GetString(runtimeVersionBytes, 0, runtimeVersionLength);
            }

            if ((fields & (AssemblyMetadataFields.AssemblyName | AssemblyMetadataFields.AssemblyReferences)) != 0)
            {
                // Using Cecil.
                stream.Position = 0;
                var imageReader = new ImageReader(stream);

                if ((fields & AssemblyMetadataFields.AssemblyName) != 0)
                    assemblyName = imageReader.GetAssemblyName();

                if ((fields & AssemblyMetadataFields.AssemblyReferences) != 0)
                    assemblyReferences = imageReader.GetAssemblyReferences();
            }

            // Done.
            return new AssemblyMetadata(majorRuntimeVersion, minorRuntimeVersion, corflags, peFormat,
                assemblyName, assemblyReferences, runtimeVersion);
        }

        private static uint ResolveRva(IEnumerable<Section> sections, uint rva)
        {
            foreach (Section section in sections)
            {
                if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.VirtualSize)
                    return rva - section.VirtualAddress + section.Pointer;
            }

            return 0;
        }

        private sealed class ImageReader
        {
            private readonly AssemblyDefinition assemblyDefinition;

            public ImageReader(Stream stream)
            {
                assemblyDefinition = AssemblyDefinition.ReadAssembly(stream);
            }

            public AssemblyName GetAssemblyName()
            {
                var assemblyName = new AssemblyName
                {
                    Name = assemblyDefinition.Name.Name,
                    Version = assemblyDefinition.Name.Version,
                    CultureInfo = new CultureInfo(assemblyDefinition.Name.Culture)
                };

                if (assemblyDefinition.Name.PublicKey.Length > 0)
                    assemblyName.SetPublicKey(assemblyDefinition.Name.PublicKey);

                return assemblyName;
            }

            public IList<AssemblyName> GetAssemblyReferences()
            {
                var assemblyReferences = new List<AssemblyName>();

                foreach (var reference in assemblyDefinition.MainModule.AssemblyReferences)
                {
                    var assemblyName = new AssemblyName
                    {
                        Name = reference.Name,
                        Version = reference.Version,
                        CultureInfo = new CultureInfo(reference.Culture)
                    };

                    if (reference.HasPublicKey)
                        assemblyName.SetPublicKey(reference.PublicKey);

                    if (reference.PublicKeyToken != null)
                        assemblyName.SetPublicKeyToken(reference.PublicKeyToken);

                    assemblyReferences.Add(assemblyName);
                }

                return assemblyReferences;
            }
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
