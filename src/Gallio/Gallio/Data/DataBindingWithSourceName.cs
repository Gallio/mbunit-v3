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

namespace Gallio.Data
{
    /// <summary>
    /// Combines a data binding with a source name.
    /// </summary>
    public struct DataBindingWithSourceName
    {
        private readonly string sourceName;
        private readonly DataBinding binding;

        /// <summary>
        /// Creates a data binding with a source name.
        /// </summary>
        /// <param name="sourceName">The source name, or an empty string if it is anonymous</param>
        /// <param name="binding">The binding</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceName"/> or <paramref name="binding"/> is null</exception>
        public DataBindingWithSourceName(string sourceName, DataBinding binding)
        {
            if (sourceName == null)
                throw new ArgumentNullException("sourceName");
            if (binding == null)
                throw new ArgumentNullException("binding");

            this.sourceName = sourceName;
            this.binding = binding;
        }

        /// <summary>
        /// Gets the name of the source, or an empty string if it is anonymous.
        /// </summary>
        public string SourceName
        {
            get { return sourceName; }
        }

        /// <summary>
        /// Gets the data binding.
        /// </summary>
        public DataBinding Binding
        {
            get { return binding; }
        }
    }
}
