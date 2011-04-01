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
using Gallio.Framework.Data;
using Gallio.Common.Reflection;
using Gallio.Model.Tree;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// Represents an parameter of a <see cref="PatternTest" /> derived from a field, property
    /// or method parameter.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTestParameter : TestParameter, IPatternTestComponent
    {
        private readonly PatternTestDataContext dataContext;
        private readonly PatternTestParameterActions testParameterActions;
        private IDataBinder binder;

        /// <summary>
        /// Creates a test pattern parameter.
        /// </summary>
        /// <param name="name">The name of the test parameter.</param>
        /// <param name="codeElement">The code element (usually a slot) represented by the parameter, or null if none.</param>
        /// <param name="dataContext">The data context of the test parameter.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>
        /// or <paramref name="dataContext"/> is null.</exception>
        public PatternTestParameter(string name, ICodeElementInfo codeElement, PatternTestDataContext dataContext)
            : base(name, codeElement)
        {
            if (dataContext == null)
                throw new ArgumentNullException("dataContext");

            this.dataContext = dataContext;

            testParameterActions = new PatternTestParameterActions();
        }

        /// <summary>
        /// Gets the set of actions that describe the behavior of the test parameter.
        /// </summary>
        public PatternTestParameterActions TestParameterActions
        {
            get { return testParameterActions; }
        }

        /// <summary>
        /// Gets the test that owns this parameter.
        /// </summary>
        new public PatternTest Owner
        {
            get { return (PatternTest) base.Owner; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IDataBinder" /> for this test parameter.
        /// </summary>
        /// <remarks>
        /// The default value is a <see cref="ScalarDataBinder" /> bound to the anonymous
        /// data source using a <see cref="DataBinding"/> whose path is the name of this parameter and whose
        /// index is the implicit index computed by the pararameter's data context.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        public IDataBinder Binder
        {
            get
            {
                return binder ?? GetImplicitBinder();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                binder = value;
            }
        }

        /// <inheritdoc />
        public PatternTestDataContext DataContext
        {
            get { return dataContext; }
        }

        /// <inheritdoc />
        public void SetName(string value)
        {
            Name = value;
        }

        private IDataBinder GetImplicitBinder()
        {
            DataBinding binding = new TestParameterDataBinding(dataContext.ResolveImplicitDataBindingIndex(), Name, this);
            return new ScalarDataBinder(binding, "");
        }
    }
}