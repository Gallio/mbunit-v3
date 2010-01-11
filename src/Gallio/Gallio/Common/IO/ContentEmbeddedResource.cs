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
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Common.IO
{
    /// <summary>
    /// Represents a resource embedded in an assembly file.
    /// </summary>
    public class ContentEmbeddedResource : Content
    {
        private readonly string name;
        private readonly Type typeScope;

        /// <summary>
        /// Constructs the representation of an embedded resource.
        /// </summary>
        /// <param name="name">The qualified or unqualified name of the resource.</param>
        /// <param name="typeScope">When <paramref name="name"/> is unqualified, a type used to locate the assembly and namespace within which to resolve the manifest resource.</param>
        public ContentEmbeddedResource(string name, Type typeScope)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
            this.typeScope = typeScope;
        }

        /// <inheritdoc />
        public override bool IsDynamic
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public override Stream OpenStream()
        {
            Assembly scopeAssembly;
            string scopeNamespace = GetScope(out scopeAssembly);
            string resourceName = name.Replace('\\', '.');

            Stream stream =
                Swallow(() => scopeAssembly.GetManifestResourceStream(resourceName)) ??
                Swallow(() => scopeNamespace.Length == 0 ? null : scopeAssembly.GetManifestResourceStream(scopeNamespace + "." + resourceName));

            if (stream == null)
            {
                throw new PatternUsageErrorException(String.Format("[{0}] - Could not find manifest resource '{1}'.", GetType().Name, name));
            }

            return stream;
        }

        /// <inheritdoc />
        public override TextReader OpenTextReader()
        {
            return new StreamReader(OpenStream());
        }

        private static Stream Swallow(Func<Stream> func)
        {
            try
            {
                return func();
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        private string GetScope(out Assembly scopeAssembly)
        {
            return typeScope == null
                ? GetScopeFromTypeCodeElementInfo(out scopeAssembly)
                : GetScopeFromType(out scopeAssembly);
        }

        private string GetScopeFromType(out Assembly scopeAssembly)
        {
            scopeAssembly = typeScope.Assembly;

            if (scopeAssembly != null)
            {
                return typeScope.Namespace ?? String.Empty;
            }

            throw new PatternUsageErrorException(String.Format("[{0}] - Could not determine the assembly from which to load the manifest resource.", GetType().Name));
        }

        private string GetScopeFromTypeCodeElementInfo(out Assembly scopeAssembly)
        {
            if (CodeElementInfo != null)
            {
                var assembly = ReflectionUtils.GetAssembly(CodeElementInfo);

                if (assembly != null)
                {
                    INamespaceInfo @namespace = ReflectionUtils.GetNamespace(CodeElementInfo);
                    scopeAssembly = assembly.Resolve(true);
                    return @namespace != null ? @namespace.Name : String.Empty;
                }
            }

            throw new PatternUsageErrorException(String.Format("[{0}] - Could not determine the assembly from which to load the manifest resource.", GetType().Name));
        }
    }
}