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
using Gallio.Common;

namespace Gallio.Framework.Data.Generation
{
    /// <summary>
    /// Generator of sequential <see cref="int"/> values.
    /// </summary>
    public class SequentialInt32Generator : SequentialGenerator<int>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SequentialInt32Generator()
        {
        }

        /// <inheritdoc />
        protected override IEnumerable<int> GetStartStepCountSequence()
        {
            CheckProperty(Start.Value, "Start");
            CheckProperty(Step.Value, "Step");
            CheckProperty(Count.Value, "Count");

            if (Count.Value < 0)
                throw new GenerationException("The 'Count' property must be greater than or equal to zero.");

            int d = Start.Value;

            for (int i = 0; i < Count; i++, d += Step.Value)
            {
                if (DoFilter(d))
                {
                    yield return d;
                }
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<int> GetStartEndCountSequence()
        {
            CheckProperty(Start.Value, "Start");
            CheckProperty(End.Value, "End");
            CheckProperty(Count.Value, "Count");

            if (Count.Value < 0)
                throw new GenerationException("The 'Count' property must be greater than or equal to zero.");

            int d = Start.Value;
            int step = Count.Value <= 1 ? 1 : (End.Value - Start.Value) / (Count.Value - 1);

            for (int i = 0; i < Count; i++, d += step)
            {
                if (DoFilter(d))
                {
                    yield return d;
                }
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<int> GetStartEndStepSequence()
        {
            CheckProperty(Start.Value, "Start");
            CheckProperty(End.Value, "End");
            CheckProperty(Step.Value, "Step");

            if (Step.Value == 0)
                throw new GenerationException("The 'Step' property can not be equal to zero.");

            if (End.Value != Start.Value && Math.Sign(End.Value - Start.Value) != Math.Sign(Step.Value))
                throw new GenerationException("The sequence direction specified by the 'Start' and 'End' properties " +
                    "must be consistent with the sign of the 'Step' property.");

            Func<int, bool> stopCriterion;

            if (Math.Sign(Step.Value) >= 0)
                stopCriterion = d => (d <= End.Value);
            else
                stopCriterion = d => (d >= End.Value);

            for (int d = Start.Value; stopCriterion(d); d += Step.Value)
            {
                if (DoFilter(d))
                {
                    yield return d;
                }
            }
        }

        /// <summary>
        /// Gets the default step.
        /// </summary>
        protected override int DefaultStep
        {
            get
            {
                return 1;
            }
        }
    }
}
