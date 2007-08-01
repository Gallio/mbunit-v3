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
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// <para>
    /// A parameter set groups one or more parameters logically and associates
    /// common meta-data with them.  A parameter set may be used for documentation
    /// purposes, or it take part in the data binding process by defining a
    /// group of parameters whose values are jointly drawn from a common source
    /// with multiple correlated values such as a set of columns within a single row
    /// of a table.
    /// </para>
    /// <para>
    /// All parameters belong to some parameter set.  If the name of the parameter
    /// set is empty, the set is considered anonymous.
    /// </para>
    /// </summary>
    public interface ITemplateParameterSet : ITemplateComponent
    {
        /// <summary>
        /// Gets the list of parameters in a parameter set.
        /// </summary>
        /// <remarks>
        /// The order in which the parameters appear is not significant and does
        /// not necessarily correspond to the sequence of <see cref="ITemplateParameter.Index" /> values.
        /// </remarks>
        IList<ITemplateParameter> Parameters { get; }
    }
}
