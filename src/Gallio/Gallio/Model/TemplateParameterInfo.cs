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
using Gallio.Model.Reflection;

namespace Gallio.Model
{
    /// <summary>
    /// A read-only implementation of <see cref="ITemplateParameter" /> for reflection.
    /// </summary>
    public sealed class TemplateParameterInfo : ModelComponentInfo, ITemplateParameter
    {
        /// <summary>
        /// Creates a read-only wrapper of the specified model object.
        /// </summary>
        /// <param name="source">The source model object</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null</exception>
        public TemplateParameterInfo(ITemplateParameter source)
            : base(source)
        {
        }

        /// <inheritdoc />
        public ITypeInfo Type
        {
            get { return Source.Type; }
        }
        ITypeInfo ITemplateParameter.Type
        {
            get { return Type; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        public int Index
        {
            get { return Source.Index; }
        }
        int ITemplateParameter.Index
        {
            get { return Index; }
            set { throw new NotSupportedException(); }
        }

        /// <inheritdoc />
        new internal ITemplateParameter Source
        {
            get { return (ITemplateParameter)base.Source; }
        }
    }
}