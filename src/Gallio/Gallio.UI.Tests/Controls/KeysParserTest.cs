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
using System.Windows.Forms;
using Gallio.UI.Controls;
using MbUnit.Framework;

namespace Gallio.UI.Tests.Controls
{
    public class KeysParserTest
    {
        private KeysParser keysParser;

        [SetUp]
        public void SetUp() {
            keysParser = new KeysParser();
        }

        [Test]
        public void Single_symbol_shortcuts_should_parse_correctly()
        {    
            var keys = keysParser.Parse("S");

            Assert.AreEqual(Keys.S, keys);
        }

        [Test]
        public void Unrecognised_keys_should_throw_an_exception()
        {
            Assert.Throws<Exception>(() => keysParser.Parse("blah"));
        }

        [Test]
        public void Shortcuts_with_a_modifier_should_parse_correctly()
        {
            var keys = keysParser.Parse("Alt + S");

            Assert.AreEqual(Keys.Alt | Keys.S, keys);
        }

        [Test]
        public void Ctrl_should_be_picked_up_as_Control()
        {
            var keys = keysParser.Parse("Ctrl");

            Assert.AreEqual(Keys.Control, keys);
        }
    }
}
