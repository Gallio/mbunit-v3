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
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// The singleton instance representing a null XML node.
    /// </summary>
    public class Null : INode
    {
        /// <summary>
        /// The singleton instance representing a null node.
        /// </summary>
        public static readonly Null Instance = new Null();

        private Null()
        {
        }

        /// <inheritdoc />
        public INode Child
        {
            get
            {
                return Instance;
            }
        }

        /// <inheritdoc />
        public bool IsNull
        {
            get
            {
                return true;
            }
        }

        /// <inheritdoc />
        public string ToXml()
        {
            return String.Empty;
        }

        /// <inheritdoc />
        public DiffSet Diff(INode expected, IXmlPathOpen path, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");

            return DiffSet.Empty;
        }

        /// <inheritdoc />
        public bool Contains(XmlPathClosed searchedItem, Options options)
        {
            if (searchedItem == null)
                throw new ArgumentNullException("searchedItem");

            return !searchedItem.IsEmpty;
        }
    }
}
