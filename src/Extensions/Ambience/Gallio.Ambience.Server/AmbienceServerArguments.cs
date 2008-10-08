using System;
using Gallio.Ambience.Impl;
using Gallio.Runtime.ConsoleSupport;

namespace Gallio.Ambience.Server
{
    /// <summary>
    /// Command-line arguments for the Ambience server.
    /// </summary>
    public class AmbienceServerArguments
    {
        [CommandLineArgument(
             CommandLineArgumentFlags.AtMostOnce,
             ShortName = "h",
             LongName = "help",
             Description = "Display this help text.",
             Synonyms = new string[] { "?" }
             )]
        public bool Help;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "p",
            LongName = "port",
            Description = "Specifies the server port number.  Default is " + Constants.DefaultPortNumberString + ".",
            ValueLabel = "number"
            )]
        public int Port = Constants.DefaultPortNumber;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "db",
            LongName = "database",
            Description = "Specifies the directory where the database is stored.  Default is in Local Application Data.",
            ValueLabel = "path"
            )]
        public string DatabaseFolder;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "u",
            LongName = "username",
            Description = "Specifies the username that clients must authenticate with.  Default is '" + Constants.AnonymousUserName + "'.",
            ValueLabel = "username"
            )]
        public string UserName = Constants.AnonymousUserName;

        [CommandLineArgument(
            CommandLineArgumentFlags.AtMostOnce,
            ShortName = "pw",
            LongName = "password",
            Description = "Specifies the username that clients must authenticate with.  Default is '" + Constants.AnonymousPassword + "'.",
            ValueLabel = "password"
            )]
        public string Password = Constants.AnonymousPassword;
    }
}
