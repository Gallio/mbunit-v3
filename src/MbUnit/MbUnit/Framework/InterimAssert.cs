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
using System.Reflection;
using System.Text;
using Gallio;
using Gallio.Framework.Assertions;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Framework.Formatting;

#pragma warning disable 1591

namespace MbUnit.Framework
{
    /// <summary>
    /// This is an interim assertion class intended to be used within
    /// MbUnit v3 tests.  We'll refactor these assertions when the new constraint framework is developed.
    /// </summary>
    /// <remarks>
    /// DO NOT USE THIS AS THE MASTER PATTERN FOR GALLIO ASSERTIONS!
    /// The real asserts will have much more diagnostic output and will
    /// be integrated more tightly with framework services for
    /// formatting and logging.
    /// </remarks>
    public static class InterimAssert
    {

        public static void AreElementsEqualIgnoringOrder<TValue>(IEnumerable<TValue> expected, IEnumerable<TValue> actual,
            Func<TValue, TValue, bool> equivalenceRelation)
        {
            LinkedList<TValue> expectedElements = new LinkedList<TValue>(expected);
            LinkedList<TValue> actualElements = new LinkedList<TValue>(actual);

            for (LinkedListNode<TValue> expectedNode = expectedElements.First; expectedNode != null; )
            {
                LinkedListNode<TValue> nextExpectedNode = expectedNode.Next;

                for (LinkedListNode<TValue> actualNode = actualElements.First; actualNode != null; actualNode = actualNode.Next)
                {
                    if (equivalenceRelation(expectedNode.Value, actualNode.Value))
                    {
                        expectedElements.Remove(expectedNode);
                        actualElements.Remove(actualNode);
                        break;
                    }
                }

                expectedNode = nextExpectedNode;
            }

            StringBuilder builder = new StringBuilder();
            if (expectedElements.Count != 0)
            {
                builder.AppendFormat("The following {0} expected element(s) were not found:\n", expectedElements.Count);

                foreach (TValue value in expectedElements)
                    builder.Append("[[").Append(value).AppendLine("]]");
            }

            if (actualElements.Count != 0)
            {
                if (builder.Length != 0)
                    builder.AppendLine();

                builder.AppendFormat("The following {0} actual element(s) were not expected:\n", actualElements.Count);

                foreach (TValue value in actualElements)
                    builder.Append("[[").Append(value).AppendLine("]]");
            }

            if (builder.Length != 0)
                Assert.Fail(builder.ToString());
        }
    }
}
