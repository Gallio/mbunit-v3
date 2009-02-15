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
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides extended assertions for .Net 3.5.
    /// </summary>
    public abstract class AssertEx : Assert
    {
        #region That
        /// <summary>
        /// <para>
        /// Verifies that a particular condition holds true.
        /// </para>
        /// <para>
        /// If the condition evaluates to false, the assertion failure message will
        /// describe in detail the intermediate value of relevant sub-expressions within
        /// the condition.  Consequently the assertion failure will include more diagnostic
        /// information than if <see cref="Assert.IsTrue(bool)" /> were used instead.
        /// </para>
        /// </summary>
        /// <param name="condition">The conditional expression to evaluate</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="condition"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void That(Expression<System.Func<bool>> condition)
        {
            That(condition, (string)null, null);
        }

        /// <summary>
        /// <para>
        /// Verifies that a particular condition holds true.
        /// </para>
        /// <para>
        /// If the condition evaluates to false, the assertion failure message will
        /// describe in detail the intermediate value of relevant sub-expressions within
        /// the condition.  Consequently the assertion failure will include more diagnostic
        /// information than if <see cref="Assert.IsTrue(bool, string, object[])" /> were used instead.
        /// </para>
        /// </summary>
        /// <param name="condition">The conditional expression to evaluate</param>
        /// <param name="messageFormat">The custom assertion message format, or null if none</param>
        /// <param name="messageArgs">The custom assertion message arguments, or null if none</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="condition"/> is null</exception>
        /// <exception cref="AssertionException">Thrown if the verification failed unless the current <see cref="AssertionContext.AssertionFailureBehavior" /> indicates otherwise</exception>
        public static void That(Expression<System.Func<bool>> condition, string messageFormat, params object[] messageArgs)
        {
            if (condition == null)
                throw new ArgumentNullException("condition");

            AssertionFailure failure = AssertionConditionEvaluator.Eval(condition, true, messageFormat, messageArgs);
            if (failure != null)
                AssertionContext.CurrentContext.SubmitFailure(failure);
        }
        #endregion
    }
}
