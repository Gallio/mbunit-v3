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

namespace Gallio.Common.Media
{
    /// <summary>
    /// Specifies the parameters for a Flash screen video.
    /// </summary>
    /// <seealso cref="FlashScreenVideo"/>
    public class FlashScreenVideoParameters : VideoParameters
    {
        private int blockWidth;
        private int blockHeight;
        private int compressionLevel;
        private int keyFramePeriod;

        /// <summary>
        /// Creates a Flash video parameters object.
        /// </summary>
        /// <param name="width">The video width.</param>
        /// <param name="height">The video height.</param>
        /// <param name="framesPerSecond">The number of frames per second.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="width"/> or <paramref name="height"/>
        /// are not between 1 and 4095 or if <paramref name="framesPerSecond"/> is not greater than zero.</exception>
        public FlashScreenVideoParameters(int width, int height, double framesPerSecond)
            : base(width, height, framesPerSecond)
        {
            if (width > 4095)
                throw new ArgumentOutOfRangeException("width", "The width must be less than 4096.");
            if (height > 4095)
                throw new ArgumentOutOfRangeException("height", "The height must be less than 4096.");

            blockWidth = 64;
            blockHeight = 64;
            compressionLevel = 6;
            keyFramePeriod = (int) Math.Ceiling(framesPerSecond);
        }

        /// <summary>
        /// Gets or sets the width of each block to be compressed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The block height must be a multiple of 16 between 16 and 256.  The default is 64.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is invalid.</exception>
        public int BlockWidth
        {
            get { return blockWidth; }
            set
            {
                if (value < 16 || value > 256 || value % 16 != 0)
                    throw new ArgumentOutOfRangeException("value", "Block width must be a multiple of 16 between 16 and 256.");
                blockWidth = value;
            }
        }

        /// <summary>
        /// Gets or sets the height of each block to be compressed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The block height must be a multiple of 16 between 16 and 256.  The default is 64.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is invalid.</exception>
        public int BlockHeight
        {
            get { return blockHeight; }
            set
            {
                if (value < 16 || value > 256 || value % 16 != 0)
                    throw new ArgumentOutOfRangeException("value", "Block height must be a multiple of 16 between 16 and 256.");
                blockHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the ZLib compression level.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Range: 0 (fast / uncompressed), 1 (fast / worst compression), 6 (default), 9 (slow / best compression).
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is invalid.</exception>
        public int CompressionLevel
        {
            get { return compressionLevel; }
            set
            {
                if (value < 0 || value > 9)
                    throw new ArgumentOutOfRangeException("value", "Compression level must be between 0 and 9.");
                compressionLevel = value;
            }
        }

        /// <summary>
        /// Gets the number of frames between successive key frames.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default is <see cref="VideoParameters.FramesPerSecond" /> which produces approximately one key frame per second.
        /// If you use a value of 1, then every frame will be a key frame.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less than 1.</exception>
        public int KeyFramePeriod
        {
            get { return keyFramePeriod; }
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException("value", "Key frame period must be at least 1.");
                keyFramePeriod = value;
            }
        }
    }
}
