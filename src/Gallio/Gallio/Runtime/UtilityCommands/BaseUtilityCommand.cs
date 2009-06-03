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

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// Abstract base class for a utility command.
    /// </summary>
    /// <typeparam name="TArguments">The arguments type, may be <see cref="object" />
    /// for commands with no arguments.</typeparam>
    public abstract class BaseUtilityCommand<TArguments> : IUtilityCommand
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="context">The command execution context, not null.</param>
        /// <param name="arguments">The typed command arguments, not null.</param>
        /// <returns>The command exit code, zero for success, non-zero for errors.</returns>
        public abstract int Execute(UtilityCommandContext context, TArguments arguments);

        /// <summary>
        /// Validates the command arguments.
        /// </summary>
        /// <param name="arguments">The arguments object, not null.</param>
        /// <param name="errorReporter">The error reporter, not null.</param>
        /// <returns>True if the arguments are valid.</returns>
        public virtual bool ValidateArguments(TArguments arguments, CommandLineErrorReporter errorReporter)
        {
            return true;
        }

        int IUtilityCommand.Execute(UtilityCommandContext context)
        {
            return Execute(context, (TArguments)context.Arguments);
        }

        Type IUtilityCommand.GetArgumentClass()
        {
            return typeof(TArguments);
        }

        bool IUtilityCommand.ValidateArguments(object arguments, CommandLineErrorReporter errorReporter)
        {
            return ValidateArguments((TArguments)arguments, errorReporter);
        }
    }
}
