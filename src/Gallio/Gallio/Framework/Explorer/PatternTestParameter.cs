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
using Gallio.Data;
using Gallio.Data.Binders;
using Gallio.Model;
using Gallio.Reflection;

namespace Gallio.Framework.Explorer
{
    /// <summary>
    /// Represents an parameter of a <see cref="PatternTest" /> derived from a field, property
    /// or method parameter.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTestParameter : BaseTestParameter, IDataSourceScope
    {
        private DataSourceTable dataSourceTable;
        private IDataBinder binder;

        /// <summary>
        /// Creates a test pattern parameter.
        /// </summary>
        /// <param name="slot">The slot</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="slot"/> is null</exception>
        public PatternTestParameter(ISlotInfo slot)
            : base(ValidateSlotArgument(slot).Name, slot, slot.ValueType)
        {
            Index = slot.Position;

            // For generic methods, we offset the position of the parameter
            // by the number of generic method parameters.  That way
            // we can assign each parameter in the method a unique index
            // by reading all parameters left to right, starting with the
            // generic parameters and moving onwards to the method parameters.
            IParameterInfo parameter = slot as IParameterInfo;
            if (parameter != null)
            {
                IFunctionInfo function = parameter.Member as IFunctionInfo;
                if (function != null)
                    Index += function.GenericParameters.Count;
            }
        }

        /// <summary>
        /// Gets the test that owns this parameter.
        /// </summary>
        new public PatternTest Owner
        {
            get { return (PatternTest) base.Owner; }
        }

        /// <summary>
        /// Gets the associated slot.
        /// </summary>
        public ISlotInfo Slot
        {
            get { return (ISlotInfo) CodeElement; }
        }

        /// <summary>
        /// Gets or sets the <see cref="IDataBinder" /> for this test parameter.
        /// </summary>
        /// <remarks>
        /// The default value is a <see cref="ScalarDataBinder" /> with a
        /// <see cref="DataBinding"/> defined by the <see cref="ITestComponent.Name" />
        /// and <see cref="ITestParameter.Index" /> and <see cref="ITestParameter.Type" />
        /// of this parameter that is resolved using the anonymous data source.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public IDataBinder Binder
        {
            get
            {
                return binder ?? CreateDefaultBinder();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                binder = value;
            }
        }

        private static ISlotInfo ValidateSlotArgument(ISlotInfo slot)
        {
            if (slot == null)
                throw new ArgumentNullException(@"slot");
            return slot;
        }

        /// <inheritdoc />
        public DataSource DefineDataSource(string name)
        {
            if (dataSourceTable == null)
                dataSourceTable = new DataSourceTable();

            return dataSourceTable.DefineDataSource(name);
        }

        /// <inheritdoc />
        public DataSource ResolveDataSource(string name)
        {
            if (dataSourceTable != null)
            {
                DataSource source = dataSourceTable.ResolveDataSource(name);
                if (source != null)
                    return source;
            }

            return Owner != null ? Owner.ResolveDataSource(name) : null;
        }

        private IDataBinder CreateDefaultBinder()
        {
            DataBinding binding = new SimpleDataBinding(Type.Resolve(false), Name, Index);
            return new ScalarDataBinder(binding, "");
        }
    }
}