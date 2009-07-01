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
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Gallio.Common.Media;
using Gallio.Framework;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(CaptionOverlay))]
    public class CaptionOverlayTest
    {
        [Test]
        public void Text_CanGetSet()
        {
            var overlay = new CaptionOverlay();

            Assert.AreEqual("", overlay.Text);

            overlay.Text = "New value";
            Assert.AreEqual("New value", overlay.Text);

            Assert.Throws<ArgumentNullException>(() => overlay.Text = null);
        }

        [Test]
        public void FontSize_CanGetSet()
        {
            var overlay = new CaptionOverlay();

            Assert.AreEqual(16, overlay.FontSize);

            overlay.FontSize = 12;
            Assert.AreEqual(12, overlay.FontSize);

            Assert.Throws<ArgumentOutOfRangeException>(() => overlay.FontSize = 0);
            Assert.Throws<ArgumentOutOfRangeException>(() => overlay.FontSize = -1);
        }

        [Test]
        public void HorizontalAlignment_CanGetSet()
        {
            var overlay = new CaptionOverlay();

            Assert.AreEqual(HorizontalAlignment.Left, overlay.HorizontalAlignment);

            overlay.HorizontalAlignment = HorizontalAlignment.Center;
            Assert.AreEqual(HorizontalAlignment.Center, overlay.HorizontalAlignment);
        }

        [Test]
        public void VerticalAlignment_CanGetSet()
        {
            var overlay = new CaptionOverlay();

            Assert.AreEqual(VerticalAlignment.Top, overlay.VerticalAlignment);

            overlay.VerticalAlignment = VerticalAlignment.Bottom;
            Assert.AreEqual(VerticalAlignment.Bottom, overlay.VerticalAlignment);
        }

        [Test]
        public void Paint()
        {
            var overlayManager = new OverlayManager();
            var overlay = new CaptionOverlay()
            {
                Text = "This is some text.",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            overlayManager.AddOverlay(overlay);

            using (Bitmap bitmap = CreateBitmapWithBackground())
            {
                overlayManager.PaintOverlaysOnImage(bitmap, 0, 0);

                TestLog.WriteLine("Image should contain 'This is some text.' centered at the bottom.");
                TestLog.EmbedImage("Image", bitmap);
            }
        }

        private static Bitmap CreateBitmapWithBackground()
        {
            const int width = 400;
            const int height = 300;

            Bitmap bitmap = new Bitmap(width, height);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                double scale = 8.0 / (height * Math.PI);
                int xCenter = width / 2;
                int yCenter = height / 2;

                for (int y = 0, rowOffset = 0; y < height; y++, rowOffset += bitmapData.Stride)
                {
                    for (int x = 0, pixelOffset = rowOffset; x < width; x++, pixelOffset += 4)
                    {
                        int r = (int)Math.Floor(Math.Sin((x - xCenter) * scale) * Math.Cos((y - yCenter) * scale) * 127) + 127;
                        int g = 0;
                        int b = 0;
                        int pixel = (r << 16) | (g << 8) | b;

                        Marshal.WriteInt32(bitmapData.Scan0, pixelOffset, pixel);
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }
    }
}
