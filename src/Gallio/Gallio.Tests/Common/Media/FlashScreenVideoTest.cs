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
using System.Drawing;
using System.Linq;
using System.Text;
using Gallio.Common.Markup;
using Gallio.Common.Media;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(FlashScreenVideo))]
    public class FlashScreenVideoTest
    {
        [Test]
        public void Constructor_WhenParametersIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new FlashScreenVideo(null));
        }

        [Test]
        public void MimeType_ReturnsFlashVideoMimeType()
        {
            var flashScreenVideo = new FlashScreenVideo(new FlashScreenVideoParameters(32, 16, 30));

            Assert.AreEqual(MimeTypes.FlashVideo, flashScreenVideo.MimeType);
        }

        [Test]
        public void AddFrame_WhenFrameIsNull_Throws()
        {
            var flashScreenVideo = new FlashScreenVideo(new FlashScreenVideoParameters(32, 16, 30));

            Assert.Throws<ArgumentNullException>(() => flashScreenVideo.AddFrame(null));
        }

        [Test]
        public void AddFrame_WhenFrameWidthIsIncorrect_Throws()
        {
            var flashScreenVideo = new FlashScreenVideo(new FlashScreenVideoParameters(32, 16, 30));

            var ex = Assert.Throws<ArgumentException>(() => flashScreenVideo.AddFrame(new BitmapVideoFrame(new Bitmap(30, 16))));
            Assert.Contains(ex.Message, "The frame dimensions must exactly equal those of the video.");
        }

        [Test]
        public void AddFrame_WhenFrameHeightIsIncorrect_Throws()
        {
            var flashScreenVideo = new FlashScreenVideo(new FlashScreenVideoParameters(32, 16, 30));

            var ex = Assert.Throws<ArgumentException>(() => flashScreenVideo.AddFrame(new BitmapVideoFrame(new Bitmap(32, 20))));
            Assert.Contains(ex.Message, "The frame dimensions must exactly equal those of the video.");
        }

        [Test]
        public void FrameCount_WhenInitiallyCreated_IsZero()
        {
            var flashScreenVideo = new FlashScreenVideo(new FlashScreenVideoParameters(32, 32, 30));

            Assert.AreEqual(0, flashScreenVideo.FrameCount);
        }

        [Test]
        public void AddFrame_WhenFrameIsValid_IncreasesFrameCount()
        {
            var flashScreenVideo = new FlashScreenVideo(new FlashScreenVideoParameters(32, 32, 30));

            flashScreenVideo.AddFrame(new BitmapVideoFrame(new Bitmap(32, 32)));
            Assert.AreEqual(1, flashScreenVideo.FrameCount);

            flashScreenVideo.AddFrame(new BitmapVideoFrame(new Bitmap(32, 32)));
            Assert.AreEqual(2, flashScreenVideo.FrameCount);
        }

        [Test]
        public void Save_WhenStreamIsNull_Throws()
        {
            var flashScreenVideo = new FlashScreenVideo(new FlashScreenVideoParameters(32, 16, 30));

            Assert.Throws<ArgumentNullException>(() => flashScreenVideo.Save(null));
        }
    }
}
