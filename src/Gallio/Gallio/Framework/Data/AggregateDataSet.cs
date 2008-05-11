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
using System.Collections.Generic;
using Gallio.Collections;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// Abstract base class for an aggregate data set that combines a list of data
    /// sets according to some algorithm.
    /// </summary>
    public abstract class AggregateDataSet : BaseDataSet
    {
        private List<IDataSet> dataSets;

        /// <summary>
        /// Gets the immutable list of combined data sets.
        /// </summary>
        public IList<IDataSet> DataSets
        {
            get
            {
                if (dataSets == null)
                    return EmptyArray<IDataSet>.Instance;
                return dataSets.AsReadOnly();
            }
        }

        /// <summary>
        /// Adds a data set to the aggregate.
        /// </summary>
        /// <param name="dataSet">The data set to add</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dataSet"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="dataSet"/> is already a member of this aggregate</exception>
        public virtual void AddDataSet(IDataSet dataSet)
        {
            if (dataSet == null)
                throw new ArgumentNullException("dataSet");

            if (dataSets == null)
                dataSets = new List<IDataSet>();
            else if (dataSets.Contains(dataSet))
                throw new ArgumentException("The data set is already a member of the aggregate.", "dataSet");

            dataSets.Add(dataSet);
        }
    }
}