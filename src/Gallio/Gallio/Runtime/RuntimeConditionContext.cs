// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common;
using Gallio.Common.Platform;
using System.Diagnostics;
using System.IO;
using System.Globalization;

namespace Gallio.Runtime
{
    /// <summary>
    /// A condition context that is used to enable or disable plugins,
    /// components and services based on characteristics of the runtime environment.
    /// </summary>
    /// <remarks>
    /// This condition context recognizes the following property tokens:
    /// <list type="bullet">
    /// <item>"${env:ENVIRONMENTVARIABLE}": Satisfied when the environment contains
    /// a variable called "ENVIRONMENTVARIABLE".</item>
    /// <item>"${minFramework:NET20}", "${minFramework:NET30}", "${minFramework:NET35}", "${minFramework:NET40}":
    /// Satisfied when the currently running .Net runtime version is at least the specified version.</item>
    /// <item>"${process:PROC.EXE}", "${process:PROC.EXE_V1}", "${process:PROC.EXE_V1.2}",
    /// "${process:PROC.EXE_V1.2.3}", "${process:PROC.EXE_V1.2.3.4}": Satisfied when the currently
    /// running process main module is "PROC.EXE" and exactly matches the specified file version
    /// components (if any).</item>
    /// </list>
    /// </remarks>
    public class RuntimeConditionContext : ConditionContext
    {
        /// <inheritdoc />
        protected override bool HasPropertyImpl(string @namespace, string identifier)
        {
            if (string.Compare("env", @namespace, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return ! string.IsNullOrEmpty(Environment.GetEnvironmentVariable(identifier));
            }

            if (string.Compare("minFramework", @namespace, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                switch (identifier.ToUpperInvariant())
                {
                    case "NET40":
                        return DotNetFrameworkSupport.FrameworkVersion >= DotNetFrameworkVersion.DotNet40;

                    case "NET35":
                        return DotNetFrameworkSupport.FrameworkVersion >= DotNetFrameworkVersion.DotNet35;

                    case "NET30":
                        return DotNetFrameworkSupport.FrameworkVersion >= DotNetFrameworkVersion.DotNet30;

                    case "NET20":
                        return DotNetFrameworkSupport.FrameworkVersion >= DotNetFrameworkVersion.DotNet20;

                    default:
                        return false;
                }
            }

            if (string.Compare("framework", @namespace, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                switch (identifier.ToUpperInvariant())
                {
                    case "NET40":
                        return DotNetFrameworkSupport.FrameworkVersion == DotNetFrameworkVersion.DotNet40;

                    case "NET35":
                        return DotNetFrameworkSupport.FrameworkVersion == DotNetFrameworkVersion.DotNet35;

                    case "NET30":
                        return DotNetFrameworkSupport.FrameworkVersion == DotNetFrameworkVersion.DotNet30;

                    case "NET20":
                        return DotNetFrameworkSupport.FrameworkVersion == DotNetFrameworkVersion.DotNet20;

                    default:
                        return false;
                }
            }

            if (string.Compare("process", @namespace, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                ProcessModule module = Process.GetCurrentProcess().MainModule;
                string moduleName = Path.GetFileName(module.FileName).ToUpperInvariant();
                FileVersionInfo moduleVersionInfo = module.FileVersionInfo;

                identifier = identifier.ToUpperInvariant();

                if (identifier == moduleName)
                    return true;

                if (identifier == string.Format(CultureInfo.InvariantCulture,
                    "{0}_V{1}", moduleName, moduleVersionInfo.FileMajorPart))
                    return true;

                if (identifier == string.Format(CultureInfo.InvariantCulture,
                    "{0}_V{1}.{2}", moduleName, moduleVersionInfo.FileMajorPart, moduleVersionInfo.FileMinorPart))
                    return true;

                if (identifier == string.Format(CultureInfo.InvariantCulture,
                    "{0}_V{1}.{2}.{3}", moduleName, moduleVersionInfo.FileMajorPart, moduleVersionInfo.FileMinorPart,
                    moduleVersionInfo.FileBuildPart))
                    return true;

                if (identifier == string.Format(CultureInfo.InvariantCulture,
                    "{0}_V{1}.{2}.{3}.{4}", moduleName, moduleVersionInfo.FileMajorPart, moduleVersionInfo.FileMinorPart,
                    moduleVersionInfo.FileBuildPart, moduleVersionInfo.FilePrivatePart))
                    return true;
            }

            return false;
        }
    }
}
