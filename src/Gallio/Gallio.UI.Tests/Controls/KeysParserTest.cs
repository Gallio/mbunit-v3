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
