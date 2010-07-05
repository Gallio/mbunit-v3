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
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common.IO;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;
using Gallio.Framework;
using Gallio.Model;

namespace MbUnit.Framework
{
    /// <summary>
    /// An abstract base class for data source attributes that obtain contents from
    /// a local file, manifest resource, or inline data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// At most one location type may be used.
    /// </para>
    /// </remarks>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public abstract class ContentAttribute : DataPatternAttribute
    {
        // TODO: Add support for Uris.  We will need to define an IUriLoader service to help
        //       with this.  Such a service would be quite useful for many other reasons also.

        /// <summary>
        /// Gets or sets the inline data contents as a string.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is an error to specify more than one content source property.
        /// </para>
        /// </remarks>
        public string Contents
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of a local file relative to the current working
        /// directory from which the file contents should be read.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is an error to specify more than one content source property.
        /// </para>
        /// </remarks>
        public string FilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a <see cref="Type"/> that is used to locate the assembly and
        /// namespace within which to resolve a manifest resource in combination
        /// with the <see cref="ResourcePath"/> property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no value is specified, the test fixture type is used as the resource scope.
        /// </para>
        /// </remarks>
        /// <seealso cref="ResourcePath"/>
        public Type ResourceScope
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the path of a manifest resource from which the contents should be read.  
        /// </summary>
        /// <remarks>
        /// <para>
        /// The path will be resolved within the assembly containing the
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
        /// <para>
        /// It is an error to specify more than one content source property.
        /// </para>
        /// </remarks>
        /// <seealso cref="ResourceScope"/>
        public string ResourcePath
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the outcome of the test when an error occured 
        /// while opening or reading the data file or the resource.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default outcome is <see cref="MbUnit.Framework.OutcomeOnFileError.Failed"/>.
        /// </para>
        /// </remarks>
        public OutcomeOnFileError OutcomeOnFileError
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ContentAttribute()
        {
            OutcomeOnFileError = OutcomeOnFileError.Failed;
        }

        /// <summary>
        /// Gets the name of the location that is providing the data, or null if none.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The name will be the filename or resource path if specified, or a special
        /// locale-aware string (such as "&lt;inline&gt;") if the contents were specified
        /// inline via the <see cref="Contents"/> property.
        /// </para>
        /// </remarks>
        protected virtual string GetDataLocationName()
        {
            return Contents != null ? "<inline>" : (FilePath ?? ResourcePath);
        }

        /// <summary>
        /// Opens the contents as a stream.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you override this method to return data from a different stream, consider
        /// also overriding <see cref="ValidateSource" /> in case the manner in which the
        /// data source location is specified has also changed.
        /// </para>
        /// </remarks>
        /// <param name="codeElementInfo">The code element to which the attribute was applied.</param>
        /// <returns>The stream.</returns>
        protected virtual Stream OpenStream(ICodeElementInfo codeElementInfo)
        {
            var content = GetContent();
            content.CodeElementInfo = codeElementInfo;
            return content.OpenStream();
        }

        /// <summary>
        /// Opens the contents as a text reader.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If you override this method to return data from a different stream, consider
        /// also overriding <see cref="ValidateSource" /> in case the manner in which the
        /// data source location is specified has also changed.
        /// </para>
        /// </remarks>
        /// <param name="codeElementInfo">The code element to which the attribute was applied.</param>
        /// <returns>The text reader.</returns>
        protected virtual TextReader OpenTextReader(ICodeElementInfo codeElementInfo)
        {
            var content = GetContent();
            content.CodeElementInfo = codeElementInfo;

            try
            {
                return content.OpenTextReader();
            }
            catch (FileNotFoundException exception)
            {
                OnDataError(exception);
                throw;
            }
            catch (UnauthorizedAccessException exception)
            {
                OnDataError(exception);
                throw;
            }
        }

        private void OnDataError(Exception exception)
        {
            switch (OutcomeOnFileError)
            {
                case OutcomeOnFileError.Inconclusive:
                    throw new TestInconclusiveException("An exception occurred while getting data items.", exception);

                case OutcomeOnFileError.Skipped:
                    throw new TestTerminatedException(TestOutcome.Skipped, "An exception occurred while getting data items.", exception);

                default:
                    break;
            }
        }

        /// <summary>
        /// Returns true if the contents are dynamic, or false if they are static.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Static contents can only change if the test assembly is recompiled.
        /// </para>
        /// </remarks>
        protected virtual bool IsDynamic
        {
            get
            {
                return GetContent().IsDynamic;
            }
        }

        /// <inheritdoc />
        protected override void Validate(IPatternScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);
            ValidateSource(scope, codeElement);
        }

        /// <summary>
        /// Validates the data source properties of the content attribute.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Throws a <see cref="PatternUsageErrorException" /> if none of the source
        /// properties, such as <see cref="Contents"/>, <see cref="FilePath" /> or
        /// <see cref="ResourcePath"/> have been set.
        /// </para>
        /// </remarks>
        /// <param name="scope">The pattern scope.</param>
        /// <param name="codeElement">The code element to which the attribute was applied.</param>
        /// <exception cref="PatternUsageErrorException">Thrown if none of the source properties, such as <see cref="Contents"/>, 
        /// <see cref="FilePath" /> or <see cref="ResourcePath"/> have been set.</exception>
        protected virtual void ValidateSource(IPatternScope scope, ICodeElementInfo codeElement)
        {
            if (Contents == null && FilePath == null && ResourcePath == null)
                ThrowUsageErrorException("At least one source property must be specified.");
        }

        private Content GetContent()
        {
            if (Contents != null)
            {
                return new ContentInline(Contents);
            }

            if (FilePath != null)
            {
                return new ContentFile(FilePath);
            }

            return new ContentEmbeddedResource(ResourcePath, ResourceScope);
        }
    }

    /// <summary>
    /// Determines the outcome of data-driven test when an error occured while 
    /// opening or reading the external data file or resource. 
    /// </summary>
    public enum OutcomeOnFileError
    {
        /// <summary>
        /// The test is skipped on file error.
        /// </summary>
        /// <seealso cref="TestOutcome.Skipped"/>
        Skipped,

        /// <summary>
        /// The test is inconclusive on file error.
        /// </summary>
        /// <seealso cref="TestOutcome.Inconclusive"/>
        Inconclusive,

        /// <summary>
        /// The test failed on file error.
        /// </summary>
        /// <seealso cref="TestOutcome.Failed"/>
        /// <seealso cref="TestOutcome.Error"/>
        Failed,
    }
}
