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
    /// Summary description for AttributedMethodEnumerator.
    /// </summary>
    public sealed class AttributedMethodEnumerator : IEnumerator
    {
        private MethodInfo[] methods;
        private IEnumerator methodEnumerator;
        private Type customAttributeType;

        public AttributedMethodEnumerator(Type testedType, Type customAttributeType)
        {
            if (testedType == null)
                throw new ArgumentNullException("testedType");
            if (customAttributeType == null)
                throw new ArgumentNullException("customAttributeType");

            this.methods = testedType.GetMethods();
            this.customAttributeType = customAttributeType;
            this.methodEnumerator = methods.GetEnumerator();
        }

        public void Reset()
        {
            this.methodEnumerator.Reset();
        }

        public MethodInfo Current
        {
            get
            {
                return (MethodInfo)this.methodEnumerator.Current;
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
                success = this.methodEnumerator.MoveNext();
                if (!success)
                    break;

                if (TypeHelper.HasCustomAttribute(
                    (MethodInfo)this.methodEnumerator.Current,
                    this.customAttributeType
                    ))
                {
                    success = true;
                    break;
                }
                else
                    success = false;
            }

            return success;
        }
    }
}
