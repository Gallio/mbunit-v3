// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Gallio.Icarus.Controls;
using Gallio.Icarus.Tests.Properties;

using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Icarus.Tests.Controls
{
    [TestFixture, Category("Controls")]
    class TestStatusBarTest : TestStatusBar
    {
        private TestStatusBar testStatusBar;

        [SetUp]
        public void SetUp()
        {
            testStatusBar = new TestStatusBar();
        }

        [Test]
        public void BackgroundImage_Test()
        {
            testStatusBar.BackgroundImage = Resources.MbUnitLogo;
            //Assert.AreEqual(Resources.MbUnitLogo, testStatusBar.BackgroundImage);
        }

        [Test]
        public void BackgroundImageLayout_Test()
        {
            testStatusBar.BackgroundImageLayout = ImageLayout.Center;
            Assert.AreEqual(ImageLayout.Center, testStatusBar.BackgroundImageLayout);
        }

        [Test]
        public void Clear_Test()
        {
            Assert.AreEqual(0, testStatusBar.Passed);
            Assert.AreEqual(0, testStatusBar.Failed);
            Assert.AreEqual(0, testStatusBar.Skipped);
            Assert.AreEqual(0, testStatusBar.Inconclusive);
            Assert.AreEqual(0, testStatusBar.ElapsedTime);
            testStatusBar.Passed = 5;
            testStatusBar.Failed = 3;
            testStatusBar.Skipped = 15;
            testStatusBar.Inconclusive = 5;
            testStatusBar.ElapsedTime = 12;
            testStatusBar.Clear();
            Assert.AreEqual(0, testStatusBar.Passed);
            Assert.AreEqual(0, testStatusBar.Failed);
            Assert.AreEqual(0, testStatusBar.Skipped);
            Assert.AreEqual(0, testStatusBar.Inconclusive);
            Assert.AreEqual(0, testStatusBar.ElapsedTime);
        }

        [Test]
        public void FailedColor_Test()
        {
            Assert.AreEqual(Color.Red, testStatusBar.FailedColor);
            testStatusBar.FailedColor = Color.Black;
            Assert.AreEqual(Color.Black, testStatusBar.FailedColor);
        }

        [Test]
        public void SkippedColor_Test()
        {
            Assert.AreEqual(Color.SlateGray, testStatusBar.SkippedColor);
            testStatusBar.SkippedColor = Color.Black;
            Assert.AreEqual(Color.Black, testStatusBar.SkippedColor);
        }

        [Test]
        public void InconclusiveColor_Test()
        {
            Assert.AreEqual(Color.Gold, testStatusBar.InconclusiveColor);
            testStatusBar.InconclusiveColor = Color.Black;
            Assert.AreEqual(Color.Black, testStatusBar.InconclusiveColor);
        }

        [Test]
        public void PassedColor_Test()
        {
            Assert.AreEqual(Color.Green, testStatusBar.PassedColor);
            testStatusBar.PassedColor = Color.Black;
            Assert.AreEqual(Color.Black, testStatusBar.PassedColor);
        }

        [Test]
        public void Mode_Test()
        {
            testStatusBar.Mode = "test";
            Assert.AreEqual("test", testStatusBar.Mode);
        }

        [Test]
        public void Text_Test()
        {
            Assert.AreEqual("{0} tests - {1} passed - {2} failed - {3} inconclusive - {4} skipped - {5:0.0}s", 
                testStatusBar.Text);
        }

        [Test]
        public void Total_Test()
        {
            Assert.AreEqual(0, testStatusBar.Total);
            testStatusBar.Total = 15;
            Assert.AreEqual(15, testStatusBar.Total);
        }

        [Test]
        public void OnPaint_Test()
        {
            MockRepository mockRepository = new MockRepository();
            Graphics g = mockRepository.StrictMock<Graphics>();
            using (mockRepository.Record())
            {
                g.FillRectangle(new SolidBrush(Color.Black), new Rectangle());
                LastCall.IgnoreArguments();
                Expect.Call(g.SmoothingMode).Return(SmoothingMode.Default);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawRectangle(Pens.Black, new Rectangle());
                LastCall.IgnoreArguments();
                g.DrawString(string.Empty, Font, new SolidBrush(Color.Empty), 0, 0, new StringFormat());
                LastCall.IgnoreArguments();
                g.SmoothingMode = SmoothingMode.Default;
            }

            mockRepository.ReplayAll();

            OnPaint(new PaintEventArgs(g, new Rectangle()));

            mockRepository.VerifyAll();
        }
    }
}
