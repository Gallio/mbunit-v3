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
using MbUnit.Core.IO.CommandLine;
using MbUnit.Core.IO;
using MbUnit2::MbUnit.Framework;
using Rhino.Mocks;

namespace MbUnit.Tests.Core.IO.CommandLine
{
    [TestFixture]
    public class CommandLineArgumentParserTests
    {
        private MockRepository _mocks;
        IFileSystem _resourceFileMock;

        [SetUp]
        public void TestStart()
        {
            _mocks = new MockRepository();
            _resourceFileMock = _mocks.CreateMock<IFileSystem>();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OnlyOneDefaultCommandLineArgumentIsAllowed()
        {
            new CommandLineArgumentParser(new MainArgumentsDuplicateDefaultCommandLineArgumentStub().GetType());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CommandLineArgumentOnlyWithUniqueShortNameIsAllowed()
        {
            new CommandLineArgumentParser(new MainArgumentsDuplicateShortNameStub().GetType());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CommandLineArgumentOnlyWithUniqueLongNameIsAllowed()
        {
            new CommandLineArgumentParser(new MainArgumentsDuplicateLongNameStub().GetType());
        }

        [RowTest]
        [Row("/invalid")]
        [Row("-invalid")]
        public void ParseInvalidArgument(string arg)
        {
            string errorMsg = string.Empty;
            MainArguments arguments = new MainArguments();
            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType());
            
            Assert.AreEqual(false, parser.Parse(new string[] { arg }, arguments,
                delegate(string message) { errorMsg = message; }));
            Assert.AreEqual(string.Format("Unrecognized argument '{0}'.", arg), errorMsg);
        }

        [Test]
        public void ParseInvalidValueForBooleanArgument()
        {
            string errorMsg = string.Empty;
            MainArguments arguments = new MainArguments();
            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType());

            Assert.AreEqual(false, parser.Parse(new string[] { "/help:bad" }, arguments,
                delegate(string message) { errorMsg = message; }));
            Assert.AreEqual("Invalid 'help' argument value 'bad'.", errorMsg);
        }

        [RowTest]
        [Row("/help", "/help")]
        [Row("/help+", "/help")]
        public void ParseDuplicatedArgument(string arg1, string arg2)
        {
            string errorMsg = string.Empty;
            MainArguments arguments = new MainArguments();
            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType());

            Assert.AreEqual(false, parser.Parse(new string[] { arg1, arg2 }, arguments,
                delegate(string message) { errorMsg = message; }));
            Assert.AreEqual("Duplicate 'help' argument.", errorMsg);
        }

        [Test]
        public void ParseResourceFile_InvalidFileTest()
        {
            string errorMsg = string.Empty;
            MainArguments arguments = new MainArguments();
            Expect.Call(_resourceFileMock.ReadAllText("InvalidFile")).Throw(new FileNotFoundException());
            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType(), _resourceFileMock);
            _mocks.ReplayAll();
            Assert.AreEqual(false, parser.Parse(new string[] { "@InvalidFile" }, arguments,
                delegate(string message) { errorMsg = message; }));
            _mocks.VerifyAll();
            Assert.Contains(errorMsg, "Response file '0' does not exist.");
        }

        [Test]
        public void ParseEmptyResourceFile()
        {
            string errorMsg = string.Empty;
            MainArguments arguments = new MainArguments();
            Expect.Call(_resourceFileMock.ReadAllText("EmptyResourceFile")).Return("");
            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType(), _resourceFileMock);
            _mocks.ReplayAll();
            Assert.AreEqual(true, parser.Parse(new string[] { "@EmptyResourceFile" }, arguments,
                delegate(string message) { errorMsg = message; }));
            _mocks.VerifyAll();
            Console.WriteLine(errorMsg);
        }

        [Test]
        public void ParseResourceFile()
        {
            string errorMsg = string.Empty;
            string fileContent = "C:\\file.dll";
            MainArguments arguments = new MainArguments();
            Expect.Call(_resourceFileMock.ReadAllText("ResourceFile")).Return(fileContent);
            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType(), _resourceFileMock);
            _mocks.ReplayAll();
            Assert.AreEqual(true, parser.Parse(new string[] { "@ResourceFile" },
                arguments, delegate(string message) { errorMsg = message; }));
            _mocks.VerifyAll();
            Console.WriteLine(errorMsg);
        }
    }

    public class MainArguments
    {
        [DefaultCommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            LongName = "assemblies",
            Description = "List of assemblies containing the tests."
            )]
        public string[] Assemblies;

        [CommandLineArgument(
         CommandLineArgumentFlags.AtMostOnce,
         ShortName = "h",
         LongName = "help",
         Description = "Display this help text"
         )]
        public bool Help = false;

        [CommandLineArgument(
        CommandLineArgumentFlags.MultipleUnique,
        ShortName = "hd",
        LongName = "hint-directories",
        Description = "The list of directories used for loading assemblies and other dependent resources."
        )]
        public string[] HintDirectories;
    }

    public class MainArgumentsDuplicateDefaultCommandLineArgumentStub : MainArguments
    {
        [DefaultCommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            LongName = "duplicate",
            Description = "duplicated default command line argument"
            )]
        public string[] DuplicateDefault = null;
    }

    public class MainArgumentsDuplicateLongNameStub : MainArguments
    {
        [CommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
            ShortName = "unique",
           LongName = "help",
           Description = "Duplicated long name."
         )]
        public string[] DuplicateLongName = null;
    }

    public class MainArgumentsDuplicateShortNameStub : MainArguments
    {
        [CommandLineArgument(
            CommandLineArgumentFlags.MultipleUnique,
           ShortName = "h",
            LongName = "long name",
            Description = "Duplicated short name."
         )]
        public string[] DuplicateShortName = null;
    }
}