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
using Gallio.Model;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// A test component builder applies contributions to a test or test parameter under construction.
    /// </summary>
    public interface ITestComponentBuilder : ISupportDeferredActions
    {
        /// <summary>
        /// Gets the stable unique identifier of the component.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The identifier must be unique across all components
        /// within a given test project.  It should also be stable so that the
        /// identifier remains valid across recompilations and code changes that
        /// do not alter the underlying declarations (insofar as is possible).
        /// </para>
        /// <para>
        /// The identifier does not refer to a specific instance of <see cref="TestComponent" />,
        /// but rather incorporates enough information so that we can unambiguously find a
        /// corresponding instance in a model that has been populated.  When we rebuild
        /// the model, assuming the code hasn't changed too much, the objects in the model
        /// will have the same identifier as before.  This allows the identifier
        /// to be saved in project files to construct lists of components.  We can also use
        /// it to refer to components remotely.
        /// </para>
        /// </remarks>
        string Id { get; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        /// <seealso cref="TestComponent.Name"/>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        string Name { get; set; }

        /// <summary>
        /// Gets a reference to the point of definition of this test
        /// component in the code, or null if unknown.
        /// </summary>
        ICodeElementInfo CodeElement { get; }

        /// <summary>
        /// Adds a metadata key value pair.
        /// </summary>
        /// <param name="key">The metadata key.</param>
        /// <param name="value">The value to add.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or <paramref name="value"/> is null.</exception>
        /// <seealso cref="TestComponent.Metadata"/>
        /// <seealso cref="MetadataKeys"/>
        void AddMetadata(string key, string value);

        /// <summary>
        /// Gets the underlying test component.
        /// </summary>
        /// <returns>The underlying test component.</returns>
        IPatternTestComponent ToTestComponent();
    }
}
