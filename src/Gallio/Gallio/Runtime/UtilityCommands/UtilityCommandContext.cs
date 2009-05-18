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
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.ConsoleSupport;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// Describes the execution context of a utility command and allows it
    /// to interact with the environment.
    /// </summary>
    public class UtilityCommandContext
    {
        private readonly object arguments;
        private readonly IRichConsole console;
        private readonly ILogger logger;
        private readonly IProgressMonitorProvider progressMonitorProvider;
        private readonly Verbosity verbosity;

        /// <summary>
        /// Creates a utility command context.
        /// </summary>
        /// <param name="arguments">The parsed command line arguments</param>
        /// <param name="console">The console</param>
        /// <param name="logger">The logger</param>
        /// <param name="progressMonitorProvider">The progress monitor provider</param>
        /// <param name="verbosity">The verbosity</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="arguments"/>,
        /// <paramref name="console"/>, <paramref name="logger"/> or <paramref name="progressMonitorProvider"/> is null</exception>
        public UtilityCommandContext(object arguments, IRichConsole console, ILogger logger, IProgressMonitorProvider progressMonitorProvider, Verbosity verbosity)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");
            if (console == null)
                throw new ArgumentNullException("console");
            if (logger == null)
                throw new ArgumentNullException("logger");
            if (progressMonitorProvider == null)
                throw new ArgumentNullException("progressMonitorProvider");

            this.arguments = arguments;
            this.console = console;
            this.logger = logger;
            this.progressMonitorProvider = progressMonitorProvider;
            this.verbosity = verbosity;
        }

        /// <summary>
        /// Gets the parsed command-line arguments.
        /// </summary>
        public object Arguments
        {
            get { return arguments; }

        }

        /// <summary>
        /// Gets the console.
        /// </summary>
        public IRichConsole Console
        {
            get { return console; }
        }

        /// <summary>
        /// Get the logger for writing messages to the console colored and
        /// filtered by severity.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
        }

        /// <summary>
        /// Gets the progress monitor provider for obtaining console progress monitors,
        /// or a null progress monitor provider if progress monitoring has been disabled.
        /// </summary>
        public IProgressMonitorProvider ProgressMonitorProvider
        {
            get { return progressMonitorProvider; }
        }

        /// <summary>
        /// Gets the verbosity requested by the user.
        /// </summary>
        public Verbosity Verbosity
        {
            get { return verbosity; }
        }
    }
}
