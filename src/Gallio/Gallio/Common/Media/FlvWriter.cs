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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Useful docs:
//   http://www.adobe.com/devnet/flv/
//   http://www.adobe.com/devnet/swf/
//   http://osflash.org/flv

namespace Gallio.Common.Media
{
    /// <summary>
    /// Writes FLV streams.
    /// </summary>
    internal sealed class FlvWriter
    {
        private byte[] buffer;
        private int bufferOffset;

        public FlvWriter(FlvFlags flags)
        {
            WriteFlvHeader(flags);
        }

        [Flags]
        public enum FlvFlags : byte
        {
            Video = 0x01,
            Audio = 0x04
        }

        [Flags]
        public enum FlvTagType : byte
        {
            Audio = 0x08,
            Video = 0x09,
            Meta = 0x12
        }

        [Flags]
        public enum FlvAudioFrameFlags : byte
        {
            Type_Mono = 0x00,   // Mono
            Type_Stereo = 0x01, // Stereo
            Type_Mask = 0x01,

            Bits_8 = 0x00,      // 8 bit
            Bits_16 = 0x02,     // 16 bit
            Bits_Mask = 0x02,

            Rate_5500Hz = 0x00,     // 5.5kHz
            Rate_11000Hz = 0x04,    // 11.0kHz
            Rate_22000Hz = 0x08,    // 22.0kHz
            Rate_44000Hz = 0x0c,    // 44.0kHz
            Rate_Mask = 0x0c,

            Format_PCM_PE = 0x00,       // PCM, platform-endian
            Format_ADPCM = 0x10,        // ADPCM
            Format_MP3   = 0x20,        // MP3
            Format_PCM_LE = 0x30,       // PCM, little-endian
            Format_Nellymoser_16kHz = 0x40, // Nellymoser 16kHz mono
            Format_Nellymoser_8kHz = 0x50,  // Nellymoser 8KkHz mono
            Format_Nellymoser = 0x60,   // Nellymoser
            Format_Alaw = 0x70,         // G.711 A-law logarithmic PCM
            Format_Mulaw = 0x80,        // G.711 mu-law logarithmic PCM
            Format_AAC = 0xa0,          // AAC
            Format_Speex = 0xb0,        // Speex
            Format_MP3_8kHz = 0xe0,        // MP3 8kHz
            Format_Mask = 0xf0
        }

        [Flags]
        public enum FlvVideoFrameFlags : byte
        {
            Codec_JPEG = 0x01,          // JPEG
            Codec_Sorensen = 0x02,      // Sorensen H.263
            Codec_ScreenVideo = 0x03,   // ScreenVideo
            Codec_VP6 = 0x04,           // On2 VP6
            Codec_VP6Alpha = 0x05,      // On2 VP6 with alpha channel
            Codec_ScreenVideo2 = 0x06,  // ScreenVideo 2
            Codec_AVC = 0x07,           // AVC
            Codec_Mask = 0x0f,

            Type_KeyFrame = 0x10,       // Key Frame
            Type_InterFrame = 0x20,     // Inter Frame
            Type_DisposableInterFrame = 0x30, // Disposable Inter Frame
            Type_GeneratedKeyFrame = 0x40, // Generated Key Frame (server-only)
            Type_CommandFrame = 0x50,   // Video Info / Command Frame
            Type_Mask = 0xf0
        }

        private enum AmfDataType : byte
        {
            Number = 0,
            Boolean = 1,
            String = 2,
            Object = 3,
            MovieClip = 4,
            Null = 5,
            Undefined = 6,
            Reference = 7,
            Array = 8,
            End = 9,
            StrictArray = 10,
            Date = 11,
            LongString = 12
        }

        public sealed class FlvMetadata : IEnumerable<KeyValuePair<string, object>>
        {
            private readonly Dictionary<string, object> contents = new Dictionary<string, object>();

            public void Add(string key, double value)
            {
                contents.Add(key, value);
            }

            public void Add(string key, bool value)
            {
                contents.Add(key, value);
            }

            public void Add(string key, string value)
            {
                contents.Add(key, value);
            }

            public void Update(string key, double value)
            {
            }

            internal FlvMetadataUpdater WriteTo(FlvWriter writer)
            {
                var offsets = new Dictionary<string, int>();

                writer.WriteAmfData("onMetaData");
                writer.WriteAmfArrayBeginMarker(contents.Count);
                foreach (var pair in contents)
                {
                    writer.WriteAmfStringContent(pair.Key);
                    offsets.Add(pair.Key, writer.bufferOffset);
                    writer.WriteAmfData(pair.Value);
                }
                writer.WriteAmfArrayEndMarker();

                return new FlvMetadataUpdater(writer, offsets);
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return contents.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public sealed class FlvMetadataUpdater
        {
            private readonly FlvWriter writer;
            private readonly Dictionary<string, int> offsets;

            internal FlvMetadataUpdater(FlvWriter writer, Dictionary<string, int> offsets)
            {
                this.writer = writer;
                this.offsets = offsets;
            }

            public void Update(string key, double value)
            {
                int oldOffset = writer.bufferOffset;
                writer.bufferOffset = offsets[key];
                writer.WriteAmfData(value);
                writer.bufferOffset = oldOffset;
            }
        }

        public delegate void FlvTagWriter(byte[] buffer, ref int bufferOffset);

        public void WriteFlvAudioFrame(FlvAudioFrameFlags flags, int timestampMillis, int reserveBytes, FlvTagWriter tagWriter)
        {
            WriteFlvTag(FlvTagType.Audio, timestampMillis, reserveBytes + 1, delegate
            {
                buffer[bufferOffset++] = (byte)flags;
                tagWriter(buffer, ref bufferOffset);
            });
        }

        public void WriteFlvVideoFrame(FlvVideoFrameFlags flags, int timestampMillis, int reserveBytes, FlvTagWriter tagWriter)
        {
            WriteFlvTag(FlvTagType.Video, timestampMillis, reserveBytes + 1, delegate
            {
                buffer[bufferOffset++] = (byte)flags;
                tagWriter(buffer, ref bufferOffset);
            });
        }

        public FlvMetadataUpdater WriteFlvMetaFrame(FlvMetadata metadata, int timestampMillis)
        {
            FlvMetadataUpdater updater = null;
            WriteFlvTag(FlvTagType.Meta, timestampMillis, 0, delegate
            {
                updater = metadata.WriteTo(this);
            });
            return updater;
        }

        private void WriteFlvHeader(FlvFlags flags)
        {
            ReserveCapacity(9);
            buffer[bufferOffset++] = (byte) 'F';
            buffer[bufferOffset++] = (byte) 'L';
            buffer[bufferOffset++] = (byte) 'V';
            buffer[bufferOffset++] = 0x01;
            buffer[bufferOffset++] = (byte) flags;
            buffer[bufferOffset++] = 0x00;
            buffer[bufferOffset++] = 0x00;
            buffer[bufferOffset++] = 0x00;
            buffer[bufferOffset++] = 0x09;

            WriteFlvTagTotalLenth(0);
        }

        private void WriteFlvTag(FlvTagType type, int timestampMillis, int reserveBytes, FlvTagWriter tagWriter)
        {
            int tagStartOffset = bufferOffset;
            int bodyLengthOffset = WriteFlvTagHeader(type, timestampMillis);

            int bodyStartOffset = bufferOffset;
            ReserveCapacity(reserveBytes);
            tagWriter(buffer, ref bufferOffset);

            WriteFlvTagBodyLengthAtOffset(bodyLengthOffset, bufferOffset - bodyStartOffset);
            WriteFlvTagTotalLenth(bufferOffset - tagStartOffset);
        }

        private int WriteFlvTagHeader(FlvTagType type, int timestampMillis)
        {
            ReserveCapacity(11);

            buffer[bufferOffset++] = (byte)type;
            int bodyLengthOffset = bufferOffset;
            bufferOffset += 3;
            buffer[bufferOffset++] = (byte)(timestampMillis >> 16);
            buffer[bufferOffset++] = (byte)(timestampMillis >> 8);
            buffer[bufferOffset++] = (byte) timestampMillis;
            buffer[bufferOffset++] = (byte)(timestampMillis >> 24); // Most significant bits after other 24 bits.
            buffer[bufferOffset++] = 0x00;
            buffer[bufferOffset++] = 0x00;
            buffer[bufferOffset++] = 0x00;
            return bodyLengthOffset;
        }

        private void WriteFlvTagBodyLengthAtOffset(int bodyLengthOffset, int length)
        {
            buffer[bodyLengthOffset++] = (byte) (length >> 16);
            buffer[bodyLengthOffset++] = (byte) (length >> 8);
            buffer[bodyLengthOffset] = (byte) length;
        }

        private void WriteFlvTagTotalLenth(int length)
        {
            ReserveCapacity(4);

            buffer[bufferOffset++] = (byte)(length >> 24);
            buffer[bufferOffset++] = (byte)(length >> 16);
            buffer[bufferOffset++] = (byte)(length >> 8);
            buffer[bufferOffset++] = (byte)length;
        }

        private void WriteAmfData(object value)
        {
            if (value == null)
            {
                ReserveCapacity(1);
                buffer[bufferOffset++] = (byte) AmfDataType.Null;
            }
            else if (value is bool)
            {
                ReserveCapacity(2);
                buffer[bufferOffset++] = (byte)AmfDataType.Boolean;
                buffer[bufferOffset++] = (byte) ((bool)value ? 1 : 0);
            }
            else if (value is double)
            {
                ReserveCapacity(9);
                buffer[bufferOffset++] = (byte)AmfDataType.Number;

                byte[] valueBytes = BitConverter.GetBytes((double)value);
                for (int i = 7; i >= 0; i--)
                    buffer[bufferOffset++] = valueBytes[i];
            }
            else if (value is string)
            {
                ReserveCapacity(1);
                buffer[bufferOffset++] = (byte)AmfDataType.String;
                WriteAmfStringContent((string) value);
            }
        }

        private void WriteAmfArrayBeginMarker(int size)
        {
            ReserveCapacity(5);
            buffer[bufferOffset++] = (byte)AmfDataType.Array;
            buffer[bufferOffset++] = (byte) (size >> 24);
            buffer[bufferOffset++] = (byte) (size >> 16);
            buffer[bufferOffset++] = (byte) (size >> 8);
            buffer[bufferOffset++] = (byte) size;
        }

        private void WriteAmfArrayEndMarker()
        {
            ReserveCapacity(3);
            buffer[bufferOffset++] = 0; // no more properties
            buffer[bufferOffset++] = 0;
            buffer[bufferOffset++] = (byte)AmfDataType.End;
        }

        private void WriteAmfStringContent(string value)
        {
            ReserveCapacity(2 + Encoding.UTF8.GetByteCount(value));
            buffer[bufferOffset++] = (byte)(value.Length >> 8);
            buffer[bufferOffset++] = (byte)value.Length;
            bufferOffset += Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, bufferOffset);
        }

        private void ReserveCapacity(int reserveBytes)
        {
            if (buffer == null)
            {
                buffer = new byte[Math.Max(reserveBytes, 256)];
            }
            else if (bufferOffset + reserveBytes > buffer.Length)
            {
                Array.Resize(ref buffer, Math.Max(bufferOffset + reserveBytes, buffer.Length * 2));
            }
        }

        public void WriteTo(Stream stream)
        {
            stream.Write(buffer, 0, bufferOffset);
        }
    }
}
