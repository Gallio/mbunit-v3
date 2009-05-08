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
using Gallio.Common;
using Gallio.Model.Execution;
using Gallio.Common.Reflection;

namespace Gallio.MSTestAdapter.Model
{
    internal class MSTestAssembly : MSTest
    {
        private readonly Version frameworkVersion;

        /// <summary>
        /// Creates an object to represent an MSTest assembly.
        /// </summary>
        /// <param name="name">The name of the component</param>
        /// <param name="codeElement">The point of definition, or null if none</param>
        /// <param name="frameworkVersion">The version number of the Microsoft.VisualStudio.QualityTools.UnitTestFramework assembly</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>, or
        /// <paramref name="frameworkVersion" /> is null</exception>
        public MSTestAssembly(string name, ICodeElementInfo codeElement, Version frameworkVersion) 
            : base(name, codeElement)
        {
            this.frameworkVersion = frameworkVersion;
        }

        /// <summary>
        /// Gets the path of the assembly.
        /// </summary>
        public string AssemblyFilePath
        {
            get { return ((IAssemblyInfo)CodeElement).Path; }
        }

        /// <inheritdoc />
        public override Func<ITestController> TestControllerFactory
        {
            get { return () => MSTestController.CreateController(frameworkVersion); }
        }
    }
}
