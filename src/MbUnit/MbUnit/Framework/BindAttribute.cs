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
using Gallio.Framework.Data;
using Gallio.Reflection;
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// The bind attribute overrides the default binding rules for a test parameter
    /// by specifying a different data source, a binding path or an index.  At most
    /// one such attribute may appear on any given test parameter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The bind attribute supports more complex data binding binding scenarios.
    /// For example, in a typical use of <see cref="CsvDataAttribute" />, the binding
    /// of test parameters to CSV columns is performed by name and by position.
    /// This assumes that the CSV file either contains a header with columns that
    /// are named the same as the test parameter or that the columns appear in the
    /// same order as the test parameters.  Should this not be the case, we may
    /// apply the <see cref="BindAttribute" /> to the test parameter to specify
    /// an explicit column name (by setting the path) or column position (by
    /// setting the index).
    /// </para>
    /// </remarks>
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
        protected override void DecorateTestParameter(PatternEvaluationScope slotScope, ISlotInfo slot)
        {
            DataBinding binding = new DataBinding(index, path);
            slotScope.TestParameter.Binder = new ScalarDataBinder(binding, source ?? @"");
        }
    }
}
