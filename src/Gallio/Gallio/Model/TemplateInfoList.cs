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
using System.Collections.Generic;

namespace Gallio.Model
{
    /// <summary>
    /// Wraps a list of <see cref="ITemplate"/> for reflection.
    /// </summary>
    public sealed class TemplateInfoList : BaseInfoList<ITemplate, TemplateInfo>
    {
        /// <summary>
        /// Creates a wrapper for the specified input list of model objects.
        /// </summary>
        /// <param name="inputList">The input list</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="inputList"/> is null</exception>
        public TemplateInfoList(IList<ITemplate> inputList)
            : base(inputList)
        {
        }

        /// <inheritdoc />
        protected override TemplateInfo Wrap(ITemplate inputItem)
        {
            return new TemplateInfo(inputItem);
        }
    }
}
