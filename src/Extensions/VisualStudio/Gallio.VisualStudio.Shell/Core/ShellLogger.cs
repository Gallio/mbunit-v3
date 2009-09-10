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
using System.Linq;
using System.Text;
using Gallio.Runtime.Logging;
using Gallio.Common.Diagnostics;
using Microsoft.VisualStudio.Shell.Interop;

namespace Gallio.VisualStudio.Shell.Core
{
    /// <summary>
    /// Logs to the Visual Studio Output Window.
    /// </summary>
    public class ShellLogger : BaseLogger
    {
        private static readonly Guid OutputPaneGuid = new Guid("DEC10AAA-A2E0-4F0E-9D01-85AE8A88A71E");

        private readonly ShellPackage shellPackage;

        /// <summary>
        /// Creates a shell logger.
        /// </summary>
        /// <param name="shellPackage">The shell package.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="shellPackage"/> is null.</exception>
        public ShellLogger(ShellPackage shellPackage)
        {
            if (shellPackage == null)
                throw new ArgumentNullException("shellPackage");

            this.shellPackage = shellPackage;
        }

        /// <inheritdoc />
        protected override void LogImpl(LogSeverity severity, string message, ExceptionData exceptionData)
        {
            if (severity < LogSeverity.Info)
                return;

            StringBuilder output = new StringBuilder();
            output.AppendLine(message);
            if (exceptionData != null)
            {
                output.Append("  ");
                output.AppendLine(exceptionData.ToString());
            }

            IVsOutputWindowPane outputWindowPane = GetOutputWindowPane();
            outputWindowPane.OutputStringThreadSafe(output.ToString());
        }

        private IVsOutputWindowPane GetOutputWindowPane()
        {
            return shellPackage.GetOutputPane(OutputPaneGuid, "Gallio");
        }
    }
}
