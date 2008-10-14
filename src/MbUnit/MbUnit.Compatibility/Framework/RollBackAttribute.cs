// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides compatibility with MbUnit v2 rollback.
    /// </summary>
    [Obsolete("Use the MbUnit v3 [Rollback] attribute instead.  This attribute has been renamed to resolve incorrect casing and upgraded for .Net Framework 2.0.")]
    [AttributeUsage(PatternAttributeTargets.Test, AllowMultiple = false, Inherited = true)]
    public class RollBackAttribute : RollbackAttribute
    {
        /// <summary>
        /// Tags the test for rollback.
        /// </summary>
        public RollBackAttribute()
        {
        }

        /// <summary>
        /// Tags the test for rollback.
        /// </summary>
        /// <param name="timeout">Ignored.  The implementation uses the test's own timeout instead.</param>
        public RollBackAttribute(int timeout)
        {
        }
    }
}
