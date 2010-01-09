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
using System.Collections.ObjectModel;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.Runtime.UtilityCommands
{
    /// <summary>
    /// The default utility command manager implementation.
    /// </summary>
    public class DefaultUtilityCommandManager : IUtilityCommandManager
    {
        private ComponentHandle<IUtilityCommand, UtilityCommandTraits>[] commandHandles;

        /// <summary>
        /// Creates a utility command manager.
        /// </summary>
        /// <param name="commandHandles">The command handles, not null.</param>
        public DefaultUtilityCommandManager(ComponentHandle<IUtilityCommand, UtilityCommandTraits>[] commandHandles)
        {
            this.commandHandles = commandHandles;
        }

        /// <inheritdoc />
        public IList<ComponentHandle<IUtilityCommand, UtilityCommandTraits>> CommandHandles
        {
            get { return new ReadOnlyCollection<ComponentHandle<IUtilityCommand, UtilityCommandTraits>>(commandHandles); }
        }

        /// <inheritdoc />
        public IUtilityCommand GetCommand(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (var commandHandle in commandHandles)
                if (string.Compare(commandHandle.GetTraits().Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return commandHandle.GetComponent();

            return null;
        }
    }
}
