// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// <para>
    /// Applies declarative metadata to a test component.
    /// </para>
    /// <para>
    /// A metadata attribute is similar to a decorator but more restrictive.  Metadata does
    /// not modify the structure of a test directly.  Instead it introduces additional entries
    /// in the <see cref="MetadataMap" /> collection that are useful for classification,
    /// filtering, reporting, documentation or other purposes.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
        | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field
            | AttributeTargets.Parameter, AllowMultiple = true, Inherited = true)]
    public abstract class MetadataPatternAttribute : PatternAttribute
    {
        /// <inheritdoc />
        public override void ProcessTest(IPatternTestBuilder testBuilder, ICodeElementInfo codeElement)
        {
            Apply(testBuilder.Test.Metadata);
        }

        /// <inheritdoc />
        public override void ProcessTestParameter(IPatternTestParameterBuilder testParameterBuilder, ICodeElementInfo codeElement)
        {
            Apply(testParameterBuilder.TestParameter.Metadata);
        }

        /// <summary>
        /// Applies metadata contributions the metadata map of a test component.
        /// </summary>
        /// <param name="metadata">The metadata map</param>
        protected virtual void Apply(MetadataMap metadata)
        {
        }
    }
}