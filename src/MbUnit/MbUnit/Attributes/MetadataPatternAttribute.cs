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
using Gallio.Model;
using MbUnit.Model;

namespace MbUnit.Attributes
{
    /// <summary>
    /// Applies declarative metadata to an MbUnit template model object.  A metadata attribute is
    /// similar to a decorator but more restrictive.  Metadata does not modify the structure
    /// of a model object directly.  Instead it introduces additional properties that are
    /// useful for classification, filtering, reporting, documentation or other purposes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
        | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field
        | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataPatternAttribute : PatternAttribute
    {
        /// <summary>
        /// Applies metadata contributions to the specified component.
        /// </summary>
        /// <param name="builder">The builder</param>
        /// <param name="component">The component to which metadata should be applied</param>
        public virtual void Apply(MbUnitTestBuilder builder, ITemplateComponent component)
        {
        }
    }
}
