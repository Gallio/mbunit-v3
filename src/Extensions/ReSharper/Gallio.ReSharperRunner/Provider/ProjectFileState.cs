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
using System.Collections.ObjectModel;
using Gallio.Model;
using Gallio.ReSharperRunner.Provider.Daemons;
using JetBrains.ProjectModel;
using JetBrains.Util;

namespace Gallio.ReSharperRunner.Provider
{
    /// <summary>
    /// Provides state information to the <see cref="AnnotationDaemonStageProcess" />
    /// when associated with a <see cref="IProjectFile" />.  This allows the <see cref="GallioTestProvider" />
    /// to feed annotations and other information to the daemons for use in generating custom highlights.
    /// </summary>
    internal class ProjectFileState
    {
        private static readonly Key<ProjectFileState> key = new Key<ProjectFileState>(typeof(ProjectFileState).Name);
        private readonly IList<Annotation> annotations;

        /// <summary>
        /// Creates a state object with the specified annotations. 
        /// </summary>
        /// <param name="annotations">The annotations</param>
        public ProjectFileState(IList<Annotation> annotations)
        {
            this.annotations = new ReadOnlyCollection<Annotation>(annotations);
        }

        /// <summary>
        /// Sets the state associated with a file.
        /// </summary>
        /// <param name="file">The file</param>
        /// <param name="state">The associated state, or null if none</param>
        public static void SetFileState(IProjectFile file, ProjectFileState state)
        {
            file.PutData(key, state);
        }

        /// <summary>
        /// Gets the state associated with a file.
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>The associated state, or null if none</returns>
        public static ProjectFileState GetFileState(IProjectFile file)
        {
            return file.GetData(key);
        }

        /// <summary>
        /// Gets a readonly list of annotations.
        /// </summary>
        public IList<Annotation> Annotations
        {
            get { return annotations; }
        }
    }
}