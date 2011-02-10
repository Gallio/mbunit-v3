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
using Gallio.Common.Security;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generic abstract generator of random values within a given range.
    /// </summary>
    /// <typeparam name="T">The type of the value to generate.</typeparam>
    public abstract class RandomGenerator<T> : Generator<T>
        where T : IEquatable<T>
    {
        private static readonly FnvHasher hasher = new FnvHasher();
        private int seed;
        private Random innerGenerator;

        /// <summary>
        /// A general purpose pseudo-random number generator.
        /// </summary>
        protected Random InnerGenerator
        {
            get
            {
                return innerGenerator;
            }
        }

        /// <summary>
        /// Gets or sets the seed value.
        /// </summary>
        public int? Seed
        {
            get
            {
                return seed;
            }

            set
            {
                if (value.HasValue)
                {
                    seed = value.Value;
                    innerGenerator = new Random(seed);
                }
            }
        }

        /// <summary>
        /// Gets the length of the sequence of values
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
        protected RandomGenerator()
        {
            seed = hasher.Add(DateTime.Now.Ticks).ToValue();
            innerGenerator = new Random(seed);
        }

        /// <summary>
        /// Verifies the properties of the generator.
        /// </summary>
        protected virtual void Verify()
        {
            if (!Count.HasValue)
                throw new GenerationException("The 'Count' property must be initialized.");
            if (Count.Value < 0)
                throw new GenerationException("The 'Count' property wich specifies the length of the sequence must be strictly positive.");

            CheckProperty(Count.Value, "Count");
        }

        /// <inheritdoc/>
        public override IEnumerable Run()
        {
            Verify();
            return GetSequence();
        }

        /// <summary>
        /// Returns the sequence of random values.
        /// </summary>
        /// <returns>The sequence of values.</returns>
        protected abstract IEnumerable<T> GetSequence();
    }
}