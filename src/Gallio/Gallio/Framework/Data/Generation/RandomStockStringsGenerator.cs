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
using Gallio.Common.Text.RegularExpression;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generator of random <see cref="String"/> objects from a predefined list of string values.
    /// </summary>
    public class RandomStockStringsGenerator : RandomStringsGenerator
    {
        private readonly static Random InnerGenerator = new Random();

        /// <summary>
        /// Gets or sets the predefined string values.
        /// </summary>
        public string[] Values
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a generator of random <see cref="String"/> objects.
        /// </summary>
        public RandomStockStringsGenerator()
        {
        }

        /// <inheritdoc/>
        public override IEnumerable Run()
        {
            if (Values == null)
                throw new GenerationException("The 'Values' property must be initialized.");

            if (Values.Length == 0)
                throw new GenerationException("The 'Values' property must contain at least one element.");

            return base.Run();
        }

        /// <inheritdoc/>
        protected override string GetNextString()
        {
            return Values[InnerGenerator.Next(Values.Length)];
        }
    }
}
