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
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common;
using Microsoft.Win32;

namespace Gallio.Common.Platform
{
    /// <summary>
    /// Provides support for working with different versions of the .Net framework.
    /// </summary>
    public static class DotNetFrameworkSupport
    {
        private static Memoizer<DotNetFrameworkVersion> frameworkVersionMemoizer = new Memoizer<DotNetFrameworkVersion>();

        /// <summary>
        /// Gets the .Net framework version installed and currently running.
        /// </summary>
        /// <returns>The framework version.</returns>
        public static DotNetFrameworkVersion FrameworkVersion
        {
            get
            {
                return frameworkVersionMemoizer.Memoize(() =>
                {
                    if (typeof (object).Assembly.GetName().Version.Major == 4)
                        return DotNetFrameworkVersion.DotNet40;

                    try
                    {
                        Assembly.Load("System.Core, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                        return DotNetFrameworkVersion.DotNet35;
                    }
                    catch (FileNotFoundException)
                    {
                        object value = RegistryUtils.GetValueWithBitness(
                            @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.0\Setup",
                            @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\NET Framework Setup\NDP\v3.0\Setup",
                            "InstallSuccess", null);
                        if (value is int && (int)value == 1)
                            return DotNetFrameworkVersion.DotNet30;

                        return DotNetFrameworkVersion.DotNet20;
                    }
                });
            }
        }
    }
}
