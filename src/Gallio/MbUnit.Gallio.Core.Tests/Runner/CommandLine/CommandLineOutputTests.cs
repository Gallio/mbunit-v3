// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

extern alias MbUnit2;
using System;
using System.IO;
using System.Text;
using MbUnit.Core.Runner.CommandLine;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Runner.CommandLine
{
    [TestFixture]
    public class CommandLineOutputTests
    {
        private StringBuilder _sbOutput;
        private StringWriter _writer;
        private CommandLineOutput _output;

        [SetUp]
        public void TestStart()
        {
            _sbOutput = new StringBuilder();
            _writer = new StringWriter(_sbOutput);
            _output = new CommandLineOutput(_writer, 80);
        }

        [Test]
        public void DefaultConstractorTest()
        {
            CommandLineOutput output = new CommandLineOutput();
            Console.WriteLine(output.Output.GetType());
            Assert.AreEqual(Console.Out.GetType(), output.Output.GetType());
        }

        [Test]
        public void ConstractorWithTextWriterParameter()
        {
            CommandLineOutput output = new CommandLineOutput(_writer);
            Console.WriteLine(output.Output.GetType());
            Assert.AreEqual(typeof(StringWriter), output.Output.GetType());
        }

        [RowTest]
        [Row("help", "h", "Display this help text.", ""
            , "  /help              Display this help text. (Short form: /h)\r\n")]
        [Row("help", "h", "Display this help text.", "test"
            , "  /help:<test>              Display this help text. (Short form: /h)\r\n")]
        [Row("very_long_argument", "vl", "Argument description.", ""
            , "  /very_long_argument\r\n                     Argument description. (Short form: /vl)\r\n")]
         [Row("long_description", "ld", "It is a very long description. It is a very long description. It is a very long description. It is a very long description.", ""
           , "  /long_description  It is a very long description. It is a very long\r\n                     description. It is a very long description. It is a very\r\n                     long description. (Short form: /ld)\r\n")]
        public void PringArgumentHelpTest(string longName, string shortName, string description, string valueType, string expectedOutput)
        {
            _output.PrintArgumentHelp(longName, shortName, description, valueType);
            Assert.AreEqual(expectedOutput, _sbOutput.ToString());
        }

        [RowTest]
        [Row("long_description", "ld", "It is a kind of long description.", ""
     , "  /long_description  It is a kind of long\r\n                     description. (Short\r\n                     form: /ld)\r\n")]
        public void PrintArgumentHelpWidth40Chars(string longName, string shortName, string description, string valueType, string expectedOutput)
        {
            _output.LineLength = 40;
            _output.PrintArgumentHelp(longName, shortName, description, valueType);
            Assert.AreEqual(expectedOutput, _sbOutput.ToString());
        }

        [Test]
        public void PrintTextTest()
        {
            _output.PrintText("Some Text", 3);
            Assert.AreEqual("   Some Text\r\n", _sbOutput.ToString());
        }

        [Test]
        public void NewLineTest()
        {
            _output.NewLine();
            Assert.AreEqual("\r\n", _sbOutput.ToString());
        }

        private enum EnumTypeTest
        {
            Test1,
            Test2,
            Test3
        } ;
    }
}