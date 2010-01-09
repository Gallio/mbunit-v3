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
using Gallio.Common.Media;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Media
{
    [TestsOn(typeof(BitmapVideoFrame))]
    public class BitmapVideoFrameTest
    {
        [Test]
        public void Constructor_WhenBitmapIsNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new BitmapVideoFrame(null));
        }

        [Test]
        public void Bitmap_ReturnsBitmapProvidedInConstructor()
        {
            var bitmap = new Bitmap(32, 32);
            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);

            Assert.AreSame(bitmap, bitmapVideoFrame.Bitmap);
        }

        [Test]
        public void Width_ReturnsBitmapWidth()
        {
            var bitmap = new Bitmap(32, 16);
            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);

            Assert.AreEqual(32, bitmapVideoFrame.Width);
        }

        [Test]
        public void Height_ReturnsBitmapHeight()
        {
            var bitmap = new Bitmap(32, 16);
            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);

            Assert.AreEqual(16, bitmapVideoFrame.Height);
        }

        [Test]
        public void CopyPixels_WhenPixelBufferIsNull_Throws()
        {
            var bitmap = new Bitmap(32, 32);
            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);
            var rectangle = new Rectangle(0, 0, 32, 32);
            int[] pixelBuffer = null;
            int startOffset = 0;
            int stride = 32;

            Assert.Throws<ArgumentNullException>(() => bitmapVideoFrame.CopyPixels(rectangle, pixelBuffer, startOffset, stride));
        }

        [Test]
        [Row(-1, 0, 32, 32)]
        [Row(0, -1, 32, 32)]
        [Row(0, 0, -1, 32)]
        [Row(0, 0, 32, -1)]
        [Row(2, 2, 32, 30)]
        [Row(2, 2, 30, 32)]
        public void CopyPixels_WhenRectangleAreOutOfBounds_Throws(int x, int y, int width, int height)
        {
            var bitmap = new Bitmap(32, 32);
            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);
            var rectangle = new Rectangle(x, y, width, height);
            int[] pixelBuffer = new int[32 * 32];
            int startOffset = 0;
            int stride = 32;

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => bitmapVideoFrame.CopyPixels(rectangle, pixelBuffer, startOffset, stride));
            Assert.Contains(ex.Message, "Rectangle position and dimensions must be within the bounds of the frame.");
        }

        [Test]
        [Row(-1)]
        [Row(32 * 32)]
        public void CopyPixels_WhenStartOffsetIsOutOfBounds_Throws(int startOffset)
        {
            var bitmap = new Bitmap(32, 32);
            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);
            var rectangle = new Rectangle(0, 0, 32, 32);
            int[] pixelBuffer = new int[32 * 32];
            int stride = 32;

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => bitmapVideoFrame.CopyPixels(rectangle, pixelBuffer, startOffset, stride));
            Assert.Contains(ex.Message, "Start offset must be within the bounds of the pixel buffer.");
        }

        [Test]
        [Row(-1)]
        [Row(31)]
        public void CopyPixels_WhenStrideIsLessThanWidth_Throws(int stride)
        {
            var bitmap = new Bitmap(32, 32);
            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);
            var rectangle = new Rectangle(0, 0, 32, 32);
            int[] pixelBuffer = new int[32 * 32];
            int startOffset = 0;

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => bitmapVideoFrame.CopyPixels(rectangle, pixelBuffer, startOffset, stride));
            Assert.Contains(ex.Message, "Stride must be at least as large as the width of the rectangle.");
        }

        [Test]
        [Row(32, 32, 0, 32, 32 * 32)]
        [Row(32, 32, 1, 32, 32 * 32 + 1)]
        [Row(13, 16, 0, 32, 13 + 15 * 32)]
        [Row(13, 16, 5, 32, 13 + 15 * 32 + 5)]
        [Row(0, 32, 0, 100, 1)]
        [Row(32, 0, 0, 100, 1)]
        [Row(0, 0, 0, 100, 1)]
        public void CopyPixels_WhenSpaceRequirementsExceedPixelBuffer_Throws(int width, int height, int startOffset, int stride,
            int exactPixelBufferLengthRequired)
        {
            var bitmap = new Bitmap(32, 32);
            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);
            var rectangle = new Rectangle(0, 0, width, height);

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(
                    () => bitmapVideoFrame.CopyPixels(rectangle, new int[exactPixelBufferLengthRequired], startOffset, stride),
                    "Should not throw when pixel buffer is exactly the right size.");

                Assert.DoesNotThrow(
                    () => bitmapVideoFrame.CopyPixels(rectangle, new int[exactPixelBufferLengthRequired + 1], startOffset, stride),
                    "Should not throw when pixel buffer is one pixel too large.");

                if (width != 0 && height != 0)
                {
                    var ex = Assert.Throws<ArgumentException>(
                        () => bitmapVideoFrame.CopyPixels(rectangle, new int[exactPixelBufferLengthRequired - 1], startOffset, stride),
                        "Should throw when pixel buffer is one pixel too small.");
                    Assert.Contains(ex.Message, "The combined rectangle dimensions, start offset and stride would cause pixels to be written out of bounds of the pixel buffer.");
                }
            });
        }

        [Test]
        [Row(0, 0, 2, 2, 0, 2, 4)]
        [Row(0, 0, 2, 2, 1, 2, 5)]
        [Row(0, 0, 2, 2, 0, 3, 5)]
        [Row(0, 0, 4, 4, 2, 8, 32)]
        [Row(1, 2, 3, 2, 3, 4, 32)]
        public void CopyPixels_WhenArgumentsValid_ShouldCopyTheRegion(int x, int y, int width, int height, int startOffset, int stride, int pixelBufferLength)
        {
            // Create a 4x4 bitmap initialized with an array of RGB pixel colors like this:
            //   {000, 100, 200}, {001, 101, 201}, {002, 102, 202}, {003, 103, 203}
            //   {010, 110, 210}, {011, 111, 211}, {012, 112, 212}, {013, 113, 213}
            //   {020, 120, 220}, {021, 121, 221}, {022, 122, 222}, {023, 123, 223}
            //   {030, 130, 230}, {031, 131, 231}, {032, 132, 232}, {033, 133, 233}
            var bitmap = new Bitmap(4, 4);
            for (int bx = 0; bx < bitmap.Width; bx++)
            {
                for (int by = 0; by < bitmap.Height; by++)
                {
                    int component = bx + by * 10;
                    Color color = Color.FromArgb(component, component + 100, component + 200);
                    bitmap.SetPixel(bx, by, color);
                }
            }

            var bitmapVideoFrame = new BitmapVideoFrame(bitmap);
            var rectangle = new Rectangle(x, y, width, height);
            var pixelBuffer = new int[pixelBufferLength];

            bitmapVideoFrame.CopyPixels(rectangle, pixelBuffer, startOffset, stride);

            for (int i = 0; i < pixelBufferLength; i++)
            {
                int bx = (i - startOffset) % stride + x;
                int by = (i - startOffset) / stride + y;

                if (bx < x || by < y || bx - x >= width || by - y >= height)
                {
                    Assert.AreEqual(0, pixelBuffer[i], "i: {0}, bx: {1}, by: {2}", i, bx, by);
                }
                else
                {
                    Assert.AreEqual(bitmap.GetPixel(bx, by).ToArgb(), pixelBuffer[i], "i: {0}, bx: {1}, by: {2}", i, bx, by);
                }
            }
        }
    }
}
