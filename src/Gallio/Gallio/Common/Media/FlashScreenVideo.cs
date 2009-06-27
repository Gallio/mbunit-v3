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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Gallio.Common.Markup;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace Gallio.Common.Media
{
    /// <summary>
    /// Encodes video frames as FLV files using the Flash ScreenVideo lossless video codec.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This video format is useful for embedding video screen captures within web based test reports.
    /// Frames are encoded as blocks of 16x16 pixel tiles compressed using ZLib.  Unchanged tiles are retained
    /// from the previous frame.
    /// </para>
    /// <para>
    /// The maximum frame size for this video format is 4095x4095.
    /// </para>
    /// </remarks>
    public class FlashScreenVideo : Video
    {
        private const int ZLibWorstCaseInflation = 20; // max number of bytes Zlib will inflate an input in the worst case, 5 bytes per 16Kb per http://www.zlib.net/zlib_tech.html, but our block sizes will always be smaller than 64Kb

        private readonly int width;
        private readonly int height;
        private readonly double framesPerSecond;
        private readonly int nominalBlockWidth;
        private readonly int nominalBlockHeight;
        private readonly int keyFramePeriod;

        private readonly int cols;
        private readonly int rows;

        private readonly FlvWriter flvWriter;
        private readonly FlvWriter.FlvMetadataUpdater flvMetadataUpdater;
        private readonly Deflater deflater;
        private readonly int reserveBytesPerFrame; // number of bytes reserved in buffer for each frame as a maximum

        private readonly byte[] blockBuffer;
        private int[] previousFramePixels;
        private int[] currentFramePixels;
        private int frameCount;

        /// <summary>
        /// Creates a new Flash screen video.
        /// </summary>
        /// <param name="parameters">The video parameters.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameters"/> is null.</exception>
        public FlashScreenVideo(FlashScreenVideoParameters parameters)
            : base(parameters)
        {
            width = parameters.Width;
            height = parameters.Height;
            framesPerSecond = parameters.FramesPerSecond;
            nominalBlockWidth = parameters.BlockWidth;
            nominalBlockHeight = parameters.BlockHeight;
            keyFramePeriod = parameters.KeyFramePeriod;

            cols = (width + nominalBlockWidth - 1) / nominalBlockWidth;
            rows = (height + nominalBlockHeight - 1) / nominalBlockHeight;

            int framePixelsLength = width * height;

            deflater = new Deflater(parameters.CompressionLevel, false);
            reserveBytesPerFrame = (nominalBlockHeight * nominalBlockWidth * 3 + 2 /*block header size*/ + ZLibWorstCaseInflation) * rows * cols + 4 /*frame header size*/;

            previousFramePixels = new int[framePixelsLength];
            currentFramePixels = new int[framePixelsLength];
            blockBuffer = new byte[nominalBlockHeight * nominalBlockWidth * 3];

            flvWriter = new FlvWriter(FlvWriter.FlvFlags.Video);
            var flvMetadata = new FlvWriter.FlvMetadata()
            {
                { "duration", 0.0 },
                { "width", width },
                { "height", height },
                { "framerate", framesPerSecond },
                { "videocodecid", 3 /*screen video*/ }
            };
            flvMetadataUpdater = flvWriter.WriteFlvMetaFrame(flvMetadata, 0);
        }

        /// <summary>
        /// Gets the video parameters.
        /// </summary>
        new public FlashScreenVideoParameters Parameters
        {
            get { return (FlashScreenVideoParameters) base.Parameters; }
        }

        /// <inheritdoc />
        public override string MimeType
        {
            get { return MimeTypes.FlashVideo; }
        }

        /// <inheritdoc />
        protected override void AddFrameImpl(VideoFrame frame)
        {
            SwitchFramePixels();
            LoadCurrentFramePixels(frame);

            bool keyFrame = frameCount % keyFramePeriod == 0;

            FlvWriter.FlvVideoFrameFlags frameFlags = FlvWriter.FlvVideoFrameFlags.Codec_ScreenVideo
                | (keyFrame ? FlvWriter.FlvVideoFrameFlags.Type_KeyFrame : FlvWriter.FlvVideoFrameFlags.Type_InterFrame);

            flvWriter.WriteFlvVideoFrame(frameFlags,
                (int) (frameCount * 1000.0 / framesPerSecond),
                reserveBytesPerFrame,
                delegate (byte[] buffer, ref int bufferOffset)
            {
                // write frame header
                buffer[bufferOffset++] = (byte)(((nominalBlockWidth / 16 - 1) << 4) | (width >> 8));
                buffer[bufferOffset++] = (byte)width;
                buffer[bufferOffset++] = (byte)(((nominalBlockHeight / 16 - 1) << 4) | (height >> 8));
                buffer[bufferOffset++] = (byte)height;

                // proceed from bottom-left to top-right row by row
                for (int blockYOrigin = height; blockYOrigin >= 0; blockYOrigin -= nominalBlockHeight)
                {
                    int blockHeight = Math.Min(blockYOrigin, nominalBlockHeight);

                    for (int blockXOrigin = 0; blockXOrigin < width; blockXOrigin += nominalBlockWidth)
                    {
                        int blockWidth = Math.Min(width - blockXOrigin, nominalBlockWidth);

                        bool diff = false;
                        int blockBufferOffset = 0;
                        int frameOffset = blockYOrigin * width + blockXOrigin;
                        for (int y = 0; y < blockHeight; y++)
                        {
                            frameOffset -= width;

                            for (int x = 0; x < blockWidth; x++)
                            {
                                int color = currentFramePixels[frameOffset];

                                blockBuffer[blockBufferOffset++] = (byte) color; // B
                                blockBuffer[blockBufferOffset++] = (byte) (color >> 8); // G
                                blockBuffer[blockBufferOffset++] = (byte) (color >> 16); // R

                                if (previousFramePixels[frameOffset++] != color)
                                    diff = true;
                            }

                            frameOffset -= blockWidth;
                        }

                        if (keyFrame || diff)
                        {
                            deflater.Reset();
                            deflater.SetInput(blockBuffer, 0, blockBufferOffset);
                            deflater.Finish();

                            int blockLength = deflater.Deflate(buffer, bufferOffset + 2, buffer.Length - bufferOffset - 2);
                            Debug.Assert(deflater.IsFinished, "Deflater should be finished.");

                            buffer[bufferOffset++] = (byte) (blockLength >> 8);
                            buffer[bufferOffset++] = (byte) blockLength;
                            bufferOffset += blockLength;
                        }
                        else
                        {
                            buffer[bufferOffset++] = 0;
                            buffer[bufferOffset++] = 0;
                        }
                    }
                }
            });

            frameCount += 1;
        }

        private void SwitchFramePixels()
        {
            int[] temp = previousFramePixels;
            previousFramePixels = currentFramePixels;
            currentFramePixels = temp;
        }

        private void LoadCurrentFramePixels(VideoFrame frame)
        {
            frame.CopyPixels(new Rectangle(0, 0, width, height), currentFramePixels, 0, width);
        }

        /// <inheritdoc />
        protected override void SaveImpl(Stream stream)
        {
            flvMetadataUpdater.Update("duration", frameCount / framesPerSecond);

            flvWriter.WriteTo(stream);
        }
    }
}
