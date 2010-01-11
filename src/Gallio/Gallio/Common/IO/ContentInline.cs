// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common.Reflection;
using Gallio.Framework.Pattern;

namespace Gallio.Common.IO
{
    /// <summary>
    /// Represents an inline memory resource.
    /// </summary>
    public class ContentInline : Content
    {
        private readonly string contents;

        /// <summary>
        /// Constructs the representation of an inline memory resource.
        /// </summary>
        /// <param name="contents">The content of the resource as a string.</param>
        public ContentInline(string contents)
        {
            if (contents == null)
                throw new ArgumentNullException("contents");

            this.contents = contents;
        }

        /// <inheritdoc />
        public override bool IsDynamic
        {
            get
            {
                return false;
            }
        }

        /// <inheritdoc />
        public override Stream OpenStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(contents));
        }

        /// <inheritdoc />
        public override TextReader OpenTextReader()
        {
            return new StringReader(contents);
        }
    }
}