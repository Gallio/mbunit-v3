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

using Gallio.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace Gallio.ReSharperRunner.Reflection
{
    /// <summary>
    /// Provides additional information about ReSharper code elements.
    /// </summary>
    internal interface IReSharperCodeElementInfo : ICodeElementInfo
    {
        /// <summary>
        /// Gets the declared element associated with the code element, or null if none.
        /// </summary>
        IDeclaredElement DeclaredElement { get; }

        /// <summary>
        /// Gets the project to which a code element belongs, or null if none.
        /// </summary>
        IProject Project { get; }
    }
}
