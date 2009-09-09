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
using Gallio.Runtime.Extensibility;

namespace Gallio.VisualStudio.Shell.UI.Commands
{
    /// <summary>
    /// A command describes an action that is presented in the Visual Studio command bar.
    /// </summary>
    [Traits(typeof(CommandTraits))]
    public interface ICommand
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="presentation">The command presentation, not null.</param>
        void Execute(ICommandPresentation presentation);

        /// <summary>
        /// Provides an opportunity for the command to update the status, text and other
        /// properties of its presentation.
        /// </summary>
        /// <param name="presentation">The command presentation, not null.</param>
        void Update(ICommandPresentation presentation);
    }
}
