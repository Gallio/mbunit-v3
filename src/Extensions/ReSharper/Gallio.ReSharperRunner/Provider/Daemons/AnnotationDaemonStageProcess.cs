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
using System.Collections.Generic;
using Gallio.Collections;
using Gallio.Model;
using Gallio.ReSharperRunner.Provider.Daemons;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace Gallio.ReSharperRunner.Provider.Daemons
{
    internal class AnnotationDaemonStageProcess : IDaemonStageProcess
    {
        private readonly IDaemonProcess process;

        public AnnotationDaemonStageProcess(IDaemonProcess process)
        {
            this.process = process;
        }

#if RESHARPER_31 || RESHARPER_40 || RESHARPER_41
        public DaemonStageProcessResult Execute()
        {
            DaemonStageProcessResult result = new DaemonStageProcessResult();
            result.FullyRehighlighted = true;
            result.Highlightings = GetHighlightings();

            return result;
        }
#else
        public void Execute(Action<DaemonStageResult> committer)
        {
            committer(new DaemonStageResult(GetHighlightings()));
        }
#endif

        private HighlightingInfo[] GetHighlightings()
        {
            IProjectFile projectFile = process.ProjectFile;
            ProjectFileState state = ProjectFileState.GetFileState(projectFile);
            if (state != null)
            {
                List<HighlightingInfo> highlightings = new List<HighlightingInfo>();

                foreach (Annotation annotation in state.Annotations)
                {
                    IDeclaredElement declaredElement = ReSharperReflectionPolicy.GetDeclaredElement(annotation.CodeElement);
                    if (declaredElement != null)
                    {
                        foreach (IDeclaration declaration in declaredElement.GetDeclarationsIn(projectFile))
                        {
                            highlightings.Add(new HighlightingInfo(declaration.GetNameDocumentRange(),
                                AnnotationHighlighting.CreateHighlighting(annotation)));
                        }
                    }
                }

                return highlightings.ToArray();
            }

            return EmptyArray<HighlightingInfo>.Instance;
        }
    }
}