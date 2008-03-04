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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Runner.Reports
{
    /// <summary>
    /// Visits an <see cref="ExecutionLogStreamTag" />.
    /// </summary>
    public interface IExecutionLogStreamTagVisitor
    {
        /// <summary>
        /// Visits a body tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        void VisitBodyTag(ExecutionLogStreamBodyTag tag);

        /// <summary>
        /// Visits a section tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        /// <returns>True if the visitor should be called again for each child of the section tag</returns>
        void VisitSectionTag(ExecutionLogStreamSectionTag tag);

        /// <summary>
        /// Visits an embedded attachment tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        void VisitEmbedTag(ExecutionLogStreamEmbedTag tag);

        /// <summary>
        /// Visits a text tag.
        /// </summary>
        /// <param name="tag">The tag to visit</param>
        void VisitTextTag(ExecutionLogStreamTextTag tag);
    }
}
