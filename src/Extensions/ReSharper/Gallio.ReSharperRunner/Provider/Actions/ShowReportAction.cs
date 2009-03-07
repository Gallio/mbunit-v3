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
using System.IO;
using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.UnitTestExplorer;

#if RESHARPER_31
using JetBrains.Shell.VSIntegration;
#elif RESHARPER_40 || RESHARPER_41
using JetBrains.VSIntegration.Shell;
#else
using JetBrains.VSIntegration.Application;
#endif

namespace Gallio.ReSharperRunner.Provider.Actions
{
    public abstract class ShowReportAction : GallioAction
    {
        public override void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            UnitTestSession session = GetUnitTestSession(context);
            if (session == null)
                return;

            FileInfo formattedReport = SessionCache.GetHtmlFormattedReport(session.ID, IsCondensed);
            if (formattedReport == null)
                return;

            ShowHtmlDocument(new Uri(formattedReport.FullName));
        }

        public override bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            UnitTestSession session = GetUnitTestSession(context);
            if (session == null)
                return false;

            return SessionCache.HasSerializedReport(session.ID);
        }

        protected abstract bool IsCondensed { get; }

        private static void ShowHtmlDocument(Uri url)
        {
            VSShell.Instance.ApplicationObject.ItemOperations.Navigate(url.ToString(), vsNavigateOptions.vsNavigateOptionsDefault);
        }
    }
}
