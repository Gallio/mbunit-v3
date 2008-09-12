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
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Gallio.VisualStudio.Shell;
using Gallio.VisualStudio.Shell.ToolWindows;

namespace Gallio.VisualStudio.Sail.UI
{
    public class TestConsole : ToolWindow
    {
        private static readonly Guid Guid = new Guid("E968F45E-4E94-4f9d-9023-06AE260872B2");

        public TestConsole(IShell shell)
            : base(shell, typeof(TestConsoleControl), Guid, "Gallio Test Console")
        {
        }
    }
}
