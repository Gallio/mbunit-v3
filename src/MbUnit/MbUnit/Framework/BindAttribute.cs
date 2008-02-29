// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using Gallio.Framework.Data;
using Gallio.Framework.Data.Binders;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// The bind attribute overrides the default binding rules for a test parameter
    /// by specifying a different data source, a binding path or an index.  At most
    /// one such attribute may appear on any given test parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false, Inherited=true)]
    public class BindAttribute : TestParameterDecoratorPatternAttribute
    {
        private string source;
        private readonly string path;
        private readonly int? index;

        /// <summary>
        /// Sets the binding path for the associated test parameter.
        /// </summary>
        /// <param name="path">The binding path</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> is null</exception>
        public BindAttribute(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            this.path = path;
        }

        /// <summary>
        /// Sets the binding index for the associated test parameter.
        /// </summary>
        /// <param name="index">The binding index</param>
        public BindAttribute(int index)
        {
            this.index = index;
        }

        /// <summary>
        /// <para>
        /// Gets or sets the name of the data source to bind, or null to bind
        /// the default data source for the test parameter.
        /// </para>
        /// <para>
        /// The default source for a test parameter is the anonymous data source defined
        /// within the scope of the test parameter or by its enclosing test.
        /// </para>
        /// </summary>
        public string Source
        {
            get { return source; }
            set { source = value; }
        }

        /// <summary>
        /// Gets the binding path, or null if none.
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        /// <summary>
        /// Gets the binding index, or null if none.
        /// </summary>
        public int? Index
        {
            get { return index; }
        }

        /// <inheritdoc />
        protected override void DecorateTestParameter(IPatternTestParameterBuilder builder, ISlotInfo slot)
        {
            DataBinding binding = new SimpleDataBinding(index, path);
            builder.TestParameter.Binder = new ScalarDataBinder(binding, source ?? @"");
        }
    }
}
