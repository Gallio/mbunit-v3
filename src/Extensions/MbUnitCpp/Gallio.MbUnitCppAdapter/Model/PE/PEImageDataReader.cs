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
    public class PEImageDataReader : IDisposable
    {
        private readonly Stream stream;
        private readonly BinaryReader reader;
        private readonly bool ownStream;
        private bool disposed;

        public PEImageDataReader(string file)
        {
            ownStream = true;
            stream = new FileStream(file, FileMode.Open, FileAccess.Read);
            reader = new BinaryReader(stream);
        }

        public PEImageDataReader(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            ownStream = false;
            this.stream = stream;
            reader = new BinaryReader(stream);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                reader.Close();

                if (ownStream)
                    stream.Dispose();
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Seek(long offset)
        {
            stream.Seek(offset, SeekOrigin.Begin);
        }

        public ushort ReadUInt16(long offset)
        {
            Seek(offset);
            return reader.ReadUInt16();
        }

        public int ReadInt32(long offset)
        {
            Seek(offset);
            return reader.ReadInt32();
        }

        public uint ReadUInt32(long offset)
        {
            Seek(offset);
            return reader.ReadUInt32();
        }

        public string ReadAsciiString(long offset)
        {
            Seek(offset);
            byte[] bytes = GenericCollectionUtils.ToArray(ReadBytes());
            return Encoding.ASCII.GetString(bytes);
        }

        private IEnumerable<byte> ReadBytes()
        {
            byte @byte;

            while ((@byte = reader.ReadByte()) != 0)
            {
                yield return @byte;
            }
        }
    }
}