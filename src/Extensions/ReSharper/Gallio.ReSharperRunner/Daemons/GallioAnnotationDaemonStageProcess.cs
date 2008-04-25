// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System;
using System.Collections.Generic;
using Gallio.Model;
using Gallio.ReSharperRunner.Daemons;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace Gallio.ReSharperRunner.Daemons
{
    internal class GallioAnnotationDaemonStageProcess : IDaemonStageProcess
    {
        private readonly IDaemonProcess process;

        public GallioAnnotationDaemonStageProcess(IDaemonProcess process)
        {
            this.process = process;
        }

        public DaemonStageProcessResult Execute()
        {
            DaemonStageProcessResult result = new DaemonStageProcessResult();
            result.FullyRehighlighted = true;

            IProjectFile projectFile = process.ProjectFile;
            GallioProjectFileState state = GallioProjectFileState.GetFileState(projectFile);
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
                                new GallioAnnotationHighlighting(annotation)));
                        }
                    }
                }

                result.Highlightings = highlightings.ToArray();
            }

            return result;
        }
    }
}