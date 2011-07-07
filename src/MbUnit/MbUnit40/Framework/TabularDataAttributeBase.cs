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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Data.DataObjects;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;

namespace MbUnit.Framework
{
    /// <summary>
    /// Abstract base class to be used by implementations which use the TabularDataObjectFactory.
    /// Eliminates repetition of the filtering arrays and much of the Factory-property plumbing.
    /// </summary>
    [AttributeUsage(PatternAttributeTargets.DataContext, AllowMultiple = true, Inherited = true)]
    public abstract class TabularDataAttributeBase : DataPatternAttribute
    {
        /// <summary>
        /// Factory Type used by the FixtureMemberInvoker
        /// </summary>
        protected readonly Type factoryType = typeof(TabularDataObjectFactory);

        /// <summary>
        /// Factory Method Name used by the FixtureMemberInvoker
        /// </summary>
        protected readonly string factoryMethodName = "EmitDynamicObjectFromDataTable";

        
        /// <summary>
        /// Indices of Rows from the file to include.  If this Array is kept as null, all Rows are included 
        /// by default.
        /// </summary>
        public int[] RowIndexIncludeFilterArray { get; set; }

        /// <summary>
        /// Indices of Rows from the file to exclude.  Exclusion takes precidence over inclusion.
        /// </summary>
        public int[] RowIndexExcludeFilterArray { get; set; }

        /// <summary>
        /// Cell Values belonging to Rows which should be included.  If this Array is kept as null, all Rows 
        /// are included by default.
        /// </summary>
        public string[] CellValueIncludeFilterArray { get; set; }

        /// <summary>
        /// Cell Values belonging to Rows which should be excluded.  Exclusion takes precidence over inclusion.
        /// </summary>
        public string[] CellValueExcludeFilterArray { get; set; }

        /// <summary>
        /// Subclasses are obliged to create an implementation which generates a DataTable. This is a cliche
        /// usage of the Template Method pattern.
        /// </summary>
        protected abstract DataTable BuildDataTable();

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            // Create the invoker object which Gallio will call by reflection
            var invoker = new FixtureMemberInvoker<IEnumerable>(factoryType, scope, factoryMethodName);

            // Get an ADO DataTable from the implementation.
            DataTable dataTable = BuildDataTable();
            
            // Create the array of arguments which will be passed to the method called by the invoker
            var parameters = new object[] {dataTable, 
                this.RowIndexIncludeFilterArray, this.RowIndexExcludeFilterArray,
                this.CellValueIncludeFilterArray, this.CellValueExcludeFilterArray};

            // Create a FactoryDataSet with an invoker of our factory methods in the delegate 
            var dataSet = new FactoryDataSet(() => invoker.Invoke(parameters), FactoryKind.Auto, 0);
            dataSource.AddDataSet(dataSet);
        }
    }
}

