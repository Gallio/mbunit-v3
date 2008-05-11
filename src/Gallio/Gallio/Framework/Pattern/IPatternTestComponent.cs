// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model;

namespace Gallio.Framework.Pattern
{
    /// <summary>
    /// An interface shared by <see cref="PatternTest" /> and <see cref="PatternTestParameter" />.
    /// </summary>
    public interface IPatternTestComponent : ITestComponent
    {
        /// <summary>
        /// Gets the data context of the component.
        /// </summary>
        PatternTestDataContext DataContext { get; }

        /// <summary>
        /// Sets the name of the component.
        /// </summary>
        void SetName(string value);
    }
}
