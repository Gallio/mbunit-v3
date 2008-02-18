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

namespace Gallio.Framework.Data.Binders
{
    /// <summary>
    /// Abstract base class for <see cref="IDataBinder" /> that validates
    /// input arguments before passing them on to the implementation.
    /// </summary>
    public abstract class BaseDataBinder : IDataBinder
    {
        /// <inheritdoc />
        public IDataBindingAccessor Register(DataBindingContext context, IDataSourceResolver resolver)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (resolver == null)
                throw new ArgumentNullException("resolver");

            return RegisterInternal(context, resolver);
        }

        /// <summary>
        /// Implementation of <see cref="Register" />.
        /// </summary>
        /// <param name="context">The data binding context, not null</param>
        /// <param name="resolver">The data source resolver, not null</param>
        /// <returns>The data binding accessor</returns>
        protected abstract IDataBindingAccessor RegisterInternal(DataBindingContext context, IDataSourceResolver resolver);
    }
}
