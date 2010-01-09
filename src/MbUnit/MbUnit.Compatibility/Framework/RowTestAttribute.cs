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
using Gallio.Framework.Pattern;

namespace MbUnit.Framework
{
    /// <summary>
    /// Provides compatibility with MbUnit v2 row test feature.
    /// </summary>
    [Obsolete("Use the MbUnit v3 [Test] attribute instead.  The MbUnit v3 data-driven testing features have been consolidated so the [RowTest] attribute is no longer necessary.")]
    [AttributeUsage(PatternAttributeTargets.TestMethod, AllowMultiple = false, Inherited = true)]
    public class RowTestAttribute : TestAttribute
    {
    }
}
