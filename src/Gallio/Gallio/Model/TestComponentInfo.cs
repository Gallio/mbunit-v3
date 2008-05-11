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
using System.Threading;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// A read-only implementation of <see cref="ITestComponent" /> for reflection.
    /// </summary>
    public abstract class TestComponentInfo : BaseInfo, ITestComponent
    {
        private MetadataMap cachedMetadata;

        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source test component</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        internal TestComponentInfo(ITestComponent source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public string Id
        {
            get { return Source.Id; }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return Source.Name; }
        }

        /// <inheritdoc />
        public MetadataMap Metadata
        {
            get
            {
                if (cachedMetadata == null)
                    Interlocked.CompareExchange(ref cachedMetadata, Source.Metadata.AsReadOnly(), null);
                return cachedMetadata;
            }
        }

        /// <inheritdoc />
        public ICodeElementInfo CodeElement
        {
            get{ return Source.CodeElement; }
        }

        /// <inheritdoc />
        new internal ITestComponent Source
        {
            get { return (ITestComponent)base.Source; }
        }
    }
}