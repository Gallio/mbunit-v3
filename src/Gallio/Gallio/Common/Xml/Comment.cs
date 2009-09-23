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
    /// Represents a comment tag in an XML fragment.
    /// </summary>
    public class Comment : Element, IDiffable<Comment>
    {
        /// <summary>
        /// Constructs an XML comment tag.
        /// </summary>
        /// <param name="value">The literal content of the comment tag.</param>
        public Comment(string value)
            : base(Null.Instance, String.Empty, value, AttributeCollection.Empty)
        {
        }

        /// <inheritdoc />
        public override string ToXml()
        {
            return String.Format("<!--{0}-->", Value);
        }

        /// <inheritdoc />
        public override DiffSet Diff(INode expected, IXmlPathOpen path, Options options)
        {
            return Diff((Element)expected, path, options);
        }

        /// <inheritdoc />
        public override DiffSet Diff(Element expected, IXmlPathOpen path, Options options)
        {
            if (expected is Comment)
            {
                return Diff((Comment)expected, path, options);
            }

            return base.Diff(expected, path, options);
        }

        /// <inheritdoc />
        public DiffSet Diff(Comment expected, IXmlPathOpen path, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (path == null)
                throw new ArgumentNullException("path");

            if (((options & Options.IgnoreComments) != 0) ||
                Value.Equals(expected.Value))
            {
                return DiffSet.Empty;
            }
            else
            {
                return new DiffSetBuilder()
                    .Add(new Diff(path.ToString(), "Unexpected comment found.", expected.Value, Value))
                    .ToDiffSet();
            }
        }
    }
}
