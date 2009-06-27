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
        public void Save_WhenStreamIsNull_Throws()
        {
            var flashScreenVideo = new FlashScreenVideo(new FlashScreenVideoParameters(32, 16, 30));

            Assert.Throws<ArgumentNullException>(() => flashScreenVideo.Save(null));
        }
    }
}
