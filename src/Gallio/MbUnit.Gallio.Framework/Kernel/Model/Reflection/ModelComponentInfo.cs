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
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model.Reflection
{
    /// <summary>
    /// A read-only implementation of <see cref="IModelComponent" /> for reflection.
    /// </summary>
    public abstract class ModelComponentInfo : BaseInfo, IModelComponent
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        internal ModelComponentInfo(IModelComponent source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public string Id
        {
            get { return Source.Id; }
        }
        string IModelComponent.Id
        {
            get { return Id; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public string Name
        {
            get { return Source.Name; }
        }
        string IModelComponent.Name
        {
            get { return Name; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public MetadataMap Metadata
        {
            get { return Source.Metadata.Copy(); }
        }

        /// <inheritdoc />
        public CodeReference CodeReference
        {
            get { return Source.CodeReference.Copy(); }
        }
        CodeReference IModelComponent.CodeReference
        {
            get { return CodeReference; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        new internal IModelComponent Source
        {
            get { return (IModelComponent)base.Source; }
        }
    }
}