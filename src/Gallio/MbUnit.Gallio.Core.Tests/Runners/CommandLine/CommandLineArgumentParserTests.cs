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
using MbUnit.Core.Runner.CommandLine;
using MbUnit2::MbUnit.Framework;

namespace MbUnit.Core.Tests.Runners.CommandLine
{
    [TestFixture]
    public class CommandLineArgumentParserTests
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void OnlyOneDefaultCommandLineArgumentIsAllowed()
        {
            new CommandLineArgumentParser(new MainArgumentsDuplicateDefaultCommandLineArgumentStub().GetType(), null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CommandLineArgumentOnlyWithUniqueShortNameIsAllowed()
        {
            new CommandLineArgumentParser(new MainArgumentsDuplicateShortNameStub().GetType(), null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CommandLineArgumentOnlyWithUniqueLongNameIsAllowed()
        {
            new CommandLineArgumentParser(new MainArgumentsDuplicateLongNameStub().GetType(), null);
        }

        [RowTest]
        [Row("/invalid")]
        [Row("-invalid")]
        public void ParseInvalidArgument(string arg)
        {
            string errorMsg = string.Empty;
            MainArguments arguments = new MainArguments();
            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType(), delegate (string message)
            { errorMsg = message;});
            Assert.AreEqual(false, parser.Parse(new string[] { arg }, arguments));
            Assert.AreEqual(string.Format("Unrecognized command line argument '{0}'", arg), errorMsg);
        }

        [Test]
        public void ParseInvalidValueForBooleanArgument()
        {
            string errorMsg = string.Empty;
            MainArguments arguments = new MainArguments();
            CommandLineArgumentParser parser = new CommandLineArgumentParser(arguments.GetType(), delegate(string message)
            { errorMsg = message;});
            Assert.AreEqual(false, parser.Parse(new string[] { "/help:bad" }, arguments));
            Assert.AreEqual("'bad' is not a valid value for the 'help' command line option", errorMsg);
        }
    }

    public class MainArguments
    {
        [CommandLineArgument(
             CommandLineArgumentType.AtMostOnce,
             ShortName = "h",
             LongName = "help",
             Description = "Display this help text"
             )]
        public bool Help = false;
    }

    public class MainArgumentsDuplicateDefaultCommandLineArgumentStub : MainArguments
    {
        [DefaultCommandLineArgument(
            CommandLineArgumentType.MultipleUnique,
            Description = "default"
            )]
        public string[] Default = null;

        [DefaultCommandLineArgument(
            CommandLineArgumentType.MultipleUnique,
            LongName = "duplicate",
            Description = "duplicated default command line argument"
            )]
        public string[] DuplicateDefault = null;
    }

    public class MainArgumentsDuplicateLongNameStub : MainArguments
    {
        [CommandLineArgument(
            CommandLineArgumentType.MultipleUnique,
            ShortName = "unique1",
            LongName = "dupe",
            Description = "Duplicated long name."
         )]
        public string[] DuplicateLongName = null;

        [CommandLineArgument(
            CommandLineArgumentType.MultipleUnique,
            ShortName = "unique2",
            LongName = "dupe",
            Description = "Duplicated long name."
         )]
        public string[] DuplicateLongName2 = null;
    }

    public class MainArgumentsDuplicateShortNameStub : MainArguments
    {
        [CommandLineArgument(
            CommandLineArgumentType.MultipleUnique,
           ShortName = "dupe",
            LongName = "unique1",
            Description = "Duplicated short name."
         )]
        public string[] DuplicateShortName = null;


        [CommandLineArgument(
            CommandLineArgumentType.MultipleUnique,
           ShortName = "dupe",
            LongName = "unique2",
            Description = "Duplicated short name."
         )]
        public string[] DuplicateShortName2 = null;
    }
}