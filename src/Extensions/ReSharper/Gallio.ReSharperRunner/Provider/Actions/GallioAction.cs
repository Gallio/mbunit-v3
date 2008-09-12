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
using Gallio.Loader;
using Gallio.ReSharperRunner.Runtime;
using JetBrains.ReSharper.UnitTestExplorer;
using JetBrains.ActionManagement;

#if RESHARPER_31
using JetBrains.ReSharper.TreeModelBrowser;
#else
using JetBrains.IDE.TreeBrowser;
#endif

namespace Gallio.ReSharperRunner.Provider.Actions
{
    public abstract class GallioAction : IActionHandler
    {
        static GallioAction()
        {
            GallioLoader.Initialize().SetupRuntime();
        }

        public abstract bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate);

        public abstract void Execute(IDataContext context, DelegateExecute nextExecute);

        protected static UnitTestSession GetUnitTestSession(IDataContext context)
        {
            return context.GetData(TreeModelBrowser.TREE_MODEL_DESCRIPTOR) as UnitTestSession;
        }
    }
}
