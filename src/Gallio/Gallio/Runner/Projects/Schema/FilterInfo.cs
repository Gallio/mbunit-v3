// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Xml.Serialization;
using Gallio.Common;
using Gallio.Common.Validation;

namespace Gallio.Runner.Projects.Schema
{
    /// <summary>
    /// Filter record for Gallio project.
    /// </summary>
    [Serializable]
    [XmlRoot("filterInfo", Namespace=SchemaConstants.XmlNamespace)]
    [XmlType(Namespace=SchemaConstants.XmlNamespace)]
    public sealed class FilterInfo : IValidatable
    {
        private string filterName;
        private string filterExpr;

        /// <summary>
        /// Parameterless constructor for serialization.
        /// </summary>
        private FilterInfo()
        {
        }

        /// <summary>
        /// Creates a filter description.
        /// </summary>
        /// <param name="filterName">The name of the filter.</param>
        /// <param name="filterExpr">The filter expression.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="filterName"/> or
        /// <paramref name="filterExpr"/> is null.</exception>
        public FilterInfo(string filterName, string filterExpr)
        {
            if (filterName == null)
                throw new ArgumentNullException("filterName");
            if (filterExpr == null)
                throw new ArgumentNullException("filterExpr");

            this.filterName = filterName;
            this.filterExpr = filterExpr;
        }

        /// <summary>
        /// Gets or sets the name of the filter.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("filterName")]
        public string FilterName
        {
            get { return filterName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                filterName = value;
            }
        }

        /// <summary>
        /// Gets or sets a string representation of the filter expression.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
        [XmlAttribute("filterExpr")]
        public string FilterExpr
        {
            get { return filterExpr; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                filterExpr = value;
            }
        }

        /// <summary>
        /// Creates a copy of the filter.
        /// </summary>
        /// <returns>The new copy.</returns>
        public FilterInfo Copy()
        {
            FilterInfo copy = new FilterInfo()
            {
                FilterName = filterName,
                FilterExpr = filterExpr
            };

            return copy;
        }

        /// <inherit />
        public void Validate()
        {
            ValidationUtils.ValidateNotNull("filterName", filterName);
            ValidationUtils.ValidateNotNull("filterExpr", filterExpr);
        }
    }
}