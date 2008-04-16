using System;
using System.Reflection;

namespace Gallio.Runner.Extensions
{
    /// <summary>
    /// Provides utilities for manipulating test runner extensions.
    /// </summary>
    public static class TestRunnerExtensionUtils
    {
        /// <summary>
        /// <para>
        /// Creates an extension from its specification.
        /// </para>
        /// <para>
        /// An extension specification has the form "[ExtensionNamespace.]ExtensionClassName,{ExtensionAssemblyName|ExtensionAssemblyFile}[;ExtensionParameters]".
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
        /// </summary>
        /// <param name="extensionSpecification">The extension specification</param>
        /// <returns>The extension</returns>
        /// <exception cref="RunnerException">Thrown if the extension cannot be instantiated and configured</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="extensionSpecification"/> is null</exception>
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

        private static Assembly LoadAssembly(string assemblyName)
        {
            return Assembly.Load(assemblyName);
        }

        private static Type GetTypeByName(Assembly assembly, string typeName)
        {
            Type type = assembly.GetType(typeName);

            if (type == null)
            {
                foreach (Type candidate in assembly.GetExportedTypes())
                {
                    if (candidate.Name == typeName)
                        return candidate;
                }
            }

            return null;
        }
    }
}
