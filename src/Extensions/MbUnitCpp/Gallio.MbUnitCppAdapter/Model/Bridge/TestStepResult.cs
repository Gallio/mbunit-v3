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
using System.Runtime.InteropServices;
using Gallio.Model;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// A structure that holds and wraps the native results of the test step.
    /// </summary>
    public class TestStepResult
    {
        private NativeTestStepResult native;
        private MbUnitCppAssertionFailure failure;
        private IStringResolver stringResolver;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="native">The native structure that holds data.</param>
        public TestStepResult(NativeTestStepResult native, IStringResolver stringResolver)
        {
            this.native = native;
            this.stringResolver = stringResolver;
        }

        private static readonly IDictionary<NativeOutcome, TestOutcome> map = new Dictionary<NativeOutcome, TestOutcome>
        {
            {NativeOutcome.Inconclusive, TestOutcome.Inconclusive},
            {NativeOutcome.Passed, TestOutcome.Passed},
            {NativeOutcome.Failed, TestOutcome.Failed},
        };

        /// <summary>
        /// Gets the test outcome.
        /// </summary>
        public TestOutcome TestOutcome
        {
            get
            {
                return map[native.NativeOutcome];
            }
        }

        /// <summary>
        /// Gets the assertion failure.
        /// </summary>
        public MbUnitCppAssertionFailure Failure
        {
            get
            {
                if (failure == null)
                    failure = new MbUnitCppAssertionFailure(native.Failure, stringResolver);

                return failure;
            }
        }

        /// <summary>
        /// Gets the number of assertions processed.
        /// </summary>
        public int AssertCount
        {
            get
            {
                return native.AssertCount;
            }
        }
    }
}