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
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generic abstract generator of values.
    /// </summary>
    /// <typeparam name="T">The type of the values returned by the generator.</typeparam>
    public abstract class Generator<T> : IGenerator
        where T : struct, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Generator()
        {
        }

        /// <inheritdoc />
        public abstract IEnumerable Run();

        /// <summary>
        /// Checks for the specified value to be not one of the following:
        /// <list type="bullet">
        /// <item><see cref="Double.NaN"/></item>
        /// <item><see cref="Double.PositiveInfinity"/></item>
        /// <item><see cref="Double.NegativeInfinity"/></item>
        /// <item><see cref="Double.MinValue"/></item>
        /// <item><see cref="Double.MaxValue"/></item>
        /// </list>
        /// </summary>
        /// <param name="propertyValue">The <see cref="Double"/> property value to be checked.</param>
        /// <param name="propertyName">A friendly name for the property.</param>
        /// <exception cref="GenerationException">The specified property value is invalid.</exception>
        protected static void CheckProperty(double propertyValue, string propertyName)
        {
            if (Double.IsNaN(propertyValue) ||
                Double.IsInfinity(propertyValue) ||
                propertyValue == Double.MinValue ||
                propertyValue == Double.MaxValue)
                throw new GenerationException(String.Format("The '{0}' property cannot be one of the following: " +
                    "Double.NaN, Double.PositiveInfinity, Double.NegativeInfinity, Double.MinValue, Double.MaxValue.", propertyName));
        }

        /// <summary>
        /// Checks for the specified value to be not one of the following:
        /// <list type="bullet">
        /// <item><see cref="Int32.MinValue"/></item>
        /// <item><see cref="Int32.MaxValue"/></item>
        /// </list>
        /// </summary>
        /// <param name="propertyValue">The <see cref="Double"/> property value to be checked.</param>
        /// <param name="propertyName">A friendly name for the property.</param>
        /// <exception cref="GenerationException">The specified property value is invalid.</exception>
        protected static void CheckProperty(int propertyValue, string propertyName)
        {
            if (propertyValue == Int32.MinValue || propertyValue == Int32.MaxValue)
                throw new GenerationException(String.Format("The '{0}' property cannot be one of the following: " +
                    "Int32.MinValue or Int32.MaxValue.", propertyName));
        }
    }
}
