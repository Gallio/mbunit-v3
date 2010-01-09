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
using System.Threading;
using Gallio.Common.Markup;
using Gallio.Common.Media;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(ScreenRecorder))]
    public class ScreenRecorderTest
    {
        [Test]
        public void Constructor_WhenScreenGrabberIsNull_Throws()
        {
            var video = new FlashScreenVideo(new FlashScreenVideoParameters(100, 100, 5));

            Assert.Throws<ArgumentNullException>(() => new ScreenRecorder(null, video));
        }

        [Test]
        public void Constructor_WhenVideoIsNull_Throws()
        {
            var grabber = new ScreenGrabber(new CaptureParameters());

            Assert.Throws<ArgumentNullException>(() => new ScreenRecorder(grabber, null));
        }

        [Test]
        public void Constructor_WhenVideoSizeDoesNotMatchGrabberScreenshotsSize_Throws()
        {
            var grabber = new ScreenGrabber(new CaptureParameters());

            var video = new FlashScreenVideo(new FlashScreenVideoParameters(1, grabber.ScreenshotHeight, 5));
            var ex = Assert.Throws<ArgumentException>(() => new ScreenRecorder(grabber, video));
            Assert.Contains(ex.Message, "The video dimensions must be exactly the same as the screenshots obtained by the grabber.");

            video = new FlashScreenVideo(new FlashScreenVideoParameters(grabber.ScreenshotWidth, 1, 5));
            ex = Assert.Throws<ArgumentException>(() => new ScreenRecorder(grabber, video));
            Assert.Contains(ex.Message, "The video dimensions must be exactly the same as the screenshots obtained by the grabber.");
        }

        [Test]
        public void Grabber_ReturnsGrabber()
        {
            var grabber = new ScreenGrabber(new CaptureParameters());
            var video = new FlashScreenVideo(new FlashScreenVideoParameters(grabber.ScreenshotWidth, grabber.ScreenshotHeight, 5));
            using (var recorder = new ScreenRecorder(grabber, video))
            {
                Assert.AreSame(grabber, recorder.Grabber);
            }
        }

        [Test]
        public void Grabber_WhenDisposed_Throws()
        {
            var grabber = new ScreenGrabber(new CaptureParameters());
            var video = new FlashScreenVideo(new FlashScreenVideoParameters(grabber.ScreenshotWidth, grabber.ScreenshotHeight, 5));
            var recorder = new ScreenRecorder(grabber, video);
            recorder.Dispose();

            ScreenGrabber x;
            Assert.Throws<ObjectDisposedException>(() => x = recorder.Grabber);
        }

        [Test]
        public void Video_ReturnsVideo()
        {
            var grabber = new ScreenGrabber(new CaptureParameters());
            var video = new FlashScreenVideo(new FlashScreenVideoParameters(grabber.ScreenshotWidth, grabber.ScreenshotHeight, 5));
            using (var recorder = new ScreenRecorder(grabber, video))
            {
                Assert.AreSame(video, recorder.Video);
            }
        }

        [Test]
        public void Video_WhenDisposed_Throws()
        {
            var grabber = new ScreenGrabber(new CaptureParameters());
            var video = new FlashScreenVideo(new FlashScreenVideoParameters(grabber.ScreenshotWidth, grabber.ScreenshotHeight, 5));
            var recorder = new ScreenRecorder(grabber, video);
            recorder.Dispose();

            Video x;
            Assert.Throws<ObjectDisposedException>(() => x = recorder.Video);
        }

        [Test]
        public void Start_WhenDisposed_Throws()
        {
            var grabber = new ScreenGrabber(new CaptureParameters());
            var video = new FlashScreenVideo(new FlashScreenVideoParameters(grabber.ScreenshotWidth, grabber.ScreenshotHeight, 5));
            var recorder = new ScreenRecorder(grabber, video);
            recorder.Dispose();

            Assert.Throws<ObjectDisposedException>(() => recorder.Start());
        }

        [Test]
        public void Stop_WhenDisposed_Throws()
        {
            var grabber = new ScreenGrabber(new CaptureParameters());
            var video = new FlashScreenVideo(new FlashScreenVideoParameters(grabber.ScreenshotWidth, grabber.ScreenshotHeight, 5));
            var recorder = new ScreenRecorder(grabber, video);
            recorder.Dispose();

            Assert.Throws<ObjectDisposedException>(() => recorder.Stop());
        }

        [Test]
        public void Start_CapturesVideoUntilStopped()
        {
            var grabber = new ScreenGrabber(new CaptureParameters() { Zoom = 0.25 });
            var video = new FlashScreenVideo(new FlashScreenVideoParameters(grabber.ScreenshotWidth, grabber.ScreenshotHeight, 5));
            using (var recorder = new ScreenRecorder(grabber, video))
            {
                recorder.Start();
                Thread.Sleep(2000);
                recorder.Stop();

                TestLog.EmbedVideo("Video", recorder.Video);
            }
        }
    }
}
