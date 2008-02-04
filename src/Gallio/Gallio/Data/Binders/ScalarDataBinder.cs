// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Data.Binders
{
    /// <summary>
    /// A scalar data binder queries a specified data source with a data binding
    /// and converts the resulting value to the requested type.
    /// It does nothing during unbinding.
    /// </summary>
    public class ScalarDataBinder : BaseDataBinder
    {
        private readonly DataBinding binding;
        private readonly string sourceName;

        /// <summary>
        /// Creates a scalar data binder.
        /// </summary>
        /// <param name="binding">The data binding</param>
        /// <param name="sourceName">The data source name to query, or an empty string if it is anonymous</param>
        public ScalarDataBinder(DataBinding binding, string sourceName)
        {
            if (binding == null)
                throw new ArgumentNullException("binding");
            if (sourceName == null)
                throw new ArgumentNullException("sourceName");

            this.binding = binding;
            this.sourceName = sourceName;
        }

        /// <inheritdoc />
        protected override IDataBindingAccessor RegisterInternal(DataBindingContext context, IDataSourceResolver resolver)
        {
            IDataSet dataSet = resolver.ResolveDataSource(sourceName);
            if (dataSet == null)
                throw new DataBindingException(String.Format("The data binder was unable to resolve a data source named '{0}'.", sourceName));

            return context.RegisterBinding(dataSet, binding);
        }
    }
}
