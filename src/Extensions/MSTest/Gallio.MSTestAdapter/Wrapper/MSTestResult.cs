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
using Gallio.Model;

namespace Gallio.MSTestAdapter.Wrapper
{
    /// <summary>
    /// Describes the result of an MSTest test.
    /// </summary>
    internal sealed class MSTestResult
    {
        private IList<MSTestResult> children;

        public string Guid { get; set; }
        public TimeSpan Duration { get; set; }
        public TestOutcome Outcome { get; set; }
        public string StdOut { get; set; }
        public string Errors { get; set; }

        public IList<MSTestResult> Children
        {
            get { return children ?? EmptyArray<MSTestResult>.Instance; }
        }

        public void AddChild(MSTestResult child)
        {
            if (children == null)
                children = new List<MSTestResult>();
            children.Add(child);
        }
    }
}
