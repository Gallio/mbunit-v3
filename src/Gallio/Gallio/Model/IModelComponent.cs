// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Model;
using Gallio.Model.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// Common base-type for components of the test and template object models
    /// that captures shared properties of these models.
    /// All components have a name for presentation, metadata for
    /// annotations, and a code reference to its point of definition. 
    /// </summary>
    public interface IModelComponent
    {
        /// <summary>
        /// Gets or sets the stable unique identifier of the component.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The identifier must be unique across all components
        /// within a given test project.  It should also be stable so that the
        /// identifier remains valid across recompilations and code changes that
        /// do not alter the underlying declarations (insofar as is possible).
        /// </para>
        /// <para>
        /// The identifier does not refer to a specific instance of <see cref="IModelComponent" />,
        /// but rather incorporates enough information so that we can unambiguously find a
        /// corresponding instance in a model that has been populated.  When we rebuild
        /// the model, assuming the code hasn't changed too much, the objects in the model
        /// will have the same identifier as before.  This allows the identifier
        /// to be saved in project files to construct lists of components.  We can also use
        /// it to refer to components remotely.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the component.
        /// </summary>
        /// <remarks>
        /// The name does not need to be globally unique.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        string Name { get; set; }

        /// <summary>
        /// Gets the metadata of the component.
        /// </summary>
        MetadataMap Metadata { get; }

        /// <summary>
        /// Gets or sets a reference to the point of definition of this test
        /// component in the code, or null if unknown.
        /// </summary>
        ICodeElementInfo CodeElement { get; set; }
    }
}
