// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Text;
using Gallio.Framework.Pattern;
using Gallio.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// An abstract base class for data source attributes that obtain contents from
    /// a local file, manifest resource, or inline data.  At most one location type
    /// may be used.
    /// </para>
    /// </summary>
    /// <todo author="jeff">
    /// Add support for Uris.  We will need to define an IUriLoader service to help
    /// with this.  Such a service would be quite useful for many other reasons also.
    /// </todo>
    public abstract class ContentAttribute : DataPatternAttribute
    {
        private string contents;
        private string filePath;
        private Type resourceScope;
        private string resourcePath;

        /// <summary>
        /// <para>
        /// Gets or sets the inline data contents as a string.
        /// </para>
        /// </summary>
        /// <remarks>
        /// It is an error to specify more than one content source property.
        /// </remarks>
        public string Contents
        {
            get { return contents; }
            set { contents = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the path of a local file relative to the current working
        /// directory from which the file contents should be read.
        /// </para>
        /// </summary>
        /// <remarks>
        /// It is an error to specify more than one content source property.
        /// </remarks>
        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets a <see cref="Type"/> that is used to locate the assembly and
        /// namespace within which to resolve a manifest resource in combination
        /// with the <see cref="ResourcePath"/> property.
        /// </para>
        /// <para>
        /// If no value is specified, the test fixture type is used as the resource scope.
        /// </para>
        /// </summary>
        /// <seealso cref="ResourcePath"/>
        public Type ResourceScope
        {
            get { return resourceScope; }
            set { resourceScope = value; }
        }

        /// <summary>
        /// <para>
        /// Gets or sets the path of a manifest resource from which the contents
        /// should be read.  The path will be resolved within the assembly containing the
        /// <see cref="ResourceScope"/> type or the test fixture type if none if provided.
        /// </para>
        /// <para>
        /// During resolution, a resource name is constructed from the resource path by
        /// translating backslashes to periods.  If the named resource is found within
        /// the scoped assembly manifest, it is used.  Otherwise, the name is prepended
        /// with the scoped type's namespace and second lookup is attempted.  If this
        /// final attempt fails, then an error is raised at runtime.
        /// </para>
        /// <para>
        /// Examples:
        /// <list type="bullet">
        /// <item>If the <see cref="ResourceScope" /> is <c>MyNamespace.MyType</c> within
        /// assembly <c>MyAssembly.dll</c> and if <see cref="ResourcePath" /> is <c>"Resources\Image.gif"</c>, then
        /// resolution will first check whether <c>Resources.Image.gif</c> in
        /// <c>MyAssembly.dll</c> is a valid resource.  If not found, it will consider
        /// <c>MyNamespace.Resources.Image.gif</c>.  If still not found, then a runtime error will be raised.</item>
        /// <item>If no <see cref="ResourceScope" /> is provided, then the containing test fixture type
        /// will be used as the resource scope.  The above resolution strategy still applies.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <remarks>
        /// It is an error to specify more than one content source property.
        /// </remarks>
        /// <seealso cref="ResourceScope"/>
        public string ResourcePath
        {
            get { return resourcePath; }
            set { resourcePath = value; }
        }

        /// <summary>
        /// <para>
        /// Gets the name of the location that is providing the data, or null if none.
        /// </para>
        /// <para>
        /// The name will be the filename or resource path if specified, or a special
        /// locale-aware string (such as "&lt;inline&gt;") if the contents were specified
        /// inline via the <see cref="Contents"/> property.
        /// </para>
        /// </summary>
        protected virtual string GetDataLocationName()
        {
            if (contents != null)
                return "<inline>";

            if (filePath != null)
                return filePath;

            return resourcePath;
        }

        /// <summary>
        /// Opens the contents as a stream.
        /// </summary>
        /// <param name="codeElement">The code element to which the attribute was applied</param>
        /// <returns>The stream</returns>
        protected virtual Stream OpenStream(ICodeElementInfo codeElement)
        {
            if (contents != null)
                return new MemoryStream(Encoding.UTF8.GetBytes(contents));

            if (filePath != null)
                return File.OpenRead(filePath);

            return OpenResourceStream(codeElement);
        }

        /// <summary>
        /// Opens the contents as a text reader.
        /// </summary>
        /// <param name="codeElement">The code element to which the attribute was applied</param>
        /// <returns>The text reader</returns>
        protected virtual TextReader OpenTextReader(ICodeElementInfo codeElement)
        {
            if (contents != null)
                return new StringReader(contents);

            return new StreamReader(OpenStream(codeElement));
        }

        /// <summary>
        /// Returns true if the contents are dynamic, or false if they are static.
        /// Static contents can only change if the test assembly is recompiled.
        /// </summary>
        protected virtual bool IsDynamic
        {
            get
            {
                return filePath != null;
            }
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (contents == null && filePath == null && resourcePath == null)
                ThrowUsageErrorException("At least one source property must be specified.");
        }

        private Stream OpenResourceStream(ICodeElementInfo codeElement)
        {
            Assembly scopeAssembly;
            string scopeNamespace;
            GetResourceScope(codeElement, out scopeAssembly, out scopeNamespace);

            string resourceName = resourcePath.Replace('\\', '.');

            try
            {
                Stream stream = scopeAssembly.GetManifestResourceStream(resourceName);
                if (stream != null)
                    return stream;
            }
            catch (FileNotFoundException)
            {
            }

            try
            {
                if (scopeNamespace.Length != 0)
                {
                    Stream stream = scopeAssembly.GetManifestResourceStream(scopeNamespace + @"." + resourceName);
                    if (stream != null)
                        return stream;
                }
            }
            catch (FileNotFoundException)
            {
            }

            ThrowUsageErrorException(String.Format("Could not find manifest resource '{0}'.", resourcePath));
            return null; // unreachable
        }

        private void GetResourceScope(ICodeElementInfo codeElement, out Assembly scopeAssembly, out string scopeNamespace)
        {
            if (resourceScope == null)
            {
                IAssemblyInfo assembly = ReflectionUtils.GetAssembly(codeElement);
                if (assembly != null)
                {
                    INamespaceInfo @namespace = ReflectionUtils.GetNamespace(codeElement);
                    scopeAssembly = assembly.Resolve(true);
                    scopeNamespace = @namespace != null ? @namespace.Name : "";
                    return;
                }
            }
            else
            {
                scopeAssembly = resourceScope.Assembly;
                if (scopeAssembly != null)
                {
                    scopeNamespace = resourceScope.Namespace ?? "";
                    return;
                }
            }

            ThrowUsageErrorException("Could not determine the assembly from which to load the manifest resource.");
            scopeAssembly = null; // unreachable
            scopeNamespace = null;
        }
    }
}
