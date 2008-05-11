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

using System;
using System.IO;
using Gallio.Runtime.ConsoleSupport;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.ConsoleSupport
{
    [TestFixture]
    public class CommandLineArgumentParserTests
    {
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

        [Test]
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

        [Test]
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

            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType(),
                delegate(string responseFileName)
                {
                    Assert.AreEqual("InvalidFile", responseFileName);
                    throw new FileNotFoundException();
                });

            Assert.AreEqual(false, parser.Parse(new string[] { "@InvalidFile" }, arguments,
                delegate(string message) { errorMsg = message; }));

            Assert.Contains(errorMsg, "Response file '0' does not exist.");
        }

        [Test]
        public void ParseEmptyResourceFile()
        {
            string errorMsg = string.Empty;
            MainArguments arguments = new MainArguments();

            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType(),
                delegate(string responseFileName)
                {
                    Assert.AreEqual("EmptyResourceFile", responseFileName);
                    return "";
                });

            Assert.AreEqual(true, parser.Parse(new string[] { "@EmptyResourceFile" }, arguments,
                delegate(string message) { errorMsg = message; }));
            Console.WriteLine(errorMsg);
        }

        [Test]
        public void ParseResourceFile()
        {
            string errorMsg = string.Empty;
            string fileContent = "C:\\file.dll";
            MainArguments arguments = new MainArguments();

            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType(),
                delegate(string responseFileName)
                {
                    Assert.AreEqual("ResourceFile", responseFileName);
                    return fileContent;
                });

            Assert.AreEqual(true, parser.Parse(new string[] { "@ResourceFile" },
                arguments, delegate(string message) { errorMsg = message; }));
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