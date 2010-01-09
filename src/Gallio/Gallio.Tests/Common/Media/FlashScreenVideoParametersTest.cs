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
using System.Drawing;
using System.Linq;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Common.Media;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(FlashScreenVideoParameters))]
    public class FlashScreenVideoParametersTest
    {
        [Test]
        public void Constructor_ValidatesWidth()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new FlashScreenVideoParameters(0, 16, 30));
            Assert.Contains(ex.Message, "The width must be at least 1.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new FlashScreenVideoParameters(4096, 16, 30));
            Assert.Contains(ex.Message, "The width must be less than 4096.");

            Assert.DoesNotThrow(() => new FlashScreenVideoParameters(1, 16, 30));

            Assert.DoesNotThrow(() => new FlashScreenVideoParameters(4095, 16, 30));

            Assert.DoesNotThrow(() => new FlashScreenVideoParameters(100, 16, 30));
        }

        [Test]
        public void Constructor_ValidatesHeight()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new FlashScreenVideoParameters(16, 0, 30));
            Assert.Contains(ex.Message, "The height must be at least 1.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new FlashScreenVideoParameters(16, 4096, 30));
            Assert.Contains(ex.Message, "The height must be less than 4096.");

            Assert.DoesNotThrow(() => new FlashScreenVideoParameters(16, 1, 30));

            Assert.DoesNotThrow(() => new FlashScreenVideoParameters(16, 4095, 30));

            Assert.DoesNotThrow(() => new FlashScreenVideoParameters(16, 100, 30));
        }

        [Test]
        public void Constructor_ValidatesFramesPerSecond()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new FlashScreenVideoParameters(16, 16, 0));
            Assert.Contains(ex.Message, "Frames per second must be non-zero and positive.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => new FlashScreenVideoParameters(16, 16, -1));
            Assert.Contains(ex.Message, "Frames per second must be non-zero and positive.");

            Assert.DoesNotThrow(() => new FlashScreenVideoParameters(16, 16, 1));

            Assert.DoesNotThrow(() => new FlashScreenVideoParameters(16, 16, 30));
        }

        [Test]
        public void Width_ReturnsWidth()
        {
            var parameters = new FlashScreenVideoParameters(32, 16, 30);

            Assert.AreEqual(32, parameters.Width);
        }

        [Test]
        public void Height_ReturnsHeight()
        {
            var parameters = new FlashScreenVideoParameters(32, 16, 30);

            Assert.AreEqual(16, parameters.Height);
        }

        [Test]
        public void FramesPerSecond_ReturnsFramesPerSecond()
        {
            var parameters = new FlashScreenVideoParameters(32, 16, 30);

            Assert.AreEqual(30, parameters.FramesPerSecond);
        }

        [Test]
        public void BlockWidth_CanGetAndSetValue()
        {
            var parameters = new FlashScreenVideoParameters(32, 16, 30);

            Assert.AreEqual(64, parameters.BlockWidth);

            parameters.BlockWidth = 256;
            Assert.AreEqual(256, parameters.BlockWidth);

            parameters.BlockWidth = 16;
            Assert.AreEqual(16, parameters.BlockWidth);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.BlockWidth = 0);
            Assert.Contains(ex.Message, "Block width must be a multiple of 16 between 16 and 256.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.BlockWidth = 256 + 16);
            Assert.Contains(ex.Message, "Block width must be a multiple of 16 between 16 and 256.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.BlockWidth = 17);
            Assert.Contains(ex.Message, "Block width must be a multiple of 16 between 16 and 256.");
        }

        [Test]
        public void BlockHeight_CanGetAndSetValue()
        {
            var parameters = new FlashScreenVideoParameters(32, 16, 30);

            Assert.AreEqual(64, parameters.BlockHeight);

            parameters.BlockHeight = 256;
            Assert.AreEqual(256, parameters.BlockHeight);

            parameters.BlockHeight = 16;
            Assert.AreEqual(16, parameters.BlockHeight);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.BlockHeight = 0);
            Assert.Contains(ex.Message, "Block height must be a multiple of 16 between 16 and 256.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.BlockHeight = 256 + 16);
            Assert.Contains(ex.Message, "Block height must be a multiple of 16 between 16 and 256.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.BlockHeight = 17);
            Assert.Contains(ex.Message, "Block height must be a multiple of 16 between 16 and 256.");
        }

        [Test]
        public void CompressionLevel_CanGetAndSetValue()
        {
            var parameters = new FlashScreenVideoParameters(32, 16, 30);

            Assert.AreEqual(6, parameters.CompressionLevel);

            parameters.CompressionLevel = 9;
            Assert.AreEqual(9, parameters.CompressionLevel);

            parameters.CompressionLevel = 0;
            Assert.AreEqual(0, parameters.CompressionLevel);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.CompressionLevel = -1);
            Assert.Contains(ex.Message, "Compression level must be between 0 and 9.");

            ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.CompressionLevel = 10);
            Assert.Contains(ex.Message, "Compression level must be between 0 and 9.");
        }
        
        [Test]
        public void KeyFramePeriod_CanGetAndSetValue()
        {
            var parameters = new FlashScreenVideoParameters(32, 16, 30);

            Assert.AreEqual(TimeSpan.FromSeconds(2), parameters.KeyFramePeriod);

            parameters.KeyFramePeriod = TimeSpan.FromSeconds(0);
            Assert.AreEqual(TimeSpan.FromSeconds(0), parameters.KeyFramePeriod);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => parameters.KeyFramePeriod = TimeSpan.FromSeconds(-1));
            Assert.Contains(ex.Message, "Key frame period must not be negative.");
        }
    }
}
