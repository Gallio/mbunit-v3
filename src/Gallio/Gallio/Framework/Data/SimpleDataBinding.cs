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

namespace Gallio.Framework.Data
{
    /// <summary>
    /// <para>
    /// A simple minimalist implementation of a data binding.
    /// </para>
    /// </summary>
    public class SimpleDataBinding : DataBinding
    {
        private readonly int? index;
        private readonly string path;

        /// <summary>
        /// Creates a new data binding with an optional index and path.
        /// </summary>
        /// <param name="index">The binding index or null if none.  <seealso cref="Index"/></param>
        /// <param name="path">The binding path or null if none.  <seealso cref="Path"/></param>
        public SimpleDataBinding(int? index, string path)
        {
            this.path = path;
            this.index = index;
        }

        /// <inheritdoc />
        public override int? Index
        {
            get { return index; }
        }

        /// <inheritdoc />
        public override string Path
        {
            get { return path; }
        }

        /// <inheritdoc />
        public override DataBinding ReplaceIndex(int? index)
        {
            return new SimpleDataBinding(index, path);
        }
    }
}