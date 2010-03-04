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
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using Gallio.Common;
using Gallio.Common.Collections;
using System.Runtime.CompilerServices;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// A formatting rule that describes the structure of objects in terms of their constituent
    /// properties and fields.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Describes the object using a format similar to a C# property initializer syntax.
    /// </para>
    /// <example>
    /// {Foo.SomeType: Property = "Value", Field = "Other value"}
    /// </example>
    /// </remarks>
    public class StructuralFormattingRule : IFormattingRule
    {
        [ThreadStatic]
        private static ReentranceState state;

        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            return FormattingRulePriority.Generic;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            if (state == null)
                state = new ReentranceState();

            string result = null;
            state.Enter(reentranceCount =>
            {
                if (reentranceCount >= 3 || state.Visited.Contains(obj))
                {
                    result = "{...}";
                }
                else
                {
                    try
                    {
                        state.Visited.Add(obj);
                        result = FormatRecursive(obj, formatter);
                    }
                    finally
                    {
                        state.Visited.Remove(obj);
                    }
                }
            });

            return result;
        }

        private static string FormatRecursive(object obj, IFormatter formatter)
        {
            Type objType = obj.GetType();

            var result = new StringBuilder();
            result.Append('{');
            result.Append(objType);
            result.Append(": ");

            var accessors = new List<KeyValuePair<string, Func<object, object>>>();
            foreach (FieldInfo field in objType.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                accessors.Add(new KeyValuePair<string, Func<object, object>>(field.Name, field.GetValue));
            }

            foreach (PropertyInfo property in objType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                PropertyInfo capturedProperty = property; // avoid unintended variable capture in the lambda.
                if (property.CanRead && property.GetIndexParameters().Length == 0)
                    accessors.Add(new KeyValuePair<string, Func<object, object>>(property.Name,
                        x => capturedProperty.GetValue(x, null)));
            }

            accessors.Sort((a, b) => a.Key.CompareTo(b.Key));

            for (int i = 0; i < accessors.Count; i++)
            {
                object value;
                try
                {
                    value = accessors[i].Value(obj);
                }
                catch (Exception)
                {
                    continue;
                }

                if (i != 0)
                    result.Append(", ");

                result.Append(accessors[i].Key);
                result.Append(" = ");
                result.Append(formatter.Format(value));
            }

            result.Append('}');
            return result.ToString();
        }

        private sealed class ReentranceState
        {
            private readonly HashSet<object> visited = new HashSet<object>(ReferentialEqualityComparer<object>.Instance);
            private int currentReentranceCount;

            public HashSet<object> Visited
            {
                get { return visited; }
            }

            public void Enter(Action<int> action)
            {
                int oldReentranceCount = currentReentranceCount;
                RuntimeHelpers.ExecuteCodeWithGuaranteedCleanup(delegate
                {
                    currentReentranceCount = oldReentranceCount + 1;
                    action(currentReentranceCount);
                }, Cleanup, oldReentranceCount);
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [PrePrepareMethod]
            private void Cleanup(object oldReentranceCount, bool exceptionThrown)
            {
                currentReentranceCount = (int)oldReentranceCount;
            }
        }
    }
}
