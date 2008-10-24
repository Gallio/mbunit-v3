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

namespace Gallio.Framework.Formatting
{
    /// <summary>
    /// <para>
    /// A formatting rule that describes the structure of objects in terms of their constituent
    /// properties and fields.
    /// </para>
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
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            return FormattingRulePriority.Generic;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            Type objType = obj.GetType();

            StringBuilder result = new StringBuilder();
            result.Append('{');
            result.Append(objType);
            result.Append(": ");

            var accessors = new List<KeyValuePair<string, Func<object, object>>>();
            foreach (FieldInfo field in objType.GetFields())
            {
                accessors.Add(new KeyValuePair<string, Func<object, object>>(field.Name, field.GetValue));
            }

            foreach (PropertyInfo property in objType.GetProperties())
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
                    result.Append(',');

                result.Append(' ');
                result.Append(accessors[i].Key);
                result.Append(" = ");
                result.Append(formatter.Format(value));
            }

            result.Append('}');
            return result.ToString();
        }
    }
}
