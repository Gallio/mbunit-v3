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
using System.Collections;
using System.Reflection;

namespace MbUnit.Core.Collections
{
    /// <summary>
    /// Summary description for AttributedPropertyEnumerator.
    /// </summary>
    public sealed class AttributedPropertyEnumerator : IEnumerator
    {
        private PropertyInfo[] properties;
        private IEnumerator propertyEnumerator;
        private Type customAttributeType;

        public AttributedPropertyEnumerator(Type testedType, Type customAttributeType)
        {
            if (testedType == null)
                throw new ArgumentNullException("testedType");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            this.properties = testedType.GetProperties();
            this.customAttributeType = customAttributeType;
            this.propertyEnumerator = this.properties.GetEnumerator();
        }

        public void Reset()
        {
            this.propertyEnumerator.Reset();
        }

        public PropertyInfo Current
        {
            get
            {
                return (PropertyInfo)this.propertyEnumerator.Current;
            }
        }

        Object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        public bool MoveNext()
        {
            bool success = false;
            while (true)
            {
                success = this.propertyEnumerator.MoveNext();
                if (!success)
                    break;

                PropertyInfo pi = (PropertyInfo)this.propertyEnumerator.Current;
                if (!pi.CanRead)
                    continue;

                if (pi.GetIndexParameters().Length != 0)
                    continue;

                if (TypeHelper.HasCustomAttribute(
                    pi,
                    this.customAttributeType
                    ))
                {
                    success = true;
                    break;
                }
            }

            return success;
        }
    }
}
