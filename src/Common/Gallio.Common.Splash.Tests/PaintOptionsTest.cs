using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MbUnit.Framework;

namespace Gallio.Common.Splash.Tests
{
    public class PaintOptionsTest
    {
        [Test]
        public void Constructor_SetsPropertiesToDefault()
        {
            var options = new PaintOptions();
            Assert.AreEqual(SystemColors.Window, options.BackgroundColor);
            Assert.AreEqual(SystemColors.HighlightText, options.SelectedTextColor);
            Assert.AreEqual(SystemColors.Highlight, options.SelectedBackgroundColor);
        }

        [Test]
        public void BackgroundColor_SetGet()
        {
            var options = new PaintOptions();

            options.BackgroundColor = Color.Red;
            Assert.AreEqual(Color.Red, options.BackgroundColor);
        }

        [Test]
        public void SelectedTextColor_SetGet()
        {
            var options = new PaintOptions();

            options.SelectedTextColor = Color.Red;
            Assert.AreEqual(Color.Red, options.SelectedTextColor);
        }

        [Test]
        public void SelectedBackgroundColor_SetGet()
        {
            var options = new PaintOptions();

            options.SelectedBackgroundColor = Color.Red;
            Assert.AreEqual(Color.Red, options.SelectedBackgroundColor);
        }
    }
}
