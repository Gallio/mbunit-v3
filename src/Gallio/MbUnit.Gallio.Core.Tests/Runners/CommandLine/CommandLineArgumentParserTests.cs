extern alias MbUnit2;
using System;
using MbUnit.Core.Runner.CommandLine;
using MbUnit.Echo;
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

    public class MainArgumentsDuplicateDefaultCommandLineArgumentStub : MainArguments
    {
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
            ShortName = "unique",
          LongName = "help",
           Description = "Duplicated long name."
         )]
        public string[] DuplicateLongName = null;
    }

    public class MainArgumentsDuplicateShortNameStub : MainArguments
    {
        [CommandLineArgument(
            CommandLineArgumentType.MultipleUnique,
           ShortName = "ap",
            LongName = "long name",
            Description = "Duplicated short name."
         )]
        public string[] DuplicateShortName = null;
    }
}