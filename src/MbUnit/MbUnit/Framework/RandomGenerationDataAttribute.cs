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
using Gallio.Framework;
using Gallio.Framework.Data;
using Gallio.Framework.Data.Generation;
using Gallio.Framework.Pattern;
using Gallio.Common.Reflection;
using System.Collections;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides a column of random values as a data source.
    /// </summary>
    [CLSCompliant(false)]
    public abstract class RandomGenerationDataAttribute : GenerationDataAttribute
    {
        private bool seedReported;
        private int? seed;

        /// <summary>
        /// Gets the nullable seed value.
        /// </summary>
        protected int? NullableSeed
        {
            get
            {
                return seed;
            }
        }

        /// <summary>
        /// Gets or sets the seed value for the random generator.
        /// </summary>
        public int Seed
        {
            get
            {
                return seed ?? 0;
            }

            set
            {
                seed = value;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected RandomGenerationDataAttribute()
        {
        }

        /// <inheritdoc />
        public override void Process(IPatternScope scope, ICodeElementInfo codeElement)
        {
            base.Process(scope, codeElement);

            scope.TestParameterBuilder.TestParameterActions.BindTestParameterChain.After((state, o) =>
            {
                if (!seedReported)
                {
                    seedReported = true;
                    TestLog.WriteLine("Random Number Generator Seed = " + GetRandomGeneratorSeed(scope));
                }
            });
        }

        /// <summary>
        /// Returns the seed of the inner random number generator.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns>The seed value.</returns>
        protected abstract int? GetRandomGeneratorSeed(IPatternScope scope);
    }
}
