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
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generic abstract generator of sequential numeric values.
    /// </summary>
    /// <typeparam name="T">The type of the value to generate.</typeparam>
    public abstract class SequentialGenerator<T> : Generator<T>
        where T : struct, IComparable<T>, IEquatable<T>
    {
        /// <summary>
        /// Gets or sets the starting point of the sequence.
        /// </summary>
        public T? Start
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ending point of the sequence.
        /// </summary>
        public T? End
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the increment between each value of the sequence.
        /// </summary>
        public T? Step
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the length of the sequence of values
        /// created by the generator.
        /// </summary>
        public int? Count
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected SequentialGenerator()
        {
        }

        /// <inheritdoc/>
        public override IEnumerable Run()
        {
            if (Start.HasValue && !End.HasValue && Step.HasValue && Count.HasValue)
            {
                return GetStartStepCountSequence();
            }
            else if (Start.HasValue && End.HasValue && !Step.HasValue && Count.HasValue)
            {
                return GetStartEndCountSequence();
            }
            else if (Start.HasValue && End.HasValue && Step.HasValue && !Count.HasValue)
            {
                return GetStartEndStepSequence();
            }
            else
            {
                throw new GenerationException("Invalid data generator property settings. Only the following combinations " +
                    "are possible: {Start, Step, Count}, {Start, End, Count}, or {Start, End, Step}.");
            }
        }

        /// <summary>
        /// Returns the linear sequence constructed by using the <see cref="Start"/>, <see cref="Step"/>, and <see cref="Count"/> properties.
        /// </summary>
        /// <returns>The sequence of values.</returns>
        protected abstract IEnumerable<T> GetStartStepCountSequence();
        
        /// <summary>
        /// Returns the linear sequence constructed by using the <see cref="Start"/>, <see cref="End"/>, and <see cref="Count"/> properties.
        /// </summary>
        /// <returns>The sequence of values.</returns>
        protected abstract IEnumerable<T> GetStartEndCountSequence();
        
        /// <summary>
        /// Returns the linear sequence constructed by using the <see cref="Start"/>, <see cref="End"/>, and <see cref="Step"/> properties.
        /// </summary>
        /// <returns>The sequence of values.</returns>
        protected abstract IEnumerable<T> GetStartEndStepSequence();
    }
}
