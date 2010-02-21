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

using Gallio.UI.DataBinding;
using Gallio.UI.ProgressMonitoring;

namespace Gallio.UI.Menus
{
    /// <summary>
    /// Wraps an <see cref="ICommand">ICommand</see> and provides
    /// hints for the UI.
    /// </summary>
    /// <remarks>
    /// Inspired by the WPF Command pattern.
    /// http://msdn.microsoft.com/en-us/library/system.windows.input.icommand.aspx
    /// </remarks>
    public class MenuCommand
    {
        /// <summary>
        /// The command that will be executed.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Whether the command can currently be executed, or not.
        /// </summary>
        /// <remarks>
        /// Defaults to true.
        /// </remarks>
        public Observable<bool> CanExecute { get; set; }

        /// <summary>
        /// The text description of the command.
        /// </summary>
        public string Text { get; set; }

        ///<summary>
        /// The shortcut to use for the command.
        ///</summary>
        public string Shortcut { get; set; }

        // TODO: icons

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MenuCommand()
        {
            CanExecute = new Observable<bool>(true);
        }
    }
}
