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
