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
using System.IO;
using System.Reflection;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Provides utilities for manipulating test runner extensions.
    /// </summary>
    public static class TestRunnerExtensionUtils
    {
        /// <summary>
        /// Creates an extension from its specification.
        /// </summary>
        /// <remarks>
        /// <para>
        /// An extension specification has the form "[Namespace.]Type,Assembly[;Parameters]".
        /// The extension class must implement <see cref="ITestRunnerExtension" />.  The namespace may be omitted
        /// if the assembly contains exactly one class with the specified name.  The parameter string is passed
        /// to the extension exactly as written.
        /// </para>
        /// <para>
        /// Examples:
        /// <list type="bullet">
        /// <item>"MyExtensions.MyExtension,MyExtensions.dll;Parameter1,Parameter2"</item>
        /// <item>"CustomLogger,MyExtensions"</item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="extensionSpecification">The extension specification.</param>
        /// <returns>The extension.</returns>
        /// <exception cref="RunnerException">Thrown if the extension cannot be instantiated and configured.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extensionSpecification"/> is null.</exception>
        public static ITestRunnerExtension CreateExtensionFromSpecification(string extensionSpecification)
        {
            if (extensionSpecification == null)
                throw new ArgumentNullException("extensionSpecification");

            Exception exception;
            try
            {
                string typeName, assemblyName, parameters;
                if (ParseSpecification(extensionSpecification, out typeName, out assemblyName, out parameters))
                {
                    Assembly assembly = LoadAssembly(assemblyName);
                    Type type = GetTypeByName(assembly, typeName);
                    if (type != null)
                    {
                        ITestRunnerExtension extension = (ITestRunnerExtension)Activator.CreateInstance(type);
                        extension.Parameters = parameters;
                        return extension;
                    }
                }

                exception = null;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            throw new RunnerException(String.Format("Could not create a test runner extension from specification '{0}'.", extensionSpecification), exception);
        }

        private static bool ParseSpecification(string extensionSpecification,
            out string typeName, out string assemblyName, out string parameters)
        {
            int parameterPos = extensionSpecification.IndexOf(';');
            string typeAndAssemblyName;
            if (parameterPos < 0)
            {
                parameters = "";
                typeAndAssemblyName = extensionSpecification;
            }
            else
            {
                parameters = extensionSpecification.Substring(parameterPos + 1);
                typeAndAssemblyName = extensionSpecification.Substring(0, parameterPos);
            }

            int assemblyPos = typeAndAssemblyName.IndexOf(',');
            if (assemblyPos > 0 && assemblyPos < typeAndAssemblyName.Length - 1)
            {
                typeName = typeAndAssemblyName.Substring(0, assemblyPos);
                assemblyName = typeAndAssemblyName.Substring(assemblyPos + 1);
                return true;
            }

            typeName = null;
            assemblyName = null;
            return false;
        }

        private static Assembly LoadAssembly(string assemblyNameOrPath)
        {
            if (assemblyNameOrPath.EndsWith(".dll") || assemblyNameOrPath.EndsWith(".exe"))
            {
                // Looks like a path.  First try to resolve the assembly by partial name instead.
                // It could be that the extension was just overspecified such as when referring
                // to Gallio.dll instead of just Gallio, or referring to an extension that is
                // distributed with a plugin.
                try
                {
                    string partialName = Path.GetFileNameWithoutExtension(assemblyNameOrPath);
                    return Assembly.Load(partialName);
                }
                catch (Exception)
                {
                    return Assembly.LoadFrom(assemblyNameOrPath);
                }
            }

            return Assembly.Load(assemblyNameOrPath);
        }

        private static Type GetTypeByName(Assembly assembly, string typeName)
        {
            Type type = assembly.GetType(typeName);
            if (type != null)
                return type;

            foreach (Type candidate in assembly.GetExportedTypes())
            {
                if (candidate.Name == typeName)
                    return candidate;
            }

            return null;
        }
    }
}
