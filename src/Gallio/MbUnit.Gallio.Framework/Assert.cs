// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework
{
    /// <summary>
    /// Ignore me.  Work in progress.
    /// </summary>
    public static partial class Assert
    {
        /*
        public static IList<AssertionResult> Run(Block block)
        {
            return null;
        }

        public static void Verify(string assertionId, string message, AssertionCondition condition,
            params AssertionParam[] args)
        {
        }
         */

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actualValue"></param>
        /// <param name="message"></param>
        /// <assertion />
        public static void IsTrue(bool actualValue, string message)
        {
        }

        public static void Warning(string messageFormat, params object[] args)
        {
        }

        public static void Fail(string message)
        {
        }

        public static void AreEqual(object expectedValue, object actualValue)
        {
        }
    }
}
