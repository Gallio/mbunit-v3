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
using Gallio.Framework.Data;
using Gallio.Framework.Data.Generation;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;
using System.Collections;

namespace MbUnit.Framework
{
    /// <summary>
    /// <para>
    /// Provides a column of sequential values as a data source.
    /// </para>
    /// </summary>
    [CLSCompliant(false)]
    public abstract class GenerationDataAttribute : DataAttribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected GenerationDataAttribute()
        {
        }

        /// <inheritdoc />
        protected override void PopulateDataSource(IPatternScope scope, DataSource dataSource, ICodeElementInfo codeElement)
        {
            dataSource.AddDataSet(new ValueSequenceDataSet(GetSequence(scope), GetMetadata(), false));
        }

        private IEnumerable GetSequence(IPatternScope scope)
        {
            IGenerator generator = SafeGetGenerator(scope);
            return generator.Run();
        }

        private IGenerator SafeGetGenerator(IPatternScope scope)
        {
            try
            {
                return GetGenerator(scope);
            }
            catch (ArgumentException exception)
            {
                throw new PatternUsageErrorException(String.Format(
                    "The MbUnit data generator was incorrectly initialized ({0}).", exception.Message), exception);
            }
        }

        /// <summary>
        /// Returns a generator of random values.
        /// </summary>
        /// <param name="scope">The containing scope.</param>
        /// <returns>A generator.</returns>
        protected abstract IGenerator GetGenerator(IPatternScope scope);
    }
}
