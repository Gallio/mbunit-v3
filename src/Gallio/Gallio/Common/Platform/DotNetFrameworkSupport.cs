using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common;

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
        /// <returns>The framework version</returns>
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
                        return DotNetFrameworkVersion.DotNet20;
                    }
                });
            }
        }
    }
}
