// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Reflection;
using Gallio.Reflection;

using XunitAttributeInfo = Xunit.Sdk.IAttributeInfo;

namespace Gallio.XunitAdapter.Model
{
    /// <summary>
    /// An adapter for converting <see cref="IAttributeInfo" /> into <see cref="XunitAttributeInfo" />.
    /// </summary>
    internal class XunitAttributeInfoAdapter : XunitAttributeInfo
    {
        private readonly IAttributeInfo target;

        public XunitAttributeInfoAdapter(IAttributeInfo target)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            this.target = target;
        }

        public T GetInstance<T>() where T : Attribute
        {
            return (T)target.Resolve();
        }

        public TValue GetPropertyValue<TValue>(string propertyName)
        {
            try
            {
                return (TValue)target.GetPropertyValue(propertyName);
            }
            catch (ArgumentException)
            {
                // Note: xUnit.Net looks for attribute property values even in cases
                // where they are not initialized via the property such as the
                // Name property of a Trait.  So we eat the exception and try to 
                // fall back on the property of the real attribute instance itself.
                object attrib = target.Resolve();
                PropertyInfo property = attrib.GetType().GetProperty(propertyName);
                if (property == null)
                    throw;

                return (TValue)property.GetValue(attrib, null);
            }
        }
    }
}
