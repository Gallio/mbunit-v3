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
    /// Abstract representation of a binary or text resource.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Different kind of resources are supported:
    /// <list type="bullet">
    /// <item>Inline resources from a storage in memory.</item>
    /// <item>Embedded resources from an assembly manifest.</item>
    /// <item>File resources.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public abstract class Content
    {
        /// <summary>
        /// Returns true if the contents are dynamic, or false if they are static.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Static contents can only change if the test assembly is recompiled.
        /// </para>
        /// </remarks>
        public abstract bool IsDynamic
        {
            get;
        }

        /// <summary>
        /// Gets or sets a <see cref="ICodeElementInfo"/> that is used to locate the assembly and namespace within which to resolve a manifest resource.
        /// </summary>
        public ICodeElementInfo CodeElementInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Opens the current resource as a <see cref="Stream"/>.
        /// </summary>
        /// <returns>The stream.</returns>
        public abstract Stream OpenStream();

        /// <summary>
        /// Opens the current resource as a <see cref="TextReader"/>.
        /// </summary>
        /// <returns>The text reader.</returns>
        public abstract TextReader OpenTextReader();
    }
}
