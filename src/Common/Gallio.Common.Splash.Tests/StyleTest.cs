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
using MbUnit.Framework.ContractVerifiers;

namespace Gallio.Common.Splash.Tests
{
    public class StyleTest
    {
        [Test]
        public void CreateDefaultStyle_ReturnsStyleGeneratedFromSystemProperties()
        {
            Style style = Style.Default;

            Assert.Multiple(() =>
            {
                Assert.AreEqual(SystemFonts.DefaultFont, style.Font);
                Assert.AreEqual(SystemColors.WindowText, style.Color);
                Assert.AreEqual(60, ((PixelTabStopRuler)style.TabStopRuler).PixelsPerTabStop);
                Assert.AreEqual(10, ((PixelTabStopRuler)style.TabStopRuler).MinimumTabWidth);
                Assert.IsTrue(style.WordWrap);
                Assert.AreEqual(0, style.LeftMargin);
                Assert.AreEqual(0, style.RightMargin);
                Assert.AreEqual(0, style.FirstLineIndent);
            });
        }

        [VerifyContract]
        public readonly IContract EqualityAndHashCode = new EqualityContract<Style>()
        {
            ImplementsOperatorOverloads = false,
            EquivalenceClasses =
            {
                { Style.Default, new StyleBuilder().ToStyle() },
                { new StyleBuilder() { Color = Color.Red }.ToStyle(), new StyleBuilder() { Color = Color.Red }.ToStyle() },
                { new StyleBuilder() { Font = new Font(Style.Default.Font, FontStyle.Strikeout) }.ToStyle(), new StyleBuilder() { Font = new Font(Style.Default.Font, FontStyle.Strikeout) }.ToStyle() },
                { new StyleBuilder() { TabStopRuler = new PixelTabStopRuler(30, 10) }.ToStyle(), new StyleBuilder() { TabStopRuler = new PixelTabStopRuler(30, 10) }.ToStyle() },
                { new StyleBuilder() { WordWrap = false }.ToStyle(), new StyleBuilder() { WordWrap = false }.ToStyle() },
                { new StyleBuilder() { LeftMargin = 5 }.ToStyle(), new StyleBuilder() { LeftMargin = 5 }.ToStyle() },
                { new StyleBuilder() { RightMargin = 10 }.ToStyle(), new StyleBuilder() { RightMargin = 10 }.ToStyle() },
                { new StyleBuilder() { FirstLineIndent = 15 }.ToStyle(), new StyleBuilder() { FirstLineIndent = 15 }.ToStyle() }
            }
        };
    }
}
