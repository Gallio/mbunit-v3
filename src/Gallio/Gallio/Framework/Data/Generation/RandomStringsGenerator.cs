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
    /// Generator of random <see cref="String"/> objects based on a regular expression filter mask.
    /// </summary>
    public class RandomStringsGenerator : Generator<string>
    {
        private RegexLite regex;

        /// <summary>
        /// Gets or sets regular expression pattern that serves as a filter mask.
        /// </summary>
        public string RegularExpressionPattern
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the length of the sequence of strings
        /// created by the generator.
        /// </summary>
        public int? Count
        {
            get;
            set;
        }

        /// <summary>
        /// Constructs a generator of random <see cref="String"/> objects.
        /// </summary>
        public RandomStringsGenerator()
        {
        }

        /// <inheritdoc/>
        public override IEnumerable Run()
        {
            if (RegularExpressionPattern == null)
                throw new GenerationException("The 'RegularExpressionPattern' property must be initialized.");

            if (!Count.HasValue)
                throw new GenerationException("The 'Count' property must be initialized.");

            if (Count.Value < 0)
                throw new GenerationException("The 'Count' property wich specifies the length of the sequence must be strictly positive.");

            try
            {
                regex = new RegexLite(RegularExpressionPattern);
            }
            catch (RegexLiteException exception)
            {
                throw new GenerationException(String.Format(
                    "The specified regular expression cannot be parsed ({0}).", exception.Message), exception);
            }

            return GetSequence();
        }

        private IEnumerable GetSequence()
        {
            int i = 0;

            while (i < Count.Value)
            {
                var value = regex.GetRandomString();

                if (DoFilter(value))
                {
                    yield return value;
                    i++;
                }
            }
        }
    }
}
