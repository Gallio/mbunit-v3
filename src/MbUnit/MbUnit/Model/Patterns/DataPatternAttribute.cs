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
using System.Collections.Generic;
using Gallio.Data;
using Gallio.Model;
using Gallio.Reflection;
using MbUnit.Model.Builder;

namespace MbUnit.Model.Patterns
{
    /// <summary>
    /// <para>
    /// The data pattern attribute applies a data source to a fixture or test
    /// parameter declaratively.  It can be attached to a fixture class, a public property
    /// or field of a fixture, a test method or a test method parameter.  When attached
    /// to a property or field of a fixture, implies that the property or field is
    /// a fixture parameter (so the <see cref="TestParameterPatternAttribute" />
    /// may be omitted).
    /// </para>
    /// <para>
    /// The order in which items contributed by a data pattern attribute are
    /// use can be controlled via the <see cref="DecoratorPatternAttribute.Order" />
    /// property.  The contents of data sets with lower order indices are processed
    /// before those with higher indices.
    /// </para>
    /// <example>
    /// <code>
    /// // Ensures that the rows are processed in exactly the order they appear.
    /// [Test]
    /// [Row(1, "a"), Order=1)]
    /// [Row(2, "b"), Order=2)]
    /// [Row(3, "c"), Order=3)]
    /// public void Test(int x, string y) { ... }
    /// </code>
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
        | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field
            | AttributeTargets.Parameter, AllowMultiple=true, Inherited=true)]
    public abstract class DataPatternAttribute : DecoratorPatternAttribute
    {
        private string sourceName = "";
        private string description;
        private Type expectedException;

        /// <summary>
        /// <para>
        /// Gets or sets the name of the data source to create so that the values produced
        /// by this attribute can be referred to elsewhere.  Multiple data 
        /// attributes may use the same data source name to produce a compound
        /// data source consisting of all of their values combined.
        /// </para>
        /// <para>
        /// If no name is given to the data source (or it is an empty string), the data source
        /// is considered anonymous.  An anonymous data source is only visible
        /// within the scope of the code element with which the data source declaration
        /// is associated.  By default, test parameters are bound to
        /// the anonymous data source of their enclosing scope.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string SourceName
        {
            get { return sourceName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                sourceName = value;
            }
        }

        /// <summary>
        /// Gets or sets a description of the values provided by the data source.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Gets or sets the type of exception that should be thrown when the
        /// values provided by the data source are consumed by test.
        /// </summary>
        public Type ExpectedException
        {
            get { return expectedException; }
            set { expectedException = value; }
        }

        /// <inheritdoc />
        public override void ProcessTest(ITestBuilder testBuilder, ICodeElementInfo codeElement)
        {
            testBuilder.AddDecorator(Order, delegate
            {
                PopulateDataSource(testBuilder.Test.DefineDataSource(sourceName), codeElement);
            });
        }

        /// <inheritdoc />
        public override void ProcessTestParameter(ITestParameterBuilder testParameterBuilder, ICodeElementInfo codeElement)
        {
            testParameterBuilder.AddDecorator(Order, delegate
            {
                PopulateDataSource(testParameterBuilder.TestParameter.DefineDataSource(sourceName), codeElement);
            });
        }

        /// <summary>
        /// Populates the data source with the contributions of this attribute.
        /// </summary>
        /// <param name="dataSource">The data source</param>
        /// <param name="codeElement">The code element</param>
        protected virtual void PopulateDataSource(DataSource dataSource, ICodeElementInfo codeElement)
        {
        }

        /// <summary>
        /// Gets the metadata for the data source.
        /// </summary>
        /// <returns>The metadata keys and values</returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> GetMetadata()
        {
            if (description != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.Description, description);
            if (expectedException != null)
                yield return new KeyValuePair<string, string>(MetadataKeys.ExpectedException, expectedException.FullName);
        }
    }
}