// Copyright 2005-2011 Gallio Project - http://www.gallio.org/
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

using Gallio.Framework.Assertions;
using NHamcrest;

namespace MbUnit.Framework
{
    public abstract partial class Assert
    {
        ///<summary>
        /// Asserts that an item satisfies the condition specified by matcher.
        ///</summary>
        ///<param name="item">The item to test.</param>
        ///<param name="matcher">A <see cref="IMatcher{T}">matcher</see>.</param>
        ///<typeparam name="T">The static type accepted by the matcher.</typeparam>
        public static void That<T>(T item, IMatcher<T> matcher)
        {
            That(item, matcher, null, null);
        }

        ///<summary>
        /// Asserts that an item satisfies the condition specified by matcher.
        ///</summary>
        ///<param name="item">The item to test.</param>
        ///<param name="matcher">A <see cref="IMatcher{T}">matcher</see>.</param>
        ///<param name="messageFormat">The custom assertion message format, or null if none.</param>
        ///<param name="messageArgs">The custom assertion message arguments, or null if none.</param>
        ///<typeparam name="T">The static type accepted by the matcher.</typeparam>
        public static void That<T>(T item, IMatcher<T> matcher, string messageFormat, params object[] messageArgs)
        {
            AssertionHelper.Verify(() =>
            {
                if (matcher.Matches(item))
                    return null;

                var description = new StringDescription();
                var mismatchDescription = new StringDescription();

                matcher.DescribeTo(description);
                matcher.DescribeMismatch(item, mismatchDescription);

                return new AssertionFailureBuilder("Expected " + description)
                    .SetMessage(messageFormat, messageArgs)
                    .AddLabeledValue("Expected", description.ToString())
                    .AddLabeledValue("But", mismatchDescription.ToString())
                    .ToAssertionFailure();
            });           
        }
    }
}