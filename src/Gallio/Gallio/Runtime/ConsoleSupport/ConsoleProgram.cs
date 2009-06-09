// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using Gallio.Properties;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.ConsoleSupport
{
    /// <summary>
    /// A base class for implementing robust console applications.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The subclass should provide a Main method that creates an instance and
    /// calls <see cref="Run(IRichConsole, string[])" />.
    /// </para>
    /// </remarks>
    public abstract class ConsoleProgram<TArguments> : IDisposable
        where TArguments : new()
    {
        private IRichConsole console;
        private TArguments arguments;
        private CommandLineArgumentParser argumentParser;
        private CommandLineOutput commandLineOutput;
        private string applicationName;
        private Version applicationVersion;
        private string applicationTitle;

        /// <inheritdoc />
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Gets the console, or null if the program is not running.
        /// </summary>
        protected IRichConsole Console
        {
            get { return console; }
        }

        /// <summary>
        /// Gets or sets the parsed command-line arguments.
        /// </summary>
        /// <remarks>
        /// <para>
        /// These argument may be modified prior to calling <see cref="ParseArguments" /> to
        /// override the initial argument settings.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        protected TArguments Arguments
        {
            get
            {
                if (arguments == null)
                    arguments = new TArguments();
                return arguments;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                arguments = value;
            }
        }

        /// <summary>
        /// Gets the argument parser.
        /// </summary>
        protected CommandLineArgumentParser ArgumentParser
        {
            get
            {
                if (argumentParser == null)
                    argumentParser = new CommandLineArgumentParser(typeof(TArguments));
                return argumentParser;
            }
        }

        /// <summary>
        /// Gets the command-line output formatter, or null if the program is not running.
        /// </summary>
        protected CommandLineOutput CommandLineOutput
        {
            get
            {
                return commandLineOutput;
            }
        }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        /// <value>The name, by default this is the program assembly's name</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        protected string ApplicationName
        {
            get
            {
                if (applicationName == null)
                    applicationName = GetType().Assembly.GetName().Name;
                return applicationName;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                applicationName = value;
            }
        }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>The version, by default this is the program assembly's version</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        protected Version ApplicationVersion
        {
            get
            {
                if (applicationVersion == null)
                    applicationVersion = AssemblyUtils.GetApplicationVersion(GetType().Assembly);
                return applicationVersion;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                applicationVersion = value;
            }
        }

        /// <summary>
        /// Get or sets the application title.
        /// </summary>
        /// <value>The title, by default this is constructed from the application's name and version</value>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        protected string ApplicationTitle
        {
            get
            {
                if (applicationTitle != null)
                    return applicationTitle;

                return string.Format(Resources.ConsoleProgram_ApplicationTitleFormat, ApplicationName,
                    ApplicationVersion.Major, ApplicationVersion.Minor, ApplicationVersion.Build,
                    ApplicationVersion.Revision);
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                applicationTitle = value;
            }
        }

        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="console">The console.</param>
        /// <param name="args">The command-line arguments.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="console"/>
        /// or <paramref name="args"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the program has already started running.</exception>
        public int Run(IRichConsole console, string[] args)
        {
            if (console == null)
                throw new ArgumentNullException("console");
            if (args == null)
                throw new ArgumentNullException("args");
            if (this.console != null)
                throw new InvalidOperationException("The program has already started running.");

            this.console = console;
            commandLineOutput = new CommandLineOutput(console);

            try
            {
                if (!console.IsRedirected)
                    console.Title = ApplicationTitle;

                return RunImpl(args);
            }
            catch (Exception ex)
            {
                return HandleFatalException(ex);
            }
            finally
            {
                console.ResetColor();
            }
        }

        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>The program exit code.</returns>
        protected abstract int RunImpl(string[] args);

        /// <summary>
        /// Handles a fatal exception that escaped the <see cref="RunImpl" /> method.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>The exit code to return.</returns>
        protected virtual int HandleFatalException(Exception ex)
        {
            ShowErrorMessage(String.Format("A fatal exception occurred.\n{0}", ex));
            return -1;
        }

        /// <summary>
        /// Parses the arguments.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>True if the arguments were parsed successfully.</returns>
        protected virtual bool ParseArguments(string[] args)
        {
            return ArgumentParser.Parse(args, Arguments, ShowErrorMessage);
        }

        /// <summary>
        /// Displays an error message to the console.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected virtual void ShowErrorMessage(string message)
        {
            console.ForegroundColor = ConsoleColor.Red;
            console.WriteLine(String.Format("Error: {0}", message));
            console.ResetColor();
            console.WriteLine();
        }

        /// <summary>
        /// Displays help text to the console.
        /// </summary>
        protected virtual void ShowHelp()
        {
            console.ForegroundColor = ConsoleColor.Yellow;
            console.WriteLine(new string('-', console.Width - 2));
            console.ResetColor();
            console.ForegroundColor = ConsoleColor.White;
            console.Write(@"  ");
            console.WriteLine("Available options:");
            console.ForegroundColor = ConsoleColor.Yellow;
            console.WriteLine(new string('-', console.Width - 2));

            console.ResetColor();
            console.WriteLine();

            ArgumentParser.ShowUsage(CommandLineOutput);
        }
    }
}
