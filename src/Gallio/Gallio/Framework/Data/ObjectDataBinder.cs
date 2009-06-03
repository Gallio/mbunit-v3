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
using Gallio.Common.Collections;
using Gallio.Runtime.Conversions;
using Gallio.Common.Reflection;

namespace Gallio.Framework.Data
{
    /// <summary>
    /// An object data binder creates objects and binds values to its slots such as
    /// generic type parameters, constructor parameters, fields and properties.
    /// </summary>
    public class ObjectDataBinder : BaseDataBinder
    {
        private readonly ITypeInfo type;
        private readonly Dictionary<ISlotInfo, IDataBinder> slotBinders;

        /// <summary>
        /// Creates an object data binder initially without slot binders.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public ObjectDataBinder(ITypeInfo type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            this.type = type;

            slotBinders = new Dictionary<ISlotInfo, IDataBinder>();
        }

        /// <summary>
        /// Sets the binder for a slot.
        /// </summary>
        /// <param name="slot">The slot.</param>
        /// <param name="binder">The binder.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="slot"/>
        /// or <paramref name="binder"/> is null.</exception>
        public void SetSlotBinder(ISlotInfo slot, IDataBinder binder)
        {
            if (slot == null)
                throw new ArgumentNullException("slot");
            if (binder == null)
                throw new ArgumentNullException("binder");

            slotBinders[slot] = binder;
        }

        /// <inheritdoc />
        protected override IDataAccessor RegisterImpl(DataBindingContext context, IDataSourceResolver resolver)
        {
            var slotAccessors = new List<KeyValuePair<ISlotInfo,IDataAccessor>>(slotBinders.Count);

            foreach (KeyValuePair<ISlotInfo, IDataBinder> slotBinder in slotBinders)
            {
                IDataAccessor accessor = slotBinder.Value.Register(context, resolver);
                slotAccessors.Add(new KeyValuePair<ISlotInfo, IDataAccessor>(slotBinder.Key, accessor));
            }

            return new Accessor(type, slotAccessors, context.Converter);
        }

        private sealed class Accessor : BaseDataAccessor
        {
            private readonly ITypeInfo type;
            private readonly List<KeyValuePair<ISlotInfo, IDataAccessor>> slotAccessors;
            private readonly IConverter converter;

            public Accessor(ITypeInfo type, List<KeyValuePair<ISlotInfo, IDataAccessor>> slotAccessors,
                IConverter converter)
            {
                this.type = type;
                this.slotAccessors = slotAccessors;
                this.converter = converter;
            }

            protected override object GetValueImpl(IDataItem item)
            {
                KeyValuePair<ISlotInfo, object>[] slotValues = GenericCollectionUtils.ConvertAllToArray<
                    KeyValuePair<ISlotInfo, IDataAccessor>, KeyValuePair<ISlotInfo, object>>(slotAccessors,
                    delegate(KeyValuePair<ISlotInfo, IDataAccessor> slotAccessor)
                    {
                        object value = slotAccessor.Value.GetValue(item);
                        return new KeyValuePair<ISlotInfo, object>(slotAccessor.Key, value);
                    });

                ObjectCreationSpec spec = new ObjectCreationSpec(type, slotValues, converter);
                return spec.CreateInstance();
            }
        }
    }
}
