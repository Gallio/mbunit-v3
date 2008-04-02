using System;
using System.Collections.Generic;
using Gallio.Model;
using Gallio.ReSharperRunner.Reflection;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Daemon;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;

namespace Gallio.ReSharperRunner
{
    internal class GallioDaemonStageProcess : IDaemonStageProcess
    {
        private readonly IDaemonProcess process;

        public GallioDaemonStageProcess(IDaemonProcess process)
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
                            highlightings.Add(new HighlightingInfo(declaration.GetDocumentRange(),
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
