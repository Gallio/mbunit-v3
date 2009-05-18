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
using Gallio.Common;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generator of sequential <see cref="Decimal"/> values.
    /// </summary>
    public class SequentialNumbersGenerator : SequentialGenerator<decimal>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SequentialNumbersGenerator()
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<decimal> GetStartStepCountSequence()
        {
            CheckProperty(Start.Value, "Start");
            CheckProperty(Step.Value, "Step");
            CheckProperty(Count.Value, "Count");

            if (Count.Value < 0)
                throw new GenerationException("The 'Count' property must be greater than or equal to zero.");

            decimal d = Start.Value;

            for (int i = 0; i < Count; i++, d += Step.Value)
            {
                yield return d;
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<decimal> GetStartStopCountSequence()
        {
            CheckProperty(Start.Value, "Start");
            CheckProperty(Stop.Value, "Stop");
            CheckProperty(Count.Value, "Count");

            if (Count.Value < 0)
                throw new GenerationException("The 'Count' property must be greater than or equal to zero.");

            decimal d = Start.Value;
            decimal step = Count.Value <= 1 ? 1 : (Stop.Value - Start.Value) / (Count.Value - 1);

            for (int i = 0; i < Count; i++, d += step)
            {
                yield return d;
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<decimal> GetStartStopStepSequence()
        {
            CheckProperty(Start.Value, "Start");
            CheckProperty(Stop.Value, "Stop");
            CheckProperty(Step.Value, "Step");

            if (Step.Value == 0)
                throw new GenerationException("The 'Step' property can not be equal to zero.");

            if (Stop.Value != Start.Value && Math.Sign(Stop.Value - Start.Value) != Math.Sign(Step.Value))
                throw new GenerationException("The sequence direction specified by the 'Start' and 'Stop' properties " +
                    "must be consistent with the sign of the 'Step' property.");

            Func<decimal, bool> stopCriterion;

            if (Math.Sign(Step.Value) >= 0)
                stopCriterion = d => (d <= Stop.Value);
            else
                stopCriterion = d => (d >= Stop.Value);

            for (decimal d = Start.Value; stopCriterion(d); d += Step.Value)
            {
                yield return d;
            }
        }
    }
}
