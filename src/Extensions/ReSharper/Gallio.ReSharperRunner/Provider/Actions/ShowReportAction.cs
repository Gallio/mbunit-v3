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
using System.IO;
using EnvDTE;
using JetBrains.ActionManagement;
using JetBrains.ReSharper.UnitTestExplorer;

#if RESHARPER_31
using JetBrains.Shell.VSIntegration;
#elif RESHARPER_40 || RESHARPER_41
using JetBrains.VSIntegration.Shell;
#elif RESHARPER_45
using JetBrains.VSIntegration.Application;
#else
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.VsIntegration.Application;
#endif

namespace Gallio.ReSharperRunner.Provider.Actions
{
    public abstract class ShowReportAction : GallioAction
    {
        public override void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            string sessionId = GetSessionId(context);
            if (sessionId == null)
                return;

            FileInfo formattedReport = SessionCache.GetHtmlFormattedReport(sessionId, IsCondensed);
            if (formattedReport == null)
                return;

            ShowHtmlDocument(new Uri(formattedReport.FullName));
        }

        public override bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            string sessionId = GetSessionId(context);
            if (sessionId == null)
                return false;

            return SessionCache.HasSerializedReport(sessionId);
        }

        protected abstract bool IsCondensed { get; }

        private static void ShowHtmlDocument(Uri url)
        {
#if ! RESHARPER_50_OR_NEWER
            VSShell.Instance.ApplicationObject.ItemOperations.Navigate(url.ToString(), vsNavigateOptions.vsNavigateOptionsDefault);
#else
            VSShell.Instance.ServiceProvider.Dte().ItemOperations.Navigate(url.ToString(), vsNavigateOptions.vsNavigateOptionsDefault);
#endif
        }

#if RESHARPER_50_OR_NEWER
        private static readonly Dictionary<string, string> LastRunIdCache = new Dictionary<string, string>();
#endif
        private static string GetSessionId(IDataContext context)
        {
            UnitTestSession session = GetUnitTestSession(context);
            if (session == null)
                return null;

#if ! RESHARPER_50_OR_NEWER
            return session.ID;
#else
            // HACK: Get the last RunId for correlation instead of the SessionId because the SessionId is
            // not available to the task runner in R# 5.0 but the RunId is, so we use that for correlation.
            // Unfortunately runs are transient so they are deleted after tests finish.  However before that
            // happens, the Update method will be called many times to update UI state so we should be able
            // to catch the run in flight and save it for later.
            //
            // I have asked JetBrains to provide SessionId info to TaskRunners in a future release
            // as it was done in R# 3.1. -- Jeff.
            foreach (IUnitTestRun run in session.Runs)
            {
                string runId = run.ID;
                LastRunIdCache[session.ID] = runId;
                return runId;
            }

            string lastRunId;
            LastRunIdCache.TryGetValue(session.ID, out lastRunId);
            return lastRunId;
#endif
        }
    }
}
