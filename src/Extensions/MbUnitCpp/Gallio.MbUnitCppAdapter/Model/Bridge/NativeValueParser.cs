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
using Gallio.Model;
using System.Runtime.InteropServices;

namespace Gallio.MbUnitCppAdapter.Model.Bridge
{
    /// <summary>
    /// Service that parses an actual or an expected value provided by the MbUnitCpp framework
    /// as a unicode string, and returns a matching raw managed object.
    /// </summary>
    public static class NativeValueParser
    {
        /// <summary>
        /// Parses the specified field.
        /// </summary>
        /// <param name="field">The unicode string representing the native object to be parsed.</param>
        /// <param name="valueType">The managed type to be used.</param>
        /// <returns>A managed object that might safely be used by Gallio assertion failure engine.</returns>
        /// <exception cref="ModelException">Thrown if the parsing fails.</exception>
        public static object Parse(string field, NativeValueType valueType)
        {
            try
            {
                switch (valueType)
                {
                    case NativeValueType.Raw:
                    case NativeValueType.String:
                        return field;

                    case NativeValueType.Boolean:
                        return Boolean.Parse(field);

                    case NativeValueType.Char:
                        return Char.Parse(field);

                    case NativeValueType.Byte:
                        return Byte.Parse(field);

                    case NativeValueType.Int16:
                        return Int16.Parse(field);

                    case NativeValueType.UInt16:
                        return UInt16.Parse(field);

                    case NativeValueType.Int32:
                        return Int32.Parse(field);

                    case NativeValueType.UInt32:
                        return UInt32.Parse(field);

                    case NativeValueType.Int64:
                        return Int64.Parse(field);

                    case NativeValueType.UInt64:
                        return UInt64.Parse(field);

                    case NativeValueType.Single:
                        return Single.Parse(field);

                    case NativeValueType.Double:
                        return Double.Parse(field);

                    case NativeValueType.Address:
                        return NativeIntPtr.Parse(field);

                    default:
                        throw new ModelException(String.Format("Cannot parse the native unmanaged type value: unsupported type '{0}'.", valueType));
                }
            }
            catch (FormatException exception)
            {
                throw new ModelException(String.Format("Cannot parse the native unmanaged type value: invalid value '{0}' of type '{1}'.", field, valueType), exception);
            }
        }
    }
}
