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
using System.Reflection;
using System.Text.RegularExpressions;

namespace Gallio.Common.Reflection
{
    /// <summary>
    /// An assembly binding specifies the name and location of an assembly as well
    /// as some properties to control how the .Net assembly loader resolves
    /// particular assembly versions.
    /// </summary>
    [Serializable]
    public class AssemblyBinding
    {
        private readonly AssemblyName assemblyName;
        private readonly List<BindingRedirect> bindingRedirects;
        private Uri codeBase;

        private AssemblyBinding()
        {
            bindingRedirects = new List<BindingRedirect>();
            ApplyPublisherPolicy = true;
        }

        /// <summary>
        /// Creates an assembly binding with the specified assembly name.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyName"/> is null.</exception>
        public AssemblyBinding(AssemblyName assemblyName)
            : this()
        {
            if (assemblyName == null)
                throw new ArgumentNullException("assemblyName");

            this.assemblyName = assemblyName;
        }

        /// <summary>
        /// Creates an assembly binding using the name and codebase of the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null.</exception>
        public AssemblyBinding(Assembly assembly)
            : this()
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            assemblyName = assembly.GetName();
            codeBase = new Uri(assembly.CodeBase);
        }

        /// <summary>
        /// Gets the assembly name.
        /// </summary>
        public AssemblyName AssemblyName
        {
            get { return assemblyName; }
        }

        /// <summary>
        /// Gets or sets the assembly codebase, or null if unknown.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="value"/>
        /// is not null and is not an absolute Uri.</exception>
        public Uri CodeBase
        {
            get { return codeBase; }
            set
            {
                if (value != null && ! value.IsAbsoluteUri)
                    throw new ArgumentException("CodeBase must be an absolute Uri.", "value");
                codeBase = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the assembly full name should be used to qualify all partial
        /// name references to the assembly.
        /// </summary>
        public bool QualifyPartialName { get; set; }

        /// <summary>
        /// Gets or sets whether to apply the assembly publisher policy.  Default is <c>true</c>.
        /// </summary>
        public bool ApplyPublisherPolicy { get; set; }

        /// <summary>
        /// Gets the mutable list of assembly binding redirects.
        /// </summary>
        public IList<BindingRedirect> BindingRedirects
        {
            get { return bindingRedirects; }
        }

        /// <summary>
        /// Adds a binding redirect.
        /// </summary>
        /// <param name="bindingRedirect">The binding redirect to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="bindingRedirect"/> is null.</exception>
        public void AddBindingRedirect(BindingRedirect bindingRedirect)
        {
            if (bindingRedirect == null)
                throw new ArgumentNullException("bindingRedirect");

            bindingRedirects.Add(bindingRedirect);
        }

        /// <summary>
        /// Specifies an old version range for an assembly binding redirect.
        /// </summary>
        [Serializable]
        public sealed class BindingRedirect
        {
            private static readonly Regex OldVersionRegex = new Regex(@"^\d+\.\d+\.\d+\.\d+(?:-\d+\.\d+\.\d+\.\d+)?$",
                RegexOptions.CultureInvariant | RegexOptions.Singleline);
            private readonly string oldVersion;

            /// <summary>
            /// Creates an assembly binding redirect.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Accepts a version number like "1.2.3.4" or a range like "1.0.0.0-1.1.65535.65535".
            /// </para>
            /// </remarks>
            /// <param name="oldVersion">The old assembly version number or range.</param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="oldVersion"/> is null.</exception>
            /// <exception cref="ArgumentException">Thrown if <paramref name="oldVersion"/> is not a valid version or version range.</exception>
            public BindingRedirect(string oldVersion)
            {
                if (oldVersion == null)
                    throw new ArgumentNullException("oldVersion");
                if (!OldVersionRegex.IsMatch(oldVersion))
                    throw new ArgumentException("Old version must be a version number like ;1.2.3.4' or a range like '1.0.0.0-1.1.65535.65535'.", "oldVersion");

                this.oldVersion = oldVersion;
            }

            /// <summary>
            /// Gets the old assembly version number or range.
            /// </summary>
            /// <remarks>
            /// <para>
            /// Accepts a version number like "1.2.3.4" or a range like "1.0.0.0-1.1.65535.65535".
            /// </para>
            /// </remarks>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
            public string OldVersion
            {
                get { return oldVersion; }
            }
        }
    }
}