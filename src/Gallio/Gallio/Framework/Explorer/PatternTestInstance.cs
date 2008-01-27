// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Model;

namespace Gallio.Framework.Explorer
{
    /// <summary>
    /// An instance of a <see cref="PatternTest" />.
    /// </summary>
    /// <seealso cref="PatternTestFramework"/>
    public class PatternTestInstance : BaseTestInstance
    {
        /// <summary>
        /// Creates an test instance.
        /// </summary>
        /// <param name="test">The test of which this is an instance</param>
        /// <param name="parent">The parent test instance, or null if this is to be the
        /// root test instance</param>
        /// <param name="name">The name of the test instance</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name" /> or <paramref name="test"/> is null</exception>
        public PatternTestInstance(PatternTest test, ITestInstance parent, string name)
            : base(test, parent, name)
        {
        }

        /// <inheritdoc />
        public override bool IsDynamic
        {
            get
            {
                return base.IsDynamic;
            }
        }

        /// <inheritdoc />
        protected override object InternalGetParameterValue(ITestParameter parameter)
        {
            return base.InternalGetParameterValue(parameter);
        }
    }
}
