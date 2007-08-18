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

namespace MbUnit.Core.Tests.Runners.CommandLine
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
            _output = new CommandLineOutput(_writer);
        }

        [Test]
        public void ConstractorWithTextWriterParameter()
        {
            CommandLineOutput output = new CommandLineOutput(_writer);
        }


        [Test]
        public void PrintTextTest()
        {
            _output.PrintText("Some Text", 3);
            Assert.AreEqual("   Some Text", _sbOutput.ToString());
        }
        
        [Test]
        public void PrintArgumentNameTest()
        {
            _output.PrintArgumentName("help", "h");
            Assert.AreEqual("  [/help|/h]", _sbOutput.ToString());
        }

        [RowTest]
        [Row(typeof(string), ":<string>")]
        [Row(typeof(int), ":<int>")]
        [Row(typeof(uint), ":<uint>")]
        [Row(typeof(bool), "[+|-]")]
        [Row(typeof(EnumTypeTest), ":{Test1|Test2|Test3}")]
        public void PrintArgumentTypeTest(Type type, string expectedOutput)
        {
            _output.PrintArgumentType(type);
            Assert.AreEqual(expectedOutput, _sbOutput.ToString());
        }

        [Test]
        [ExpectedArgumentException]
        public void PrintArgumentTypeWithUnexpectedType()
        {
            _output.PrintArgumentType(typeof(StringBuilder));
        }

        [Test]
        public void NewLineTest()
        {
            _output.NewLine();
            Assert.AreEqual("\r\n", _sbOutput.ToString());
        }

        [RowTest]
        [Row("A short description.", "    A short description.")]
        [Row("The list of directories used for loading assemblies and other dependent resources."
            , "    The list of directories used for loading assemblies and other dependent\r\n    resources.")]
        [Row("A very very long description. A very very long description. A very very long description. A very very long description. A very very long description. A very very long description."
           , "    A very very long description. A very very long description. A very very long\r\n    description. A very very long description. A very very long description. A\r\n    very very long description.")]
        public void PrintDescriptionTest(string description, string expectedOutput)
        {
            _output.PrintDescription(description);
            Assert.AreEqual(expectedOutput, _sbOutput.ToString());
        }

        private enum EnumTypeTest
        {
            Test1,
            Test2,
            Test3
        } ;
    }
}